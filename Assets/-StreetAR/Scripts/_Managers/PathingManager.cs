using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Package.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class PathingManager : MonoBehaviour {

    public static PathingManager Instance;

    public static bool PlacementMode;

    [Header("Prefabs")]
    public Transform DestinationMarkerPrefab;
    public Transform PathArrowPrefab;

    [Header("Line Renderer")]
    public LineRenderer VisualMapPath;

    [HideInInspector]
    public Transform DestinationMarker;
    [HideInInspector]
    public bool ActivePath;     // TODO: Dual meaning after prelocalize path?


    private Transform _curWorldPathArrow;

    private float _relocDist = 25;
    private Vector3 _lastArrowPos;
    private Vector3 _lastArrowDir;
    private float _distSinceLastReloc = 0;
    private float _lewayDist = 5;

    private int _pathPointIndex;    // index of corner in path being headed to
                                        
    private LayerMask _sturfeeTerrainLayerMask;
    private LayerMask _sturfeeCollidersLayerMask;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        PlacementMode = false;

        _sturfeeTerrainLayerMask |= (1 << LayerMask.NameToLayer("sturfeeTerrain"));
        _sturfeeCollidersLayerMask |= (1 << LayerMask.NameToLayer("sturfeeTerrain"));
        _sturfeeCollidersLayerMask |= (1 << LayerMask.NameToLayer("sturfeeBuilding"));
    }

    //private void Update()
    //{
    //    if(ActivePath)
    //    {
    //        if(Vector3.Distance())
    //    }
    //}

    public void PrepareDestinationMarker()
    {
        print("in prepare destination marker");
        DestinationMarker = Instantiate(DestinationMarkerPrefab, Vector3.down * 10000, Quaternion.identity);
        PlacementMode = true;
    }

    public void MarkerPlacementTouchPos(Vector2 touchPos)
    {
        Ray ray = MapManager.Instance.HalfMapCam.GetComponent<Camera>().ScreenPointToRay(touchPos);
        MarkerPlacementRaycast(ray);
    }

    public void CreatePath()
    {
        // Hastily put together for pre-localization changes
        if (ActivePath)
        {
            UiManager.Instance.NextArrowButton.SetActive(true);
            PlacementMode = false;
            //ActivePath = true;
            StartCoroutine(GetMapboxPath());
        }
    }

    public void DeletePath()
    {
        if (DestinationMarker != null)
        {
            Destroy(DestinationMarker.gameObject);
        }
        if(_curWorldPathArrow != null)
        {
            Destroy(_curWorldPathArrow.gameObject);
        }
        _distSinceLastReloc = 0;
        _pathPointIndex = 0;
        VisualMapPath.positionCount = 0;
        PlacementMode = false;
        ActivePath = false;
    }

    private void MarkerPlacementRaycast(Ray ray)
    {
        // First raycast from the perspective camera
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, _sturfeeTerrainLayerMask))
        {
            // TODO: Place the marker 

            Vector3 secondRayOrigin = hit.point + Vector3.up * 1000;
            RaycastHit hit2;

            // 2nd raycast from directly above hit point to determine if the space is open
            if(Physics.Raycast(secondRayOrigin, Vector3.down, out hit2, 1100, _sturfeeCollidersLayerMask))
            {
                if(hit2.transform.gameObject.layer == LayerMask.NameToLayer("sturfeeTerrain"))
                {
                    // TODO: everything is fine (turn on necessary things on if not)
                    // TEMP
                    DestinationMarker.position = hit.point;
                    UiManager.Instance.FlagSetButtonInteractability(true);
                }
                else
                {
                    // TODO: Red X map rep, uninteractable set button, dont make the AR marker visible

                }
            }
        }
    }

    public void SetNextArrow()
    {
        Vector3 nextButOneCornerPos;  // The corner after the upcoming corner

        if(_pathPointIndex > 0)
        {
            nextButOneCornerPos = VisualMapPath.GetPosition(_pathPointIndex - 1);
        }
        else if(_pathPointIndex == 0 /*&& Vector3.Distance(DestinationMarker.position, VisualMapPath.GetPosition(0)) > _lewayDist*/)
        {
            nextButOneCornerPos = DestinationMarker.position;

            float distFromPathEndToDest = Vector3.Distance(DestinationMarker.position, VisualMapPath.GetPosition(0));
            float distToDest = distFromPathEndToDest + Vector3.Distance(_lastArrowPos, VisualMapPath.GetPosition(0));

            if( distToDest < 11 && distFromPathEndToDest < _lewayDist)
            {
                return;
            }
        }
        else
        {
            // Done. No more arrows
            return;
        }

        Vector3 nextCornerPos = VisualMapPath.GetPosition(_pathPointIndex);
        float distToCorner = Vector3.Distance(nextCornerPos, _lastArrowPos);

        print("** Distance to corner: " + distToCorner);


        // TODO: NEW
        if(distToCorner < 15)
        {
            // standard corner placement
            // Possibly put a (standard?) angle check?

            Vector3 nextCornerDir = nextButOneCornerPos - nextCornerPos;
            float angleBetweenArrows = Vector3.Angle(_lastArrowDir, nextCornerDir);

            print("** Angle Between Arrows: " + angleBetweenArrows);

            if (angleBetweenArrows > 38)
            {
                print("** Placing Standard Corner Arrow");

                _curWorldPathArrow = Instantiate(PathArrowPrefab, nextCornerPos, Quaternion.identity);
                _curWorldPathArrow.GetComponent<WorldPathArrow>().Initialize(nextButOneCornerPos, false);

                _pathPointIndex--;
                _lastArrowPos = nextCornerPos;


                _lastArrowDir = nextButOneCornerPos - nextCornerPos;

            }
            else
            {
                if(distToCorner > 5)
                {
                    print("** Replace corner arrow with flat arrow");

                    _curWorldPathArrow = Instantiate(PathArrowPrefab, nextCornerPos, Quaternion.identity);
                    _curWorldPathArrow.GetComponent<WorldPathArrow>().Initialize(nextButOneCornerPos, true);

                    _pathPointIndex--;
                    _lastArrowPos = nextCornerPos;
                    _lastArrowDir = nextButOneCornerPos - nextCornerPos;
                }
                else
                {
                    print("** Skipping Arrow. Distance to Corner < 5");

                    // Skip to next arrow
                    _pathPointIndex--;
                    SetNextArrow();
                }
            }
        }
        else
        {
            // Next corner arrow is over 15 meters away. So we place a forward arrow in between, 10 meters from last arrow point.

            print("** Standard Forward Arrow");

            Vector3 dir = nextCornerPos - _lastArrowPos;
            dir.Normalize();

            Vector3 arrowPos = _lastArrowPos + (dir * 10);

            _curWorldPathArrow = Instantiate(PathArrowPrefab, arrowPos, Quaternion.identity);
            _curWorldPathArrow.GetComponent<WorldPathArrow>().Initialize(nextCornerPos, true);
            _lastArrowPos = arrowPos;
            _lastArrowDir = nextCornerPos - arrowPos;

        }
    }

    public void SkipToNextArrow()
    {
        if (_curWorldPathArrow != null)
        {
            Destroy(_curWorldPathArrow.gameObject);
        }
        SetNextArrow();
    }

    private void AttachPathToTerrain()
    {
        for (int i = 0/*1*/; i < VisualMapPath.positionCount /*- 1*/; i++)
        {
            //Debug.DrawRay(VisualPath.GetPosition(i) + Vector3.up * 200, Vector3.down, Color.green, 100);

            RaycastHit hit;
            if (Physics.Raycast(VisualMapPath.GetPosition(i) + Vector3.up * 200, Vector3.down, out hit, 1000, /*_raycastMask*/ _sturfeeTerrainLayerMask))
            {
                Vector3 pos = VisualMapPath.GetPosition(i);
                pos.y = hit.point.y;

                VisualMapPath.SetPosition(i, pos);
            }
            else
            {
                ToastManager.Instance.ShowToastTimed("Error setting Mapbox path to sturfee terrain", 5);
            }
        }
    }

    private IEnumerator GetMapboxPath()
    {
        var fromGps = XRSessionManager.GetSession().LocalPositionToGps(PlayerController.Instance.transform.position);
        var toGps = XRSessionManager.GetSession().LocalPositionToGps(DestinationMarker.position);

        print("START GPS POS: " + fromGps.Longitude + ", " + fromGps.Latitude);
        print("END GPS POS: " + toGps.Longitude + ", " + toGps.Latitude);

        //string url = "https://api.mapbox.com/directions/v5/mapbox/cycling/" + gps.Latitude + "," + gps.Longitude

        //print("!! Getting Mapbox path");

        string url = "https://api.mapbox.com/directions/v5/mapbox/walking/" +
            fromGps.Longitude.ToString() + "," + fromGps.Latitude.ToString() + ";" + toGps.Longitude.ToString() + "," + toGps.Latitude.ToString() +
                   "?steps=true&geometries=geojson&overview=full&access_token=pk.eyJ1IjoibmV3bWFudyIsImEiOiJjamxvZWoydm0wNHllM3BxaDh6c3h0d2F0In0.d1oJAx_nKtHSX32lyv9mBg";

        //alternatives=true&

        //print("URL: " + url);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                //print("!! Error getting Mapbox pathing data");
                Debug.Log(www.error);

            }
            else
            {
                //print("!! Got Mapbox pathing data");
                //var d = JsonUtility.FromJson<MapboxAPI.Response>(www.downloadHandler.text);

                print("JSON STRING RESPONSE: " + www.downloadHandler.text);

                var b = JSON.Parse(www.downloadHandler.text);

                //print("Simple JSON string: " + b.ToString());

                //var test = b["routes"][0]["geometry"]["coordinates"].Count;//[0][0].AsFloat;
                //string code = b["code"].Value;  // works
                //print("SIMPLE JSON Value:" + code + ", " + test);

                //int pathPoints = //d.routes[routeNum].legs[0].steps.Length /*+ 1*/ /*+ finalPoint*/;
                int pathPoints = b["routes"][0]["geometry"]["coordinates"].Count;


                VisualMapPath.positionCount = pathPoints;

                print("*** PATH POINTS " + pathPoints);

                int count = 1;
                for (int i = 0; i < pathPoints; i++)
                {
                    //for(int j = 0; j < 2; j++)
                    //{
                    var wpGps = new Sturfee.Unity.XR.Core.Models.Location.GpsPosition();
                    wpGps.Latitude = b["routes"][0]["geometry"]["coordinates"][i][1].AsFloat; //d.routes[routeNum].legs[i].steps[j].maneuver.location[1];
                    wpGps.Longitude = b["routes"][0]["geometry"]["coordinates"][i][0].AsFloat;

                    Vector3 worldPos = XRSessionManager.GetSession().GpsToLocalPosition(wpGps);

                    VisualMapPath.SetPosition(pathPoints - count, worldPos);

                    count++;
                }

                _pathPointIndex = pathPoints - 1;

                AttachPathToTerrain();

                _lastArrowPos = PlayerController.Instance.transform.position;
                //_lastArrowDir = new Vector3(-90, 0, 90);    // Set angle looking up so it will definitely be different than initial arrow angle


                // OFFSET SETUP
                _lastArrowDir = VisualMapPath.GetPosition(_pathPointIndex - 1) - VisualMapPath.GetPosition(_pathPointIndex);
                _pathPointIndex--;
                SetNextArrow();


                // PRE OFFSET SETUP
                //float dist = Vector3.Distance(_lastArrowPos, VisualMapPath.GetPosition(_pathPointIndex));

                //if(dist > 8)
                //{
                //    // create an arrow pointing to the first position

                //    Vector3 dir = (VisualMapPath.GetPosition(_pathPointIndex) - _lastArrowPos);
                //    dir.Normalize();
                //    Vector3 forwardPos = _lastArrowPos + (dir * 4);
                //    _curWorldPathArrow = Instantiate(PathArrowPrefab, forwardPos /*arrowPos*/, Quaternion.identity);
                //    _curWorldPathArrow.GetComponent<WorldPathArrow>().Initialize(
                //        VisualMapPath.GetPosition(_pathPointIndex), true/*, false*/);

                //}
                //else
                //{
                //    SetNextArrow();
                //}






                //if (dist <= 6)
                //{
                //    // Place forward along path of first arrow, INSTEAD of first arrow (Replace)
                //    _lastArrowPos = VisualMapPath.GetPosition(_pathPointIndex);
                //    Vector3 dir = (VisualMapPath.GetPosition(_pathPointIndex - 1) - _lastArrowPos);//.normalized;
                //    dir.Normalize();
                //    Vector3 forwardPos = _lastArrowPos + (dir * 5);

                //    _curWorldPathArrow = Instantiate(PathArrowPrefab, forwardPos /*arrowPos*/, Quaternion.identity);
                //    _curWorldPathArrow.GetComponent<WorldPathArrow>().Initialize(
                //        VisualMapPath.GetPosition(_pathPointIndex - 1), true/*, false*/);
                //}
                //else if (dist > 12)
                //{
                //    // Place forward arrow pointing to first point on the path
                //    Vector3 dir = (VisualMapPath.GetPosition(_pathPointIndex) - _lastArrowPos);//.normalized;
                //    dir.Normalize();
                //    Vector3 forwardPos = _lastArrowPos + (dir * 5);

                //    _curWorldPathArrow = Instantiate(PathArrowPrefab, forwardPos /*arrowPos*/, Quaternion.identity);
                //    _curWorldPathArrow.GetComponent<WorldPathArrow>().Initialize(
                //        VisualMapPath.GetPosition(_pathPointIndex), true/*, false*/);
                //}
                //else
                //{
                //    SetNextArrow();
                //}
            }
        }
    }
}