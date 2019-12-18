using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Package.Utilities;
using Sturfee.Unity.XR.Core.Session;
using System;
using Sturfee.Unity.XR.Core.Exceptions;

public class RelocalizationManager : MonoBehaviour {

    public static RelocalizationManager Instance;

    //[HideInInspector]     // TODO: make private?
    public float RelocDist = 20;    // 20 for Path Arrows, 15 for Street AR

    //[Header("Scripts")]
    //[SerializeField]
    //private LocalizationManager _localizationManager;

    private float _distSinceLastReloc = 0;
    private Vector3 _lastTrackedPoint;

    // Saved variables changing localization result
    private bool _raycastTouch;
    private bool _arTouchPanelActive;   // TODO: Move to PlayerArTouchController. Make a function that checks there.

    private bool _relocalizing = false;
    private bool _initialized = false;

    private bool _tappedRelocalize;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
		
	}

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail -= OnLocalizationFail;
    }


    public void Initialize()
    {
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        SturfeeEventManager.Instance.OnLocalizationFail += OnLocalizationFail;
        _distSinceLastReloc = 0;
        _lastTrackedPoint = PlayerController.Instance.transform.position;
        _initialized = true;
    }

    // Used in 'AllowRadiusuRelocalize'
    public bool PastRelocDist() /*RadiusRelocalizeCheck()*/ /*GetDistFromLastRelocPoint()*/
    {
        //if(!_relocalizing && Vector3.Distance(_lastTrackedPoint, PlayerController.Instance.transform.position) > RelocDist)
        //{
        //    PerformRelocalization();
        //}

        return (_initialized && Vector3.Distance(_lastTrackedPoint, PlayerController.Instance.transform.position) > RelocDist) ;
    }



    // Used in 'AllowPathing'
    public bool RelocalizeCheck(bool screenTap = false)
    {
        float dist = Vector3.Distance(_lastTrackedPoint, PlayerController.Instance.transform.position);
        _distSinceLastReloc += dist;

        _lastTrackedPoint = PlayerController.Instance.transform.position;

        if ((_distSinceLastReloc) > RelocDist)
        {
        
            if (screenTap)
            {
                _raycastTouch = true;
            }
            else
            {
                // arrow hit
                PlayerArTouchController.Instance.SaveUiState = true;
            }

            UiManager.Instance.ButtonInteractibility(false);
            _arTouchPanelActive = PlayerArTouchController.Instance.gameObject.activeSelf;
            PlayerArTouchController.Instance.gameObject.SetActive(false);

            PlayerController.Instance.NeedToRelocalize = true;
            //_localizationManager.PerformLocalization();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PerformRelocalization(bool tap = false)
    {
        _tappedRelocalize = tap;

        try
        {
            _relocalizing = true;
            //Don't need to pass any parameter for relocalization
            XRSessionManager.GetSession().PerformLocalization();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);

            if (ex is PitchRequestException)
            {
                //ToastManager.Instance.ShowToastTimed("[RequestError] :: Please look forward while scanning", 5);
            }
            else if (ex is RollRequestException)
            {
                //ToastManager.Instance.ShowToastTimed("[RequestError] :: Please do not tilt your phone while scannning", 5);
            }
        }
    }

    private void OnLocalizationLoading()
    {
        _relocalizing = true;
    }

    private void OnLocalizationSuccessful()
    {
        //if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.Done)
        //{
            _distSinceLastReloc = 0;
            _lastTrackedPoint = PlayerController.Instance.transform.position;
        //}
        //else
        //{
        //    //ToastManager.Instance.ShowToastTimed("Relocalization error", 2.5f);
        //}

        //_relocalizing = false;

        //if (AppManager.Instance.AllowRadiusRelocalize)
        //{
        //    PlayerArTouchController.Instance.RaycastAfterLocalization();
        //}
        //else if (AppManager.Instance.AllowPathing)
        //{
        //    StartCoroutine(WaitAFrame());
        //}
        LocalizationReturned();
    }

    void OnLocalizationFail(Sturfee.Unity.XR.Core.Localization.ErrorType errorType, string error)
    {
        //ToastManager.Instance.ShowToastTimed("Relocalization error", 2.5f);
        LocalizationReturned();
    }

    private void LocalizationReturned()
    {
        _relocalizing = false;

        if (AppManager.Instance.AllowRadiusRelocalize && _tappedRelocalize)
        {
            _tappedRelocalize = false;
            PlayerArTouchController.Instance.RaycastAfterLocalization();
        }
        else if (AppManager.Instance.AllowPathing)
        {
            StartCoroutine(WaitAFrame());
        }
    }

    private IEnumerator WaitAFrame()
    {
        yield return null;

        UiManager.Instance.ButtonInteractibility(true);
        PlayerArTouchController.Instance.gameObject.SetActive(_arTouchPanelActive);

        if (_raycastTouch)
        {
            _raycastTouch = false;
            PlayerArTouchController.Instance.RaycastAfterLocalization();
        }
        else
        {
            PathingManager.Instance.SetNextArrow();
        }
        _arTouchPanelActive = false;
    }
}
