using UnityEngine;

using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants;

[RequireComponent(typeof(Camera))]
public class SturfeeXRCameraController : MonoBehaviour {

    private bool _sessionIsReady;

    private void Awake()
    {
        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;

        ManageLayers();
    }

    private void Update()
    {
        if (!_sessionIsReady)
        {
            return;
        }

        transform.position = XRSessionManager.GetSession().GetXRCameraPosition();
        transform.rotation = XRSessionManager.GetSession().GetXRCameraOrientation();
    }

    private void OnSessionReady()
    {
        _sessionIsReady = true;
    }

    private void ManageLayers()
    {
        Camera cam = GetComponent<Camera>();
        int layer;

        // Building
        layer = LayerMask.NameToLayer(SturfeeLayers.Building);
        if ((cam.cullingMask & (1 << layer)) == 0)
        {
            cam.cullingMask |= 1 << LayerMask.NameToLayer(SturfeeLayers.Building);
        }

        // Terrain
        layer = LayerMask.NameToLayer(SturfeeLayers.Terrain);
        if ((cam.cullingMask & (1 << layer)) == 0)
        {
            cam.cullingMask |= 1 << LayerMask.NameToLayer(SturfeeLayers.Terrain);
        }


        // Background (Remove)
        layer = LayerMask.NameToLayer(SturfeeLayers.Background);
        if ((cam.cullingMask & (1 << layer)) != 0)
        {
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer(SturfeeLayers.Background));
        }
    }
}
