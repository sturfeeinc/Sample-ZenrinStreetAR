using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIconUpdater : MonoBehaviour {

	private float _scale;
	private Transform _halfMapCam;


	void Start() {
		_scale = GetComponent<SizeScaleByDistance> ().Scale;
		_halfMapCam = MapManager.Instance.HalfMapCam.transform;
	}

	void Update () {


//		transform.rotation = Quaternion.Euler(Vector3.right * 90);
//
//		float distance = _fullMapCam /*FullMapCamera.Instance.transform*/.position.y - transform.position.y;
//		transform.localScale = Vector3.one * (_scale * distance);


		SetIconSize ();

//		print ("!!! UPDATE: SET MAP ICON SIZE");
	}

	public void SetIconSize()
	{
		transform.rotation = Quaternion.Euler(Vector3.right * 90);

        //float distance = /*_fullMapCam*/ FullMapCamera.Instance.transform.position.y - transform.position.y;
        float distance = _halfMapCam.position.y - transform.position.y;

		transform.localScale = Vector3.one * (_scale * distance);

//		print ("!!! FUNCTION: SET MAP ICON SIZE");
	}

	public void TurnOffUpdater()
	{
		StartCoroutine (TurnOffAfterFrame ());
	}

	private IEnumerator TurnOffAfterFrame()
	{
		yield return null;
		enabled = false;
	}
}
