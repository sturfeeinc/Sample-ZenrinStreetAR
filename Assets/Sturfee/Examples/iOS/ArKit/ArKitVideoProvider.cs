using System;
using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.Rendering;

using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;


[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(SturfeeArKitManager))]
public class ArKitVideoProvider : VideoProviderBase
{
    public Material ClearMaterial;    

    private Camera _arKitBackgroundCamera;

    private CommandBuffer _videoCommandBuffer;
    private Texture2D _videoTextureY;
    private Texture2D _videoTextureCbCr;
    private Matrix4x4 _displayTransform; 
    private Resolution _arKitNativeResolution;

    private bool _bCommandBufferInitialized;

    private GCHandle _yHandle;
    private GCHandle _uvHandle;
    private int currentFrameIndex;
    private byte[] m_textureYBytes;
    private byte[] m_textureUVBytes;
    private byte[] m_textureYBytes2;
    private byte[] m_textureUVBytes2;

    private UnityARCamera _camera;
    private UnityARSessionNativeInterface _session;

    private Camera _xrCamera;
    private float _nearClipPlane;
    private float _farClipPlane;

    private void Start()
    {        
        _arKitBackgroundCamera = GetComponent<Camera>();
        if (_arKitBackgroundCamera == null)
        {
            Debug.LogError("ArKit's Background/Video camera is not available");
        }        

        _xrCamera = GameObject.FindWithTag(SturfeeObjects.XRCamera).GetComponent<Camera>();
        if (_xrCamera == null)
        {
            Debug.LogError("No XR Camera in the scene");
        }
        _nearClipPlane = _xrCamera.nearClipPlane;
        _farClipPlane = _xrCamera.farClipPlane;


        UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateFrame;
        _bCommandBufferInitialized = false;
    }

    private void Update()
    {
        UnityARSessionNativeInterface.GetARSessionNativeInterface().SetCameraClipPlanes(_nearClipPlane, _farClipPlane);
        _xrCamera.projectionMatrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraProjection();
    }


    /// <summary>
    /// Returns current frame of video as an image
    /// </summary>
    /// <returns></returns>
    public override  Texture2D GetCurrentFrame()
    {
        int width = GetWidth();
        int height = GetHeight();

        // Create a temporary render texture 
        RenderTexture tempRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);

        Graphics.Blit(Texture2D.whiteTexture, tempRT, ClearMaterial);

        // Copy the tempRT to a regular texture by reading from the current render target (i.e. tempRT)
        var snap = new Texture2D(width, height);
        snap.ReadPixels(new Rect(0, 0, width, height), 0, 0, false); // ReadPixels(Rect source, ...) ==> Rectangular region of the view to read from. ***Pixels are read from current render target.***
        snap.Apply();

        Destroy(tempRT);

