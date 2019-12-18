using UnityEngine;

public class OscillateRotation : MonoBehaviour 
{

    public float Speed = 100;
    public float Angle = 30;

    private float _max;
    private float _min;
     

    private void Start()
    {       
        _max = transform.rotation.eulerAngles.z + Angle;
        _min = transform.rotation.eulerAngles.z - Angle;
    }

    private void Update () 
    {  
        float newZ = Mathf.PingPong(Time.time * Speed, _max - _min) + _min;

        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = newZ;

        transform.rotation = Quaternion.Euler(rot);
	}

}

