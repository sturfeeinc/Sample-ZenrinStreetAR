using UnityEngine;

using Sturfee.Unity.XR.Core.Utilities;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

public class SampleImuProvider : ImuProviderBase {

    public Vector3 OffsetPosition;
    private SampleManager _sampleManager;

    private ProviderStatus _providerStatus = ProviderStatus.Initializing;

    private void Start()
    {
        _sampleManager = GetComponent<SampleManager>();

        _sampleManager.OnSampleDataReady += () =>
        {
            _providerStatus = ProviderStatus.Ready;
        };
    }

    public override Quaternion GetOrientation()
    {
        if(_providerStatus != ProviderStatus.Ready)
        {
            return Quaternion.identity;
        }

        return OrientationUtils.WorldToUnity(_sampleManager.GetDataAtCurrentIndex().quaternion);
    }

    public override Vector3 GetOffsetPosition()
    {
        return OffsetPosition;
    }

    public override ProviderStatus GetProviderStatus()
    {
        return _providerStatus;
    }

    public override void Destroy()
    {

    }

}