        return snap;

    }

    /// <summary>
    /// Gets the height od the video texture
    /// </summary>
    /// <returns>The height.</returns>
    public override  int GetHeight()
    {
        return (int)ResizedResolution().y; 
    }

    /// <summary>
    /// Gets the width of the video texture
    /// </summary>
    /// <returns>The width.</returns>
    public override  int GetWidth()
    {
        return (int)ResizedResolution().x;
    }

    /// <summary>
    /// Determines whether the screen orientation is portrait.
    /// </summary>
    /// <returns><c>true</c> if this instance is portrait; otherwise, <c>false</c>.</returns>
    public override  bool IsPortrait()
    {
        //TODO: Update me once we start supporting Portrait mode
        return Screen.orientation == ScreenOrientation.Portrait;
    }

    public override Matrix4x4 GetProjectionMatrix()
    {
        return _xrCamera.projectionMatrix;
    }

    public override float GetFOV()
    {
        return 2.0f * Mathf.Atan(1.0f / _xrCamera.projectionMatrix[1, 1]) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Determines if the provider supports the device it is running on.
    /// </summary>
    /// <returns><c>true</c> if the device is supported; otherwise, <c>false</c>.</returns>
    public override ProviderStatus GetProviderStatus()
    {
        return GetComponent<SturfeeArKitManager>().GetProviderStatus();
    }
         

	public override void Destroy ()
	{
		
	}
        
    private void UpdateFrame(UnityARCamera cam)
    {    
        _displayTransform = new Matrix4x4();
        _displayTransform.SetColumn(0, cam.displayTransform.column0);
        _displayTransform.SetColumn(1, cam.displayTransform.column1);
        _displayTransform.SetColumn(2, cam.displayTransform.column2);
        _displayTransform.SetColumn(3, cam.displayTransform.column3);       
    }   

    void InitializeCommandBuffer()
    {		
        _videoCommandBuffer = new CommandBuffer(); 
        _videoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ClearMaterial);
        _arKitBackgroundCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _videoCommandBuffer);
        _bCommandBufferInitialized = true;

    }

    void OnDestroy()
    {		
        _arKitBackgroundCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _videoCommandBuffer);
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateFrame;
        _bCommandBufferInitialized = false;
    }

    #if !UNITY_EDITOR

    public void OnPreRender()
    {
        ARTextureHandles handles = UnityARSessionNativeInterface.GetARSessionNativeInterface ().GetARVideoTextureHandles();
        if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
        {
            return;
        }

        if (!_bCommandBufferInitialized)
        {
            InitializeCommandBuffer ();
        }

	       
        //TODO: This resolution should be same as Native ArKit texture resolution(1280 * 720).
        //We will set the GetWidth() and getHeight() to this resolution
        _arKitNativeResolution = Screen.currentResolution;

        //HACK
        //if (Screen.orientation == ScreenOrientation.Landscape)
        //{
        //    _arKitNativeResolution.width = 1280;
        //    _arKitNativeResolution.height = 720;
        //}
        //else if (Screen.orientation == ScreenOrientation.Portrait)
        //{
        //    _arKitNativeResolution.width = 720;
        //    _arKitNativeResolution.height = 1280;
        //}

        // Texture Y
        if (_videoTextureY == null) 
        {
            _videoTextureY = Texture2D.CreateExternalTexture(_arKitNativeResolution.width, _arKitNativeResolution.height,
            TextureFormat.R8, false, false, (System.IntPtr)handles.textureY);
            _videoTextureY.filterMode = FilterMode.Bilinear;
            _videoTextureY.wrapMode = TextureWrapMode.Repeat;
            ClearMaterial.SetTexture("_textureY", _videoTextureY);
        }

        // Texture CbCr
        if (_videoTextureCbCr == null)
        {
            _videoTextureCbCr = Texture2D.CreateExternalTexture(_arKitNativeResolution.width, _arKitNativeResolution.height,
            TextureFormat.RG16, false, false, (System.IntPtr)handles.textureCbCr);
            _videoTextureCbCr.filterMode = FilterMode.Bilinear;
            _videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
            ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
        }

        _videoTextureY.UpdateExternalTexture(handles.textureY);
        _videoTextureCbCr.UpdateExternalTexture(handles.textureCbCr);

        ClearMaterial.SetMatrix("_DisplayTransform", _displayTransform);
    }

    public void SetYTexure(Texture2D YTex)
    {     
        _videoTextureY = YTex;
    }

    public void SetUVTexure(Texture2D UVTex)
    {
        _videoTextureCbCr = UVTex;
    }

    #else

    public void OnPreRender()
    {
        
        if (!_bCommandBufferInitialized) {
            InitializeCommandBuffer ();
        }

        ClearMaterial.SetTexture("_textureY", _videoTextureY);
        ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);

        ClearMaterial.SetMatrix("_DisplayTransform", _displayTransform);
    }

    #endif


    IntPtr PinByteArray(ref GCHandle handle, byte[] array)
    {
        handle.Free ();
        handle = GCHandle.Alloc (array, GCHandleType.Pinned);
        return handle.AddrOfPinnedObject ();
    }

    byte [] ByteArrayForFrame(int frame,  byte[] array0,  byte[] array1)
    {
        return frame == 1 ? array1 : array0;
    }

    byte [] YByteArrayForFrame(int frame)
    {
        return ByteArrayForFrame (frame, m_textureYBytes, m_textureYBytes2);
    }

    byte [] UVByteArrayForFrame(int frame)
    {
        return ByteArrayForFrame (frame, m_textureUVBytes, m_textureUVBytes2);
    }

    private Vector2 ResizedResolution()
    {
        float aspectRatio = _xrCamera.projectionMatrix[1, 1] / _xrCamera.projectionMatrix[0, 0];
        float width, height;
        int divideBy = 1;

        if (IsPortrait())
        {
            width = Screen.width;
            height = width / aspectRatio;
            if (width > 720)
            {
                divideBy = (int)width / 720;
                if ((int)width % 720 != 0)
                {
                    divideBy++;
                }
            }
        }
        else
        {
            height = Screen.height;
            width = height * aspectRatio;
            if (height > 720)
            {
                divideBy = (int)height / 720;

                if ((int)height % 720 != 0)
                {
                    divideBy++;
                }
            }
        }

        return new Vector2(width / divideBy, height / divideBy);
    }


}
