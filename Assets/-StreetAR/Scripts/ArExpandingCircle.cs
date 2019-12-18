using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArExpandingCircle : MonoBehaviour {

	public static ArExpandingCircle Instance;

//	public Color _sturfeeTeal;
//	public Color _flagColor;

	private SpriteRenderer _spriteRenderer;

	void Awake()
	{
		Instance = this;
	}
		
	void Start () {
		_spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	public void Activate(Vector3 pos, Quaternion rot, bool flag = false)
	{
		transform.position = pos;
		transform.rotation = rot; // Quaternion.LookRotation(normal);

//		GetComponent<MapPlacementAnimator> ().EndAnimation ();

		if (!flag)
		{
			GetComponent<MapPlacementAnimator>().PlacementAnimation();
//			_spriteRenderer.color = _sturfeeTeal;
		}
		else
		{
			GetComponent<MapPlacementAnimator>().PlacementAnimation(true);
//			_spriteRenderer.color = _flagColor;
		}
	}


}
