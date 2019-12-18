using System.Collections;

using UnityEngine;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

using GoogleARCore;

public class ArCoreImuProvider :  ImuProviderBase
{
    private void Awake()
    {
        // Helper to get orientation with respect to true geographic north
        gameObject.AddComponent<ArWorldTracking>();
    }

    public override  Vector3 GetOffsetPosition()
    {
        return GetWorldPosition(Frame.Pose.position);
    }

    public override  Quaternion GetOrientation()
    {   
        return GetWorldRotation(Frame.Pose.rotation);
    }

    public override ProviderStatus GetProviderStatus()
    {
        return GetComponent<SturfeeArCoreManager>().GetProviderStatus();
    }

    public override void Destroy() { }
	
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

    private void OnGUI()
    {
        //string guiText =
        //    "World Pos : " + GetOffsetPosition() + "\n" +
        //    "World Rot : " + GetOrientation().eulerAngles.ToString("f3") + "\n" +
        //    "True Heading(Start) : " + ArWorldTracking.TrueHeading + "\n" +
        //    "True Heading : " + Input.compass.trueHeading + "\n" +
        //    "Status :" + Session.Status.ToString();

        //GUIStyle style = new GUIStyle();
        //style.fontSize = 40;

        //GUI.Label(new Rect(Screen.width - 800, Screen.height - 500, 400, 400), guiText, style);
    }
}
    