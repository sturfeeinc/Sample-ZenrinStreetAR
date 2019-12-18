using UnityEngine;
using UnityEditor;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;
using Sturfee.Unity.XR.Core.Models.Location;

public class DebugGpsProvider : GpsProviderBase 
{

    public double Latitude = 37.779044d;
    public double Longitude = -122.416594d;
    public double Height = 0;
    public ProviderStatus ProviderStatus = ProviderStatus.Ready;

    [Tooltip("Simulate the time it takes for GPS sensor until it is ready")]
    public float Delay;

    private GpsPosition _testGps = new GpsPosition();
    private float _startTime;

    private void Awake()
    {
        _testGps.Latitude = Latitude;
        _testGps.Longitude = Longitude;
        _testGps.Height = Height;
    }

    private void Start()
    {
        _startTime = Time.time;
    }

    private void Update()
    {

        if(Time.time - _startTime < Delay)
        {
            _testGps = null;
            ProviderStatus = ProviderStatus.Initializing;
            return;
        }

        if(_testGps == null)
        {
            _testGps = new GpsPosition()
            {
                Latitude = Latitude,
                Longitude = Longitude,
                Height = Height
            };

            ProviderStatus = ProviderStatus.Ready;
        }
    }


    public override  GpsPosition GetGPSPosition()
    {
        return _testGps;
    }   

    public override ProviderStatus GetProviderStatus()
    {
        return ProviderStatus;
    }
  

	public override void Destroy ()
	{
		
	}
}
