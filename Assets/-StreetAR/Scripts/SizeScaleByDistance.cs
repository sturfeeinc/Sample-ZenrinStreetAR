using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeScaleByDistance : MonoBehaviour {

	public float Scale = 0.35f;

	// Use this for initialization
	void Start () {

		Activate ();
	}

	public void Activate()
	{
        float distance = MapManager.Instance.MiniMapCam.transform.position.y - transform.position.y;
        //float distance = MiniMapCamera.Instance.transform.position.y - transform.position.y;
        transform.localScale =  Vector3.one * (Scale * distance);
	}

}
