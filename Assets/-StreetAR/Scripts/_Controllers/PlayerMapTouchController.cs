using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

// Controls movement of the full-screen-map-camera via player touch on the screen
public class PlayerMapTouchController : MonoBehaviour , IDragHandler, IBeginDragHandler, IPointerClickHandler{

	public static PlayerMapTouchController Instance;

	public float MapMoveSensitivity = 0.3f;

	[HideInInspector]
	public bool FlagPulsate = false;

	private int _camBoundsUp;
	private int _camBoundsDown;
	private int _camBoundsRight;
	private int _camBoundsLeft;

	// extent that the full-screen-map-camera sees vertically and horizontally
	private float _vertFullCamExtent;
	private float _horFullCamExtent;

	private Vector2 _prevTouchPos;

	private bool _flagMode = false;
	private Transform _currentFlag;
	private List<GameObject> _flagsList;
	private bool _newFlagPlaced = false; // could use _currentFlag instead???

	private void Awake()
	{
		Instance = this;
	}

	private void Start () {
        //ComputeSturfeeCamBounds();
        //ComputeMapboxCamBounds ();  // TODO: Reinstate this somewhere, probably

		_flagsList = new List<GameObject> ();
	}

	public void OnPointerClick(PointerEventData data)
	{
        if (FlagController.PlacementMode)
        {
            FlagController.Instance.FlagPlacementTouchPos(data.position);
        }
        else if(PathingManager.PlacementMode)
        {
            PathingManager.Instance.MarkerPlacementTouchPos(data.position);
        }
    }

	public void OnBeginDrag(PointerEventData data)
	{
		_prevTouchPos = data.position;

        if (FlagController.PlacementMode)
        {
            FlagController.Instance.FlagPlacementTouchPos(data.position);
        }
        else if (PathingManager.PlacementMode)
        {
            PathingManager.Instance.MarkerPlacementTouchPos(data.position);
        }
    }

	public void OnDrag(PointerEventData data)
	{
		if (!FlagController.PlacementMode && !PathingManager.PlacementMode)
		{
			Vector2 touchDifference = (_prevTouchPos - data.position) * MapMoveSensitivity;

			Vector3 camPos = MapManager.Instance.HalfMapCam.position;
			camPos.x += touchDifference.x;
			camPos.z += touchDifference.y;

			// Keep the camera in the boundaries of the map
			if (camPos.x > _camBoundsRight)
			{
				camPos.x = _camBoundsRight;
			}
			else if (camPos.x < _camBoundsLeft)
			{
				camPos.x = _camBoundsLeft;
			}
			else if (camPos.z > _camBoundsUp)
			{
				camPos.z = _camBoundsUp;
			}
			else if (camPos.z < _camBoundsDown)
			{
				camPos.z = _camBoundsDown;
			}

            MapManager.Instance.HalfMapCam.position = camPos;
			_prevTouchPos = data.position;
		}
		else
		{
            if (AppManager.Instance.AllowOldFlag)
            {
                FlagController.Instance.FlagPlacementTouchPos(data.position);
            }
            else if(AppManager.Instance.AllowPathing)
            {
                PathingManager.Instance.MarkerPlacementTouchPos(data.position);
            }
            else
            {
                Debug.Log("Error: No placeable map object allowed");
            }

        }
	}

    // TODO: This was changed from private to public, figure out how to structure this all
	public void ComputeMapboxCamBounds()
	{
        _vertFullCamExtent = MapManager.Instance.HalfMapCam.GetComponent<Camera>().orthographicSize;
            //(_fullScreenMapCam.GetComponent<Camera> ().orthographicSize);
		_horFullCamExtent = _vertFullCamExtent * Screen.width / Screen.height;

		Vector3 mapCenterCoord = MapManager.Instance.GetMapCenterUnityCoord ();
		float distFromMapCenterToEdge = MapManager.Instance.GetDistanceFromMapCenterToEdge ();

		_camBoundsUp = (int)(mapCenterCoord.z + distFromMapCenterToEdge - _vertFullCamExtent);
		_camBoundsDown = (int)(mapCenterCoord.z - distFromMapCenterToEdge + _vertFullCamExtent);
		_camBoundsRight = (int)(mapCenterCoord.x + distFromMapCenterToEdge - _horFullCamExtent);
		_camBoundsLeft = (int)(mapCenterCoord.x - distFromMapCenterToEdge + _horFullCamExtent);
	}

    //private void ComputeSturfeeCamBounds()
    //{
    //    int xOffset = 5;
    //    int zOffset = 90;

    //    List<GameObject> TerrainObjects = AppManager.Instance.TerrainObjects;
    //    //List<GameObject> TerrainObjects = GameManager.Instance.TerrainObjects;

    //    Vector3 corner1 = TerrainObjects[0].transform.position;
    //    Vector3 corner2 = TerrainObjects[TerrainObjects.Count - 1].transform.position;

    //    _camBoundsUp = (int)(corner2.z + zOffset);
    //    _camBoundsDown = (int)(corner1.z - zOffset);
    //    _camBoundsRight = (int)(corner2.x - xOffset);
    //    _camBoundsLeft = (int)(corner1.x + xOffset);

    //    print("UP: " + _camBoundsUp + ", DOWN: " + _camBoundsDown + ", RIGHT: " + _camBoundsRight + ", LEFT: " + _camBoundsLeft);

    //    /*
    //    int xOffset = 5;
    //    int zOffset = 90;

    //    Transform sturfeeMapData = GameManager.Instance.GetSturfeeMapData;

    //    //TODO: Dependent on the 5x5 Sturfee SDK 
    //    // corner 1 = bottom left sturfee tile, corner 2 = top right tile
    //    Vector3 corner1 = sturfeeMapData.GetChild(25).position;
    //    Vector3 corner2 = sturfeeMapData.GetChild(49).position;

    //    _camBoundsUp = (int) (corner2.z + zOffset);
    //    _camBoundsDown = (int) (corner1.z - zOffset);
    //    _camBoundsRight = (int) (corner2.x - xOffset);
    //    _camBoundsLeft = (int) (corner1.x + xOffset);

    //    print("UP: " + _camBoundsUp + ", DOWN: " + _camBoundsDown + ", RIGHT: " + _camBoundsRight + ", LEFT: " + _camBoundsLeft);
    //    */
    //}

    

}
