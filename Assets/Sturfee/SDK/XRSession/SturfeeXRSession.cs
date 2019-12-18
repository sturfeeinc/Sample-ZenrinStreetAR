using System.Collections;

using UnityEngine;

using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Package.Utilities;
using Sturfee.Unity.XR.Core.Providers.Base;


[RequireComponent(typeof(ProviderManager))]
public class SturfeeXRSession: MonoBehaviour
{
    public ImuProviderBase ImuProvider;
    public GpsProviderBase GpsProvider;
    public VideoProviderBase VideoProvider;

    [HideInInspector]
    public ProviderSet ProviderSet;
    [SerializeField][HideInInspector]
    private int _provTypeInt;
    [SerializeField][HideInInspector]
    public string ProviderSetName;

    private void Start()
    {
        SessionLoaderScreen.Instance.ShowLoader();
        StartSession();
    }

    private void OnDestroy()
	{
        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        SturfeeEventManager.Instance.OnSessionFailed -= OnSessionFailed; 

        XRSessionManager.DestroySession();
    }

    public void StartSession()
    {
        var xrConfig = new XRSessionConfig
        {
            ImuProvider = ImuProvider,
            GpsProvider = GpsProvider,
            VideoProvider = VideoProvider
        };

        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        SturfeeEventManager.Instance.OnSessionFailed += OnSessionFailed;

        XRSessionManager.CreateSessionWithConfig (xrConfig);
    }

    private void OnSessionReady()
    {
        Debug.Log("Session is ready");
        SessionLoaderScreen.Instance.HideLoaderSmooth();
    }

    private void OnSessionFailed(string error)
    {
        HandleError(error);
        SessionLoaderScreen.Instance.HideLoader();
    }

    private void HandleError(string error)
    {
        string errorToDisplay = "";

        if (error == ErrorMessages.Session.NoConnectivity)
        {
            errorToDisplay = " No Internet Connection ";
        }
        else if (error == ErrorMessages.Session.NotAuthorizedToken)
        {
            errorToDisplay = " Unauthorized Token ";
        }
        else if (error == ErrorMessages.Session.GpsNotAvailable)
        {
            errorToDisplay = " GPS Error ";
        }
        else if (error == ErrorMessages.Session.GpsPositionNull)
        {
            errorToDisplay = " GPS Error ";
        }
        else if (error == ErrorMessages.Session.GPSProviderNotSupported)
        {
            errorToDisplay = " GPS Error ";
        }
        else if (error == ErrorMessages.Session.NoCoverageArea)
        {
            errorToDisplay = " VPS not available at this location ";
        }
        else if (error == ErrorMessages.Session.ImuProviderNotSupported)
        {
            errorToDisplay = " IMU Error ";
        }
        else if (error == ErrorMessages.Session.VideoProviderNotSupported)
        {
            errorToDisplay = " Camera Error ";
        }
        else if (error == ErrorMessages.Session.ImuProviderTimeout)
        {
            errorToDisplay = " Session Timeout ";
        }
        else if (error == ErrorMessages.Session.GpsProviderTimeout)
        {
            errorToDisplay = " Session Timeout ";
        }
        else if (error == ErrorMessages.Session.CenterPointError)
        {
            errorToDisplay = " Tile Loading Error ";
        }
        else
        {
            errorToDisplay = error;
        }

        Debug.Log("Display Error : " + errorToDisplay);
        ToastManager.Instance.ShowToast(errorToDisplay);
    }

}
