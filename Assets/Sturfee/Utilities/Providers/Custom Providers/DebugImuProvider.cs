using UnityEngine;

using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

public class DebugImuProvider : ImuProviderBase {

    public Vector3 TestOrientation = new Vector3(354.6f, 148.932f, 0);
    public Vector3 TestPosition;

    public ProviderStatus ProviderStatus = ProviderStatus.Ready;

    [SerializeField]
	private bool _moveOrientation = true;

    private Quaternion _testQuaternion;

    private void Awake()
	{
		SturfeeEventManager.Instance.OnLocalizationLoading += () => {_moveOrientation = false;};
	}

    private void Update()
    {
        _testQuaternion = Quaternion.Euler(TestOrientation);

        if (_moveOrientation) 
		{
			float y = Mathf.PerlinNoise (Time.time * _testQuaternion.y, 0.0f);
			y *= Mathf.Rad2Deg;
			var newQuat = _testQuaternion;
			newQuat.eulerAngles = new Vector3 (_testQuaternion.eulerAngles.x, y, _testQuaternion.eulerAngles.z);

			_testQuaternion = Quaternion.Lerp (_testQuaternion, newQuat, Time.deltaTime);

            TestOrientation = _testQuaternion.eulerAngles;
		}
    }


    public override  Quaternion GetOrientation ()
    {
        return _testQuaternion;
    }

    public override  Vector3 GetOffsetPosition ()
    {
        return TestPosition;
    }

    public override ProviderStatus GetProviderStatus()
    {
        return ProviderStatus;
    }

	public override void Destroy ()
	{
		
	}

}
