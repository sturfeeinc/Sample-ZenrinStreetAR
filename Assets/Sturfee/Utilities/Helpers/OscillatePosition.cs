using UnityEngine;

public class OscillatePosition : MonoBehaviour 
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public Direction MoveDirection;
    public float Speed = 100;
    public float Distance = 30;

    private float _max;
    private float _min;
     

    private void Start()
    {
        _max = (MoveDirection == (Direction.Horizontal) ? transform.position.x : transform.position.y ) + Distance;
        _min = (MoveDirection == (Direction.Horizontal) ? transform.position.x : transform.position.y) - Distance;
                      
    }

    private void Update () 
    {  
        float newPos = Mathf.PingPong(Time.time * Speed, _max - _min) + _min;

        Vector3 updatedPos = transform.position;
        updatedPos.x = (MoveDirection == Direction.Horizontal) ? newPos : updatedPos.x;
        updatedPos.y = (MoveDirection == Direction.Vertical)   ? newPos : updatedPos.y;

        transform.position = updatedPos;
	}

}

