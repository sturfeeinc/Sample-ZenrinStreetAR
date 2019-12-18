using System.IO;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

public class SampleVideoProvider : VideoProviderBase {

    public Texture2D[] Frames;

    private SampleManager _sampleManager;
    private ProviderStatus _providerStatus;

    private Camera _xrCamera;
    private Camera _videoCamera;

    private Canvas _canvas;
    private RawImage _rawImage;
    private AspectRatioFitter _imageFitter;

    private int _framesDownloaded;

    private void Start()
    {
        _sampleManager = GetComponent<SampleManager>();
        _xrCamera = GameObject.FindWithTag(SturfeeObjects.XRCamera).GetComponent<Camera>();

        _sampleManager.OnSampleDataReady += PopulateFrames;

        _providerStatus = ProviderStatus.Initializing;
    }

    private void Update()
    {
        if(_providerStatus != ProviderStatus.Ready)
        {
            return;
        }

        _rawImage.texture = Frames[_sampleManager.Index];

        if (GetProjectionMatrix() == Matrix4x4.zero)
        {
            _xrCamera.fieldOfView = GetFOV();
        }
        else
        {
            _xrCamera.projectionMatrix = GetProjectionMatrix();
        }
    }

    public override Texture2D GetCurrentFrame()
    {
        if (_providerStatus != ProviderStatus.Ready)
        {
            Debug.LogError(" Sample Video Provider is not yet ready");
            return null;
        }

        return Frames[_sampleManager.Index];
    }

    public override int GetWidth()
    {
        if (_providerStatus != ProviderStatus.Ready)
        {
            Debug.LogError(" Sample Video Provider is not yet ready");
            return 0;
        }

        // First attempt to read from Sample Data
        int width = (int)_sampleManager.GetDataAtCurrentIndex().sceneWidth;

        // If no width data available in sample, then read from texture
        if (width == 0)
        {  
            return Frames[_sampleManager.Index].width;
        }

        return width;
    }

    public override int GetHeight()
    {
        if (_providerStatus != ProviderStatus.Ready)
        {
            Debug.LogError(" Sample Video Provider is not yet ready");
            return 0;
        }

        // First attempt to read from Sample Data
        int height = (int)_sampleManager.GetDataAtCurrentIndex().sceneHeight;

        // If no height data available in sample, then read from texture
        if (height == 0)
        {           
            return Frames[_sampleManager.Index].height;
        }

        return height;
    }

    public override bool IsPortrait()
    {
        if (_providerStatus != ProviderStatus.Ready)
        {
            Debug.LogError(" Sample Video Provider is not yet ready");
            return false;
        }

        return _sampleManager.GetDataAtCurrentIndex().isPortrait;
    }

    public override Matrix4x4 GetProjectionMatrix()
    {
        return _sampleManager.GetDataAtCurrentIndex().projectionMatrix;
    }

    public override float GetFOV()
    {
        float fov = _sampleManager.GetDataAtCurrentIndex().fov;

        if (fov == 0)
        {
            return 40;
        }

        return fov;
    }

    public override ProviderStatus GetProviderStatus()
    {
        return _providerStatus;
    }

    public override void Destroy()
    {
        
    }


    private void PopulateFrames()
    {
        Frames = new Texture2D[_sampleManager.GetDataLength()];

        _framesDownloaded = 0;

        for (int i = 0; i < Frames.Length; i++)
        {
            // First check cache
            if (File.Exists(Path.Combine(_sampleManager.CacheDirectory, "Image_" + i + ".jpg")))
            {
                StartCoroutine(DownloadFrameFromCache(i));
            }
            else    // Download from AWS
            {
                StartCoroutine(DownloadFrame(i));
            }
        }

    }

    private IEnumerator DownloadFrame(int i)
    {
        string url = "https://sdk-sample-scene.s3-us-west-1.amazonaws.com/" + _sampleManager.SampleName + "/Images/" + i + ".jpg";

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);       
        yield return unityWebRequest.SendWebRequest();

        // if url not found, remove "Images" directory from url
        if (unityWebRequest.responseCode == 404)
        {            
            url = "https://sdk-sample-scene.s3-us-west-1.amazonaws.com/" + _sampleManager.SampleName + "/" + i + ".jpg";
            unityWebRequest = UnityWebRequest.Get(url);            
            yield return unityWebRequest.SendWebRequest();
        }

        if (string.IsNullOrEmpty(unityWebRequest.error))
        {
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(unityWebRequest.downloadHandler.data);

            Frames[i] = texture2D;

            if (_sampleManager.Cache)
            {
                //Debug.Log("Caching Image_" + i + ".jpg");
                File.WriteAllBytes(Path.Combine(_sampleManager.CacheDirectory, "Image_" + i + ".jpg"), unityWebRequest.downloadHandler.data);
            }

            UpdateFramesReadyEvent();
        }
        else
        {
            Debug.LogError(unityWebRequest.error);
        }


    }

    private IEnumerator DownloadFrameFromCache(int i)
    {
        //Debug.Log("Loading Image_" + i + ".jpg from Cache");

        yield return new WaitForEndOfFrame();

        Texture2D texture2D = new Texture2D(0, 0);
        texture2D.LoadImage(File.ReadAllBytes(Path.Combine(_sampleManager.CacheDirectory, "Image_" + i + ".jpg")));

        Frames[i] = texture2D;

        UpdateFramesReadyEvent();

        yield return null;
    }

    private void UpdateFramesReadyEvent()
    {
        _framesDownloaded++;        

        if (_framesDownloaded == _sampleManager.GetDataLength())
        {
            _providerStatus = ProviderStatus.Ready;

            AddVideoCamera();
            AddCanvas();
        }
    }

    private void AddVideoCamera()
    {
        _videoCamera = new GameObject().AddComponent<Camera>();
        _videoCamera.name = "Sample video provider Background Camera";
        _videoCamera.depth = -100;
        _videoCamera.nearClipPlane = 0.1f;
        _videoCamera.farClipPlane = 2000f;
        _videoCamera.orthographic = true;
        _videoCamera.clearFlags = CameraClearFlags.Color;
        _videoCamera.backgroundColor = Color.black;
        _videoCamera.renderingPath = RenderingPath.Forward;

        // add to proper layer and set culling pTestImageroperties
        _videoCamera.gameObject.layer = LayerMask.NameToLayer(SturfeeLayers.Background);
        _videoCamera.cullingMask = 1 << LayerMask.NameToLayer(SturfeeLayers.Background);
    }

    private void AddCanvas()
    {
        _canvas = new GameObject().AddComponent<Canvas>();
        _canvas.name = "Sample Video Provider Bg Render Canvas";
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;

        _canvas.worldCamera = _videoCamera;
        _canvas.planeDistance = _videoCamera.farClipPlane - 50.0f;
        _canvas.gameObject.layer = LayerMask.NameToLayer(SturfeeLayers.Background);

        _rawImage = new GameObject().AddComponent<RawImage>();
        _rawImage.name = "Raw Image";
        _rawImage.transform.parent = _canvas.transform;
        _rawImage.transform.localPosition = Vector3.zero;
        _rawImage.transform.localScale = Vector3.one;

        _imageFitter = _rawImage.gameObject.AddComponent<AspectRatioFitter>();
        _imageFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

        _imageFitter.aspectRatio = (float)GetWidth() / GetHeight();
    }

}
