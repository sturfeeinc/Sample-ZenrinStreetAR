using UnityEngine;

using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;
using Sturfee.Unity.XR.Core.Models.Location;


public class SampleGpsProvider : GpsProviderBase
{    
    private SampleManager _sampleManage;

    private ProviderStatus _providerStatus = ProviderStatus.Initializing;
    private int _prevIndex;

    private void Start()
    {
        _sampleManage = GetComponent<SampleManager>();
        _sampleManage.OnSampleDataReady += () =>
        {
            _providerStatus = ProviderStatus.Ready;
        };
    }

    private void Update()
    {
        if(_providerStatus != ProviderStatus.Ready)
        {
            return;
        }

        if(_sampleManage.Index != _prevIndex)
        {
            XRSessionManager.GetSession().ForceLocationUpdate(GetGPSPosition());
            _prevIndex = _sampleManage.Index;
        }
    }

    public override GpsPosition GetGPSPosition()
    {
        if(_providerStatus != ProviderStatus.Ready)
        {
            Debug.Log("Sample GPsProvider is not yet ready");
            return null;
        }

        GpsPosition gpsPosition = new GpsPosition
        {
            Latitude = _sampleManage.GetDataAtCurrentIndex().latitude,
            Longitude = _sampleManage.GetDataAtCurrentIndex().longitude,
            Height = _sampleManage.GetDataAtCurrentIndex().height
        };

        return gpsPosition; 
    }

    public override ProviderStatus GetProviderStatus()
    {
        return _providerStatus;
    }

    public override void Destroy()
    {
        
    }

}
