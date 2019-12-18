using UnityEngine;

using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

using GoogleARCore;

public class ArCoreVideoProvider :  VideoProviderBase
{
    private Camera _xrCamera;

    private void Start()
    {
        _xrCamera = GameObject.FindWithTag(SturfeeObjects.XRCamera).GetComponent<Camera>();

        if(_xrCamera == null)
        {
            Debug.LogError("No XR Camera in the scene");
        }
    }

    private void Update()
    {
        _xrCamera.projectionMatrix = Frame.CameraImage.GetCameraProjectionMatrix(_xrCamera.nearClipPlane, _xrCamera.farClipPlane);
    }

    /// <summary>
    /// Returns current frame of video as an image
    /// </summary>
    /// <returns></returns>
    public override Texture2D GetCurrentFrame()
    {
        int width = GetWidth();
        int height = GetHeight();

        // Create a temporary render texture 
        RenderTexture tempRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);

        #if UNITY_ANDROID
        Graphics.Blit(Texture2D.whiteTexture, tempRT, GetComponent<ARCoreBackgroundRenderer>().BackgroundMaterial);
        #endif

        // Copy the tempRT to a regular texture by reading from the current render target (i.e. tempRT)
        var snap = new Texture2D(width, height);
        snap.ReadPixels(new Rect(0, 0, width, height), 0, 0, false); // ReadPixels(Rect source, ...) ==> Rectangular region of the view to read from. ***Pixels are read from current render target.***
        snap.Apply();

        Destroy(tempRT);

        return snap;
    }

    /// <summary>
    /// Gets the height of the video texture
    /// </summary>
    /// <returns>The height.</returns>
    public override int GetHeight()
    {
        return(int)ResizedResolution().y;
    }


    /// <summary>
    /// Gets the width of the video texture
    /// </summary>
    /// <returns>The width.</returns>
    public override int GetWidth()
    {
        return (int)ResizedResolution().x;
    }

    /// <summary>
    /// Determines whether the screen orientation is portrait.
    /// </summary>
    /// <returns><c>true</c> if this instance is portrait; otherwise, <c>false</c>.</returns>
    public override bool IsPortrait()
    {
        return (Screen.orientation == ScreenOrientation.Portrait);
    }

    public override Matrix4x4 GetProjectionMatrix()
    {
        return _xrCamera.projectionMatrix;
    }

    public override float GetFOV()
    {
        return 2.0f * Mathf.Atan(1.0f / _xrCamera.projectionMatrix[1, 1]) * Mathf.Rad2Deg;
    }

    public override ProviderStatus GetProviderStatus()
    {
        return GetComponent<SturfeeArCoreManager>().GetProviderStatus();
    }

    public override void Destroy()
	{
		
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
