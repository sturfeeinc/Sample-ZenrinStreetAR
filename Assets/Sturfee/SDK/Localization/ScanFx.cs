using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants;

public class ScanFx : MonoBehaviour {

    [SerializeField]
    private LocalizationUI _localizationUI;
    [SerializeField]
    private GameObject TopError;
    [SerializeField]
    private GameObject BotError;

    private void Update () 
    {
        Vector3 xrCameraEuler = XRSessionManager.GetSession().GetXRCameraOrientation().eulerAngles;

        int pitchMax = (ScanProperties.PitchMax < 0 ? 360 + ScanProperties.PitchMax : ScanProperties.PitchMax);
        int pitchMin = (ScanProperties.PitchMin < 0 ? 360 + ScanProperties.PitchMin : ScanProperties.PitchMin);

        if ( xrCameraEuler.x > pitchMax && xrCameraEuler.x < 180)
        {
            BotError.SetActive(true);
            _localizationUI.SetScanUIVisisbility(false);
        }
        else if (xrCameraEuler.x < pitchMin && xrCameraEuler.x > 180)
        {
            TopError.SetActive(true);
            _localizationUI.SetScanUIVisisbility(false);
        }
        else
        {
            TopError.SetActive(false);
            BotError.SetActive(false);
            _localizationUI.SetScanUIVisisbility(true);
        }
    }
}   
