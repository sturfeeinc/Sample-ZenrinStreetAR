using UnityEngine;
using UnityEngine.XR.iOS;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Package.Utilities;

public class SturfeeArKitManager : MonoBehaviour {

    private ProviderStatus _providerStatus;

	private void Awake()
	{
        _providerStatus = ProviderStatus.Initializing;

        //Configure ArKit
        UnityARSessionNativeInterface arKitSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();        

#if !UNITY_EDITOR
        // config
        //Application.targetFrameRate = 60;

        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration
        {
            planeDetection = UnityARPlaneDetection.Horizontal,
            alignment = UnityARAlignment.UnityARAlignmentGravity,
            getPointCloudData = true,
            enableLightEstimation = true,
            enableAutoFocus = false
        };

        if(!config.IsSupported)            
        {
            Debug.LogError("Tracking is not supported on this device");
            ToastManager.Instance.ShowToast(" Session Error : ArKit is not supported on this device.");
            _providerStatus = ProviderStatus.NotSupported;
        }
        else
        {           
            _providerStatus = ProviderStatus.Ready;
            arKitSession.RunWithConfigAndOptions(config, UnityARSessionRunOption.ARSessionRunOptionResetTracking);
        }


#else
        UnityARCamera scamera = new UnityARCamera ();
        scamera.worldTransform = new UnityARMatrix4x4 (new Vector4 (1, 0, 0, 0), new Vector4 (0, 1, 0, 0), new Vector4 (0, 0, 1, 0), new Vector4 (0, 0, 0, 1));
        Matrix4x4 projMat = Matrix4x4.Perspective (60.0f, 1.33f, 0.1f, 30.0f);
        scamera.projectionMatrix = new UnityARMatrix4x4 (projMat.GetColumn(0),projMat.GetColumn(1),projMat.GetColumn(2),projMat.GetColumn(3));

        UnityARSessionNativeInterface.SetStaticCamera (scamera);
        _providerStatus = ProviderStatus.Ready;

        #endif

	}

    public ProviderStatus GetProviderStatus()
    {
        return _providerStatus;
    }

}
