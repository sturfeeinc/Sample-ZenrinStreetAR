using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour {

	public Material FadedGrid;

	void Start () {
		//StartCoroutine (FadeColor ());
	}

	public void FadeGrid()
	{
		GetComponent<Renderer> ().material = FadedGrid;
	}

	private IEnumerator FadeColor()
	{
		yield return new WaitForSeconds (2.5f);
		//GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.red);
		GetComponent<Renderer> ().material = FadedGrid;
	}
}
