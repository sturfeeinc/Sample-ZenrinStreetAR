using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlacementAnimator : MonoBehaviour {

    public float ScaleAmount = 0.007f;

	public Color SturfeeTeal;
	public Color FlagColor;


    private Vector3 _origScale;
	private Color _origColor;

	private Vector3 _scale;
	private Color _color;

	private SpriteRenderer _spriteRenderer;

	private bool _reset = false;
	private bool _active = false;
	private bool _endAnimation = false;

	void Awake () {
		_scale = transform.localScale;
		_spriteRenderer = GetComponent<SpriteRenderer> ();
		_color = _spriteRenderer.color;

		_origColor = _color;
		_origScale = _scale;


	}

//	public void OnDisable()
//	{
//		print ("!*!*!*! MAP PLACEMENT ANIMATOR DISABLED");
//		EndAnimation ();
//		Reset ();
//		_reset = true;
//		_active = false;
//	}

	public void PlacementAnimation(bool flagColor = false)
	{
//		print ("** PLACEMENT ANIMATION FUNCTION");
//		_endAnimation = false;

		if (flagColor)
		{
			_origColor = FlagColor;
		}
		else
		{
			_origColor = SturfeeTeal;
		}

//		Reset ();
//		StartCoroutine (Animate());

		if (!_active)
		{
//			print ("** NOT ACTIVE");

			_active = true;
			Reset ();
			StartCoroutine (Animate ());
		}
		else
		{
//			print ("** ACTIVE");

			_reset = true;
		}


	}

	public void EndAnimation()
	{
		if (_active)
		{
			_endAnimation = true;
		}
		else
		{
			_endAnimation = false;
			_active = false;
			_reset = false;


//			if(transform.parent != null)
//			{
				transform.parent.gameObject.SetActive (false);  // SPECIAL CASE FOR THE CURRENTMAPITEMSELECTOR
//			}
		}

	}

	private IEnumerator Animate()
	{
		if (/*!gameObject.activeSelf*/ _endAnimation)
		{
			_endAnimation = false;
			_active = false;
			_reset = false;

			transform.parent.gameObject.SetActive (false);  // SPECIAL CASE FOR THE CURRENTMAPITEMSELECTOR
		}
		else
		{
			yield return new WaitForFixedUpdate ();

			if (_reset)
			{
//				print ("** RESET");

				Reset ();

				_reset = false;
			}

			_scale += Vector3.one * ScaleAmount;//0.008 //  0.2f;
			_color.a -= 0.015f;
			transform.localScale = _scale;
			_spriteRenderer.color = _color;

//		if (_endAnimation)
//		{
//			Reset ();
//			_endAnimation = false;
//		}
//		else
//		{

			if (_color.a > 0)
			{
//				print ("** GO AGAIN");
				StartCoroutine (Animate ());
			}
			else
			{
//				print ("** END");
				_active = false;
			
			}


		}

//
//		if (_color.a > 0)
//		{
//			StartCoroutine (Animate ());
//
//			_scale += Vector3.one * 0.05f;
//			_color.a -= 0.01f;
//			transform.localScale = _scale;
//			_spriteRenderer.color = _color;
//
//			yield return new WaitForFixedUpdate();
//		}
//		else
//		{
//			_active = false;
//		}
	}

	private void Reset()
	{
//		_reset = true;
		_scale = _origScale;
		_color = _origColor;
		_spriteRenderer.color = _origColor;
		transform.localScale = _origScale;

	}
}
