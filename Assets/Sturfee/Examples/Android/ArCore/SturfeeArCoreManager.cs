using System.Collections;

using UnityEngine;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Package.Utilities;

using GoogleARCore;

public class SturfeeArCoreManager : MonoBehaviour {

    public ProviderStatus GetProviderStatus()
    {
        ProviderStatus providerStatus = ProviderStatus.Initializing;

        switch (Session.Status)
        {
            case SessionStatus.ErrorApkNotAvailable:
                ToastManager.Instance.ShowToast("Session Error : The ARCore session cannot begin tracking because the ARCore service APK is not available on the device.");
                providerStatus = ProviderStatus.NotSupported;
                break;

            case SessionStatus.ErrorPermissionNotGranted:
                ToastManager.Instance.ShowToast("Session Error : The ARCore session cannot begin tracking because the Android camera permission is not granted..");
                providerStatus = ProviderStatus.NotSupported;
                break;

            case SessionStatus.ErrorSessionConfigurationNotSupported:
                ToastManager.Instance.ShowToast("Session Error : The ARCore session cannot begin tracking because the session configuration supplied is not supported or " +
                                                "no session configuration was supplied.");
                providerStatus = ProviderStatus.NotSupported;
                break;

            case SessionStatus.FatalError:
                ToastManager.Instance.ShowToast("Session Error : TThe ARCore session cannot begin tracking because a fatal error was encountered.");
                providerStatus = ProviderStatus.NotSupported;
                break;

            case SessionStatus.Initializing:
                providerStatus = ProviderStatus.Initializing;
                break;

            case SessionStatus.Tracking:
                providerStatus = ProviderStatus.Ready;
                break;
        }

        return providerStatus;
    }
}
