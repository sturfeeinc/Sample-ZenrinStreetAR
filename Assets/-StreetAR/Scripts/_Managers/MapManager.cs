using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Sturfee.Unity.XR.Core.Session;
using System.Collections;

// Used to initialize the map, and set boundaries for the full screen map camera
public class MapManager : MonoBehaviour {

	public static MapManager Instance;

	public AbstractMap Map;
    public Transform HalfMapCam;
    public Transform MiniMapCam;

    // TODO: Change these to Vector3 and adjust name too
    private int _halfMapCamHeight = 430;
    private int _miniMapCamHeight = 330;
	private int _numTilesFromCenter;	// # of tiles from center map tile to an edge tile. Assumes there is an equal amount of map tiles in every direction.

    private bool _initialized = false;
    private Vector3 _playerPos;

	void Awake()
	{
		Instance = this;
	}

	void Start () 
    {
        _numTilesFromCenter = Map.Options.extentOptions.defaultExtents.rangeAroundCenterOptions.north;//  .rangeAroundCenterOptions.north;
        //_numTilesFromCenter = Map.Options.extentOptions.rangeAroundCenterOptions.north;

        HalfMapCam.gameObject.SetActive(false);
        MiniMapCam.gameObject.SetActive(false);
	}

    private void Update()
    {
        if (_initialized)
        {
            _playerPos = XRSessionManager.GetSession().GetXRCameraPosition();
            //if (!HalfMapCam.gameObject.activeSelf)
            //{
                MiniMapCam.position = _playerPos + (Vector3.up * _miniMapCamHeight);
            //}
            //else
            //{
            //    HalfMapCam.position = _playerPos + (Vector3.up * _halfMapCamHeight);
            //}
        }
    }

    public void InitializeMap()
	{
        //XRSessionManager.GetSession().ForceLocationUpdate();

        Vector3 mapPos = Map.transform.position;
        mapPos.y = XRSessionManager.GetSession().GetXRCameraPosition().y;
        Map.transform.position = mapPos;

        var gpsPos = XRSessionManager.GetSession().LocalPositionToGps(mapPos);
        Vector2d mapboxCoord = new Vector2d(gpsPos.Latitude, gpsPos.Longitude);
        Map.Initialize(mapboxCoord, 15);
        Map.transform.position += Vector3.down * 0.3f;


        //XRSessionManager.GetSession().ForceLocationUpdate();

        //Vector3 playerPos = XRSessionManager.GetSession().GetXRCameraPosition();
        //Map.transform.position = playerPos + (Vector3.down * 0.1f);

        //var gpsPos = XRSessionManager.GetSession ().GpsProvider.GetGPSPosition ();

        //Vector2d gpsLatLongPos = new Vector2d (gpsPos.Latitude, gpsPos.Longitude);
        //Map.Initialize (gpsLatLongPos, 15);


        MiniMapCam.gameObject.SetActive(true);
        _initialized = true;
    }

    //private IEnumerator WaitForMapComputation()
    //{
    //    yield return null;
    //}

    public void SwapCams()
    {
        // turn on/off mini and half map cams
        // when half map cam turns on, center it over player correctly

        bool val = MiniMapCam.gameObject.activeSelf;
        MiniMapCam.gameObject.SetActive(!val);
        HalfMapCam.gameObject.SetActive(val);

        if(HalfMapCam.gameObject.activeSelf)
        {
            HalfMapCam.position = _playerPos + (Vector3.up * _halfMapCamHeight);
        }
    }

    // TODO: Probably need to remove these
    public Vector3 GetMapCenterUnityCoord()
	{
		return Map.transform.GetChild (0).position;
	}

	public float GetDistanceFromMapCenterToEdge()
	{
		// Child 0 is the center map square, Child 1 is the top left map square
		float mapSquareSize = Mathf.Abs ((Map.transform.GetChild (0).position.x - Map.transform.GetChild (1).position.x) / _numTilesFromCenter);
		return mapSquareSize * (_numTilesFromCenter + 0.5f);
	}
}
