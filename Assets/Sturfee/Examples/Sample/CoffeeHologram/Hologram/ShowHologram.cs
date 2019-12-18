using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sturfee.Unity.XR.Core.Events;

public class ShowHologram : MonoBehaviour {

    public enum ShowHologramOn{ SessionReady, LocalizationSuccessful };
    public ShowHologramOn ShowHologramWhen;

    private void Awake()
    {
        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
    }   

    private void OnSessionReady()
    {
        if (ShowHologramWhen == ShowHologramOn.SessionReady)
        {
            GetComponent<HologramController>().ShowHologram();
        }
    }

    private void OnLocalizationSuccessful()
    {
        GetComponent<HologramController>().ShowHologram();
    }
}
