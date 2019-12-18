using System.Collections;

using UnityEngine;

using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Package.Utilities;

public class GroundPlane : MonoBehaviour {

    private string _accessToken;

    private void Awake()
    {
        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
    }

    private void OnSessionReady ()
    {
        StartCoroutine(AddGroundAsync());
    }

    private IEnumerator AddGroundAsync()
    {
        yield return new WaitUntil(() =>
            {
                return AccessHelper.CurrentTier != Tierlevel.Checking;
            });

        AddGround();
    }

    private void AddGround ()
    {
		if (AccessHelper.CurrentTier == Tierlevel.Tier1)
        {
            Debug.Log("Adding Ground plane");
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			plane.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/BuildingHide");

            plane.transform.position = XRSessionManager.GetSession().GetXRCameraPosition() - new Vector3(0, 1.5f, 0);
        }
    }


}
