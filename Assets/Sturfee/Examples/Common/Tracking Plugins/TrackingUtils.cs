using UnityEngine;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Utilities;


/// <summary>
/// Helper class to use values received from tracking plugins like ArCore/ArKit with Sturfee 
/// </summary>
public class TrackingUtils : MonoBehaviour
{
    // XrCameraPos = AbsolutePos + RelativePos
    // AbsolutePos = Current GPS/VPS position converted into Local Coordinate system (Unity space).             
    // RelativePos = Position obtained from Tracking plugins like ArKit/ArCore (Read from ImuProvider.GetOffsetPosition())

    // Current GPS/VPS position converted into local coordinate system.             
    private static Vector3 _absolutePos;

    // Position from where we calculate new relativePos
    private static Vector3 _relativeOrigin;

    private bool _localized;

    private void Awake()
    {
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
    }

    private void Update()
    {
        if (!_localized)
        {
            _absolutePos = XRSessionManager.GetSession().GetXRCameraPosition();
        }
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
    }

    /// <summary>
    /// Updates position obtained from Tracking Plugin to Sturfee local position
    /// </summary>
    /// <returns>The rotated position based on offset correction received from VPS.</returns>
    /// <param name="position">Position.</param>
    public static Vector3 TrackingPosToSturfeePos(Vector3 pos)
    {
        // To align the position value from tracking plugins with Sturfee 3D models we rotate this
        // value using orientation correction received from VPS and subtract tracking
        // plugin's position value at the time of localization 
        Vector3 relativePos = (RotatePoint(pos) - RotatePoint(_relativeOrigin, false));       

        return _absolutePos + relativePos;
    }

    private void OnLocalizationSuccessful()
    {
        _localized = true;

        // We set _relativeOrigin to current position from tracking plugin. This enables us to calculate new relativePos from
        // this point onwards.
        _relativeOrigin = XRSessionManager.GetSession().ImuProvider.GetOffsetPosition();    

        //AbsolutePos is now set to VPS and at this stage XrCamPos = VPS --> Local and relativePos = 0.
        _absolutePos = XRSessionManager.GetSession().GpsToLocalPosition(XRSessionManager.GetSession().GetLocationCorrection());
    }

    /// <summary>
    /// Rotates a point based on orientation correction received from VPS service.
    /// </summary>
    /// <returns>The point.</returns>
    /// <param name="point">Position.</param>
    private static Vector3 RotatePoint(Vector3 point, bool useCompass=true)
    {       
        // Ar orgin where all values recieved rom Arcore/Arkit will be under this origin
        GameObject arOrigin = new GameObject("ArOrigin");        

        // Create a child Gameobject and set its localPosition to the point we get from ArCore/ArKit
        GameObject pointGO = new GameObject("Point");        
        pointGO.transform.parent = arOrigin.transform;
        pointGO.transform.localPosition = point;
        pointGO.transform.localRotation = Quaternion.identity;

        if (useCompass)
        {
            // First rotate this origin to align with trueHeading received from sensor
            arOrigin.transform.rotation = Quaternion.Euler(0, ArWorldTracking.TrueHeading, 0);
        }

        // Then rotate this again based on offset received from VPS Service
        Quaternion rotInWorld = XRSessionManager.GetSession().GetYawOrientationCorrection() * OrientationUtils.UnityToWorld(arOrigin.transform.rotation) * XRSessionManager.GetSession().GetPitchOrientationCorrection();
        arOrigin.transform.rotation = OrientationUtils.WorldToUnity(rotInWorld);        

        var rotatedPos = pointGO.transform.position;    // Unity WorldPos of pos

        Destroy(arOrigin);
        Destroy(pointGO);

        return rotatedPos;
    }
}
