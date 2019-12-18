using System.Collections;
using System;
using UnityEngine;

using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Models.Location;
using Sturfee.Unity.XR.Package.Utilities;
using Sturfee.Unity.XR.Core.Localization.Models;

public class HelloVPS : MonoBehaviour
{
    public int RelocalizationDistance = 15;

    [SerializeField]
    private GameObject _hitObjectPrefab;
    [SerializeField]
    private GameObject _reScanButton;      

    private bool _localized = false;
    private Vector3 _lastLocalizedPosition;

    private void Awake()
    {        
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail += OnLocalizationFail;

        SturfeeEventManager.Instance.OnDetectSurfacePointComplete += OnDetectSurfacePointComplete;
        SturfeeEventManager.Instance.OnDetectSurfacePointFailed += OnDetectSurfacePointFailed;
    }

    private void OnDestroy()
    {        
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail -= OnLocalizationFail;

        SturfeeEventManager.Instance.OnDetectSurfacePointComplete -= OnDetectSurfacePointComplete;
        SturfeeEventManager.Instance.OnDetectSurfacePointFailed -= OnDetectSurfacePointFailed;
    }

    private void Update()
    {
        if(!_localized)
        {
            return;
        }

        // Show rescan button
        float distance = Vector3.Distance(XRSessionManager.GetSession().GetXRCameraPosition(), _lastLocalizedPosition);
        _reScanButton.SetActive(distance > RelocalizationDistance);     

        // Surface detection
        if(Input.GetMouseButtonDown(0))
        {
            XRSessionManager.GetSession().DetectSurfaceAtPoint(Input.mousePosition);
        }
    }

    public void ReScan()
    {
        XRSessionManager.GetSession().PerformLocalization();
    }

    private void OnDetectSurfacePointComplete(GpsPosition gpsPosition, Vector3 normal)
    {
        Instantiate(_hitObjectPrefab, XRSessionManager.GetSession().GpsToLocalPosition(gpsPosition) + (0.01f * normal), Quaternion.LookRotation(normal));        
    }

    private void OnDetectSurfacePointFailed()
    {
        ToastManager.Instance.ShowToastTimed("Surface Detection failed");
    }

    private void OnLocalizationSuccessful()
    {
        if(!_localized)
        {
            _localized = true;
            ToastManager.Instance.ShowToastTimed(" Tap anywhere to place planes ");
        }
        
        _lastLocalizedPosition = XRSessionManager.GetSession().GetXRCameraPosition();
    }

    private void OnLocalizationFail(Sturfee.Unity.XR.Core.Localization.ErrorType errorType, string error)
    {
        if(!_localized)
        {
            return;
        }        

        StartCoroutine(ShowRescanButtonAsync());
    }

    private IEnumerator ShowRescanButtonAsync()
    {
        yield return new WaitForSeconds(2);

        if (_localized && _reScanButton != null)
        {
            _reScanButton.SetActive(true);
        }

    }

}
