using System.Collections;

using UnityEngine;

using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Package.Utilities;
using Sturfee.Unity.XR.Core.Localization;

public class LocalizationManager : MonoBehaviour
{
    public delegate void OnLocalizationScanStatusChangedDelegate(ScanStatus scanStatus);
    public static event OnLocalizationScanStatusChangedDelegate OnLocalizationScanStatusChanged;

    [SerializeField]
    private ScanStatus ScanStatus = ScanStatus.NotInitialized;
    private ScanStatus _scanStatus
    {
        get
        {
            return ScanStatus;
        }
        set
        {
            switch (value)
            {
                case ScanStatus.PreScan:
                    PresScan();
                    break;
                case ScanStatus.Scanning:
                    Scanning();
                    break;
                case ScanStatus.Loading:
                    Loading();
                    break;
                case ScanStatus.PostScan:
                    PostScan();
                    break;
            }

            ScanStatus = value;

            OnLocalizationScanStatusChanged?.Invoke(value);
        }
    }

    private LocalizationUI _localizationUI;
    private bool _vpsEnabled;

    private void Awake()
    {
        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail += OnLocalizationFail;
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail -= OnLocalizationFail;

    }

    private void Start()
    {
        _localizationUI = GetComponent<LocalizationUI>();
    }

    public void OnScanButton()
    {
        _scanStatus = ScanStatus.Scanning;
    }

    public void OnStopScanButton()
    {
        _scanStatus = ScanStatus.PreScan;
    }

    private void OnSessionReady()
    {
        _scanStatus = ScanStatus.PreScan;
    }

    private void OnLocalizationLoading()
    {
        _scanStatus = ScanStatus.Loading;
    }

    private void OnLocalizationSuccessful()
    {
        _localizationUI.StopScanLoadingAnimation();
        _scanStatus = ScanStatus.PostScan;
        _vpsEnabled = true;  
    }

    private void OnLocalizationFail(ErrorType errorType, string error)
    {
        Debug.Log(error);
        HandleError(error);

        if(errorType == ErrorType.RequestError)
        {
            _localizationUI.StopScanning();
        }

        if (!_vpsEnabled)
        {
            StartCoroutine(SetScanStatusDelayed(ScanStatus.PreScan, 3.0f));
        }
        else
        {
            StartCoroutine(SetScanStatusDelayed(ScanStatus.PostScan));
        }
    }

    private void PresScan()
    {
        _localizationUI.HideScanUI();
        _localizationUI.StopScanLoadingAnimation();
        _localizationUI.ShowScanButton();
    }

    private void Scanning()
    {
        _localizationUI.ShowScanUI();
        _localizationUI.StopScanLoadingAnimation();
        _localizationUI.HideScanButton();
    }

    private void Loading()
    {
        _localizationUI.HideScanUI();
        _localizationUI.PlayScanLoadingAnimation();
        _localizationUI.HideScanButton();
    }

    private void PostScan()
    {
        _localizationUI.HideScanUI();
        _localizationUI.StopScanLoadingAnimation();
        _localizationUI.HideScanButton();
    }

    private IEnumerator SetScanStatusDelayed(ScanStatus scanStatus, float delay = 2.0f)
    {
        yield return new WaitForSeconds(delay);

        _scanStatus = scanStatus;
    }

    private void HandleError(string error)
    {
        string errorToDisplay = "";

        if(error == ErrorMessages.Localization.PitchError)
        {
            errorToDisplay = "Please look forward while scanning";
        }
        else if (error == ErrorMessages.Localization.RollError)
        {
            errorToDisplay = "Please do not tilt your phone while scannning";
        }
        else if (error == ErrorMessages.Localization.YawError)
        {
            errorToDisplay = "IMU Error";
        }
        else if (error == ErrorMessages.Localization.UserMovedError)
        {
            errorToDisplay = "Moving is not allowed while scanning. Please stand still while all the frames are scanned";
        }
        else if (error == ErrorMessages.Localization.ScoreError)
        {
            errorToDisplay = "Insufficient features for localization.";
        }
        else if (error == ErrorMessages.Localization.RelocalizationDistanceError)
        {
            errorToDisplay = "Relocalization can only be performed when distance covered from last successful localization is more than 15";
        }
        else
        {
            errorToDisplay = error;
        }

        Debug.Log("Display Error : " + errorToDisplay);
        ToastManager.Instance.ShowToastTimed(errorToDisplay, 3.0f);
    }

}
