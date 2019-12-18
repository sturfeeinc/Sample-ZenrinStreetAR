using UnityEngine;

using Sturfee.Unity.XR.Core.Providers.Base;

public class ProviderSet : MonoBehaviour 
{
    public ImuProviderBase ImuProvider;
    public GpsProviderBase GpsProvider;
    public VideoProviderBase VideoProvider;
	
}
