using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArItem : MonoBehaviour {

	[HideInInspector]
	public string Id;
//	public ArItemType ItemType; 

	public GameObject MapRepresentation;

	private void Start () {
        // MapRepresentation.SetActive (true);

        //MapRepresentation.transform.rotation = Quaternion.identity;
        MapRepresentation.transform.rotation = Quaternion.Euler(Vector3.right * 90);
        MapRepresentation.SetActive (true);

    }


}
