using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour {

    public GameObject HorizontalMask;
    public GameObject VerticalMask;

    public int Speed;

	private void Update () 
    {
        HorizontalMask.transform.Rotate(0, 0, Speed * Mathf.PerlinNoise(0, 180));
        VerticalMask.transform.Rotate(0, 0, Speed * -Mathf.PerlinNoise(0, 180));
	}
}
