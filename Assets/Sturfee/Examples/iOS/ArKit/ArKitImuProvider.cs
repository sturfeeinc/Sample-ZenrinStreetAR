using System.Collections;

using UnityEngine;
using UnityEngine.XR.iOS;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

[RequireComponent(typeof(SturfeeArKitManager))]
public class ArKitImuProvider: ImuProviderBase
{
   
    private Vector3 _lastPosition = Vector3.zero;
    private Quaternion _lastOrientation = Quaternion.identity;
    private Quaternion _resetOffset = Quaternion.identity;

    private bool _sessionStarted;
   

    private void Start()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;

        // ARKitWorldTrackingSessionConfiguration is deprecated in Xcode, so calculate pose readings using Compass
        gameObject.AddComponent<ArWorldTracking>();
    }

    public override Vector3 GetOffsetPosition()
    {
        Matrix4x4 matrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();
        return _lastPosition + GetWorldPosition(UnityARMatrixOps.GetPosition(matrix)); 
    }

    public override  Quaternion GetOrientation()
    {
        Matrix4x4 matrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();
        return _resetOffset * GetWorldRotation(UnityARMatrixOps.GetRotation(matrix));
    }

    public override ProviderStatus GetProviderStatus()
    {
        return GetComponent<SturfeeArKitManager>().GetProviderStatus();
    }

	public override void Destroy ()
	{
		UnityEngine.Object.Destroy(GameObject.Find("_OnAppFocusOrientationHelper"));
	}

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            StartCoroutine(ResetOrientation());
        }
        else
        {
            _lastPosition = GetOffsetPosition();
            _lastOrientation = GetOrientation();
        }
    }

    private void FirstFrameUpdate(UnityARCamera cam)
    {
        _sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
    }

    private Vector3 GetWorldPosition(Vector3 relativePos)
    {
        Vector3 worldPos = new Vector3
        {
            x = (relativePos.x * Mathf.Cos(ArWorldTracking.Theta)) - (relativePos.z * Mathf.Sin(ArWorldTracking.Theta)),
            y = relativePos.y,
            z = (relativePos.z * Mathf.Cos(ArWorldTracking.Theta)) + (relativePos.x * Mathf.Sin(ArWorldTracking.Theta))
        };

        return worldPos;
    }

    private Quaternion GetWorldRotation(Quaternion relative)
    {
        var euler = relative.eulerAngles;
        euler.y += ArWorldTracking.TrueHeading;

        return Quaternion.Euler(euler);
    }

    private IEnumerator ResetOrientation()
    {        

        Matrix4x4 matrix =  UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();

        var rotation = UnityARMatrixOps.GetRotation(matrix);
        while 
            (
                rotation == Quaternion.identity ||
                (
                   rotation.x == 0 && 
                   rotation.y == 0 && 
                   rotation.z == 0 &&
                   rotation.w == -1
                )
            )
        {            
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        var orientation = _resetOffset * GetOrientation();

        var orientationHelper = new GameObject();
        orientationHelper.name = "_OnAppFocusOrientationHelper";

        orientationHelper.transform.rotation = _lastOrientation;
        var oldForward = orientationHelper.transform.forward;

        orientationHelper.transform.rotation = orientation;
        var newForward = orientationHelper.transform.forward;  

        var angle = Quaternion.Angle(_lastOrientation, orientation);

        if (Mathf.Abs(angle) <= 30)
        {
            _resetOffset = Quaternion.FromToRotation(oldForward, newForward);
        }
    }

    //private void OnGUI()
    //{
    //    string guiText =
    //        "ArKit Pos : " + GetOffsetPosition() + "\n" +
    //        "ArKit Rot : " + GetOrientation().eulerAngles.ToString("f3") + "\n" +
    //        "Last Pos : " + _lastPosition + "\n" +
    //        "Reset Rot : " + _lastOrientation.eulerAngles.ToString("f3") + "\n" +
    //        "Status :" + _sessionStarted;

    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 40;

    //    GUI.Label(new Rect(Screen.width - 800, Screen.height - 500, 400, 400), guiText, style);
    //}
}
