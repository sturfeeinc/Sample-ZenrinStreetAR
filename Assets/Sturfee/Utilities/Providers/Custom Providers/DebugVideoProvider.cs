using UnityEngine;
using UnityEngine.UI;

using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

public class DebugVideoProvider : VideoProviderBase {

    public int Height = 720;
    public int Width = 1280;
    public bool PortraitMode = false;

	public Texture TestImage;

    public ProviderStatus ProviderStatus = ProviderStatus.Ready;

	private Camera _backgroundCamera;
	private RawImage _rawImage;
	private AspectRatioFitter _imageFitter;

    private Texture2D _testFrame;


	private void Awake()
	{
		CreateBackgroundCameraAndCanvas ();
	}

    public override  Texture2D GetCurrentFrame ()
    {
//        byte[] imageData = File.ReadAllBytes(Path.Combine(Path.Combine(Application.dataPath + "/Sturfee", "Resources/Textures"), "TestImage.jpg"));
//        _testFrame= new Texture2D(Screen.width, Screen.height);
//        _testFrame.LoadImage(imageData);
//
		RenderTexture tempRT = RenderTexture.GetTemporary (Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
		Graphics.Blit (TestImage, tempRT);
			
		_testFrame= new Texture2D(Screen.width, Screen.height);
		_testFrame.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false); 
		_testFrame.Apply ();
        return _testFrame;
    }

    public override  int GetHeight ()
    {
        return Height;
    }

    public override  int GetWidth ()
    {
        return Width;
    }

    public override  bool IsPortrait ()
    {
        return PortraitMode;
    }

    public override ProviderStatus GetProviderStatus()
    {
        return ProviderStatus;
    }


    public override void Destroy ()
	{
        //Scene destroy will destroy these GOs. 
        if(_backgroundCamera != null)
        {
            //If session is destryoyed without destroying the scene
            UnityEngine.Object.Destroy(_backgroundCamera.gameObject);
            UnityEngine.Object.Destroy(GameObject.Find("Debug Video Provider Bg Render Canvas"));    
        }
	}

    public override Matrix4x4 GetProjectionMatrix()
    {
        return GameObject.FindWithTag(SturfeeObjects.XRCamera).GetComponent<Camera>().projectionMatrix;
    }

    public override float GetFOV()
    {
        return 40;
    }

    private void CreateBackgroundCameraAndCanvas()
	{
		_backgroundCamera = new GameObject().AddComponent<Camera>();
		_backgroundCamera.name = "Debug Video Provider Camera";
		_backgroundCamera.depth = -100;
		_backgroundCamera.nearClipPlane = 0.1f;
		_backgroundCamera.farClipPlane = 2000f;
		_backgroundCamera.orthographic = true;
		_backgroundCamera.clearFlags = CameraClearFlags.Color;
		_backgroundCamera.backgroundColor = Color.black;
		_backgroundCamera.renderingPath = RenderingPath.Forward;

		// add to proper layer and set culling pTestImageroperties
		_backgroundCamera.gameObject.layer = LayerMask.NameToLayer(SturfeeLayers.Background);
		_backgroundCamera.cullingMask = 1 << LayerMask.NameToLayer(SturfeeLayers.Background);


		var canvas = new GameObject().AddComponent<Canvas>();
		canvas.name = "Debug Video Provider Bg Render Canvas";
		canvas.renderMode = RenderMode.ScreenSpaceCamera;

		canvas.worldCamera = _backgroundCamera;
		canvas.planeDistance = 1950.0f;
		canvas.gameObject.layer = LayerMask.NameToLayer(SturfeeLayers.Background);

		_rawImage = new GameObject().AddComponent<RawImage>();
		_rawImage.name = "Raw Image";
		_rawImage.transform.parent = canvas.transform;
		_rawImage.transform.localPosition = Vector3.zero;
		_rawImage.transform.localScale = Vector3.one;

		_rawImage.texture = TestImage;
		//_rawImage.material.mainTexture = TestImage;

		_imageFitter = _rawImage.gameObject.AddComponent<AspectRatioFitter>();
		_imageFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
//		_imageFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
//		_imageFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

		_imageFitter.aspectRatio = 1280.0f / 720.0f;

//		if (Screen.orientation == ScreenOrientation.Portrait)
//		{
//			//_imageFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
//			_rawImage.rectTransform.sizeDelta = new Vector2(0, Screen.width);
//		}            
//		else
//		{
//			//_imageFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
//			_rawImage.rectTransform.sizeDelta = new Vector2(0, Screen.height);
//		}

		_backgroundCamera.gameObject.AddComponent<Sturfee.Unity.XR.Core.Utilities.BackgroundCameraRenderer> ();
	}

}
