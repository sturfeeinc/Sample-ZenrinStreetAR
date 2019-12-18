using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: probably remove this class, control MMC via PlayerMapTouchController
public class MiniMapCamera : MonoBehaviour {

	public static MiniMapCamera Instance;

//	public LayerMask PostScanLayerMask;

	private void Awake()
	{
		Instance = this;
	}

    private void Update()
    {
        Vector3 pos = PlayerController.Instance.transform.position;
        pos.y += 330;
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
