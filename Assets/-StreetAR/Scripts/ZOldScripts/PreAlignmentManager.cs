using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants;

public class PreAlignmentManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _updateGpsButton;
    [SerializeField]
    private GameObject _mapTouchPanel;

    private void Awake()
    {
        //SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        //SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;
    }

    private void OnDestroy()
    {
        //SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        //SturfeeEventManager.Instance.OnLocalizationComplete -= OnLocalizationComplete;
    }

    private void Start()
    {
        _updateGpsButton.SetActive(false);
    }

    public void OnUpdateGpsClick()
    {
        XRSessionManager.GetSession().ForceLocationUpdate();
    }

    private void OnSessionReady()
    {
        MapManager.Instance.InitializeMap();
        //PlayerController.Instance.PreScanInitialize();

        _updateGpsButton.SetActive(true);
    }

    //private void OnLocalizationComplete()
    //{
    //    //if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.Done)
    //    //{
    //        // TODO: Add sturfee buildings
    //        _updateGpsButton.SetActive(false);
    //        StartCoroutine(InitializeGame());
    //    //}
    //}

    private IEnumerator InitializeGame()
    {
        // Need to wait one frame for corrected GPS position to update for map calls
        yield return null;

        _mapTouchPanel.SetActive(true);
        PlayerMapTouchController.Instance.gameObject.SetActive(false);

        PlayerController.Instance.Initialize();
    }

}