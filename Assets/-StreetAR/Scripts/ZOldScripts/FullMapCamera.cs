using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Remove this
public class FullMapCamera : MonoBehaviour {

	public static FullMapCamera Instance;

//	public LayerMask PostScanLayerMask;

	void Awake()
	{
		Instance = this;
	}

    private void Update()
    {
        Vector3 pos = PlayerController.Instance.transform.position;
        pos.y += 430;
        transform.position = pos;
    }

    public void CenterCamOverPos(Vector3 pos)
	{
		transform.position = new Vector3 (pos.x, transform.position.y, pos.z);
	}

//	public void AddBuildingsToView()
//	{
//		GetComponent<Camera> ().cullingMask = PostScanLayerMask;
//	}
}
