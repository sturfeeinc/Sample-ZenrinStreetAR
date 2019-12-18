using System;
using System.Collections;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;

public class GridEffect : MonoBehaviour {

	public bool ScaleByDistance = true;
	public bool ApplyFadingEffect;

	private float _fadeTime = 4.0f;

	public void Start()
	{
		ShowGrid();
	}

	public void ShowGrid()
	{
		gameObject.SetActive(true);

		if (ScaleByDistance)
		{
			Vector3 pos = (XRSessionManager.GetSession() != null) ? XRSessionManager.GetSession().GetXRCameraPosition() : Camera.main.transform.position;
			var distance = Vector3.Distance(pos, transform.position);
			var scaleAmount =  (distance * 2) / Mathf.Pow (distance, 0.75f);
            transform.localScale = Vector3.one * scaleAmount;
		}

		if (ApplyFadingEffect)
		{
			FadeInAndOut(gameObject);
		}
	}

	private void FadeInAndOut(GameObject instance)
	{
		var renderer = instance.GetComponent<Renderer>();
		if(renderer == null)
		{
			renderer = instance.GetComponentInChildren<Renderer>();
		}

		if (renderer == null)
		{
			return;
		}

		StartCoroutine(FadeInAsync(renderer, FadeOut));
	}

	IEnumerator FadeTo(Renderer renderer, float aValue, float aTime, Action<Renderer> callback)
	{
		float alpha = renderer.material.color.a;
		for (float t = 0.0f; t < _fadeTime; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
			renderer.material.color = newColor;
			yield return null;
		}

		if (callback != null)
		{
			callback (renderer);
		}
	}

	private IEnumerator FadeInAsync(Renderer renderer, Action<Renderer> callback)
	{
		return FadeTo(renderer, _fadeTime, 0.3f, callback);
	}

	private void FadeOut(Renderer renderer)
	{
		StartCoroutine(FadeOutAsync(renderer, RemoveGrid));
	}

	private IEnumerator FadeOutAsync(Renderer renderer, Action<Renderer> callback)
	{
		return FadeTo(renderer, 0.0f, 0.3f, callback);
	}

	private void Hide(Renderer renderer)
	{
		gameObject.SetActive(false);
	}

	private void RemoveGrid(Renderer renderer)
	{
		Destroy (transform.parent.gameObject);
	}
}
