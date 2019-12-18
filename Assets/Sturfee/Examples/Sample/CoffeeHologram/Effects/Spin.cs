using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
	float speed = 10f;
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;

	void Update ()
	{
		transform.Rotate(x * speed * Time.deltaTime, 0 , z * speed * Time.deltaTime);
		transform.Rotate(0,y *speed * Time.deltaTime,0,Space.World);
	}
}