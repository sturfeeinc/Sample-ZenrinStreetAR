using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Constants;
using UnityEngine.UI;

public class AutoPoiController : MonoBehaviour {

    [Header("Vision Variables")]
    public int VisionGridWidth = 7;
    public int VisionGridHeight = 7;

    [Header("Components")]
    public RectTransform VisionGridBorder;
    public Text PoiText;
    public RectTransform PoiCastPointVisual;
    public Canvas RefCanvas;

    [Header("Debugging")]
    public Material BuildingHideMat;
    public Material BuildingDebugMat;
    public Transform RaycastHitPoint;

    private GameObject _prevBuilding;   // DEBUGGING OBJECT

    private Vector2[,] _visionGridPoints;
    private Vector2 _gridCenterPoint;

    // Data pertaining to the current building that is most prominently in view
    private GameObject _curPoiBuilding;
    private Vector3 _curPoiWorldPoint;
    private Vector2 _curPoiScreenPoint;

    private bool _gettingFoursquareData = false;    // TODO: Change to "MakingServerCall" or something

    private LayerMask _layerMask;
    private Camera _xrCam;

    private bool _usePresetPoi; // appmanager placement
    private float _raycastDistance;
    private int _targetLayer;

    private void Awake()
    {
        PoiCastPointVisual.gameObject.SetActive(false);
        VisionGridBorder.gameObject.SetActive(false);
    }

    void Start () {

        _xrCam = PlayerController.Instance.GetComponent<Camera>();

        VisionGridBorder.gameObject.SetActive(true);

        CalculateScreenRaycastPoints();

        _usePresetPoi = AppManager.Instance.AllowPresetPOI;

        InitializeDependentVariables();

        //_layerMask |= (1 << LayerMask.NameToLayer(SturfeeLayers.Terrain));
        //if (_usePresetPoi)
        //{
        //    _layerMask |= (1 << LayerMask.NameToLayer("PresetPOI"));

        //}
        //else
        //{
        //    _layerMask |= (1 << LayerMask.NameToLayer(SturfeeLayers.Building));
        //}
    }
	
	void Update () {

        if (!_gettingFoursquareData)
        {
            if(FindProminentBuildingInView())
            {
                //if (_usePresetPoi)
                //{
                //    DisplayClosestPoi();
                //}
                //else
                //{
                    DeterminePoiMethod();
                //}
            }
            else    // Did not find building in view
            {
                PoiCastPointVisual.gameObject.SetActive(false);
                PoiText.text = "";
            }

            // Uncomment when Debugging
            //if (_prevBuilding != _curPoiBuilding && _prevBuilding != null)
            //{
            //    _prevBuilding.GetComponent<Renderer>().material = BuildingHideMat;
            //}

            //if (_curPoiBuilding != null)
            //{
            //    _curPoiBuilding.GetComponent<Renderer>().material = BuildingDebugMat;

            //    _prevBuilding = _curPoiBuilding;
            //}
        }
        
    }

    private void InitializeDependentVariables()
    {
        if (_usePresetPoi)
        {
            _targetLayer = LayerMask.NameToLayer("PresetPOI");
            _raycastDistance = 50;
        }
        else
        {
            _targetLayer = LayerMask.NameToLayer(SturfeeLayers.Building);
            _raycastDistance = 700;
        }
        _layerMask |= (1 << _targetLayer);
    }

    #region DYNAMIC_POI
    private void CalculateScreenRaycastPoints()
    {
        _visionGridPoints = new Vector2[VisionGridWidth, VisionGridHeight];
        _gridCenterPoint = new Vector2((_visionGridPoints.GetLength(0) - 1) / 2f, (_visionGridPoints.GetLength(1) - 1) / 2f);

        float horDistBtwnPoints = (VisionGridBorder.rect.xMax - VisionGridBorder.rect.xMin) / (VisionGridWidth - 1);
        float verDistBtwnPoints = (VisionGridBorder.rect.yMax - VisionGridBorder.rect.yMin) / (VisionGridHeight - 1);

        for (int i = 0; i < _visionGridPoints.GetLength(0); i++)
        {
            for (int j = 0; j < _visionGridPoints.GetLength(1); j++)
            {
                Vector2 basePoint = new Vector2(VisionGridBorder.rect.xMin + horDistBtwnPoints * i, VisionGridBorder.rect.yMin + verDistBtwnPoints * j);

                Vector2 newPoint = basePoint * RefCanvas.scaleFactor;
                newPoint.x += RefCanvas.worldCamera.pixelWidth / 2f;
                newPoint.y += RefCanvas.worldCamera.pixelHeight / 2f;

                _visionGridPoints[i, j] = newPoint;
            }
        }
    }

    private bool FindProminentBuildingInView()
    {
        _curPoiBuilding = null;

        // The following lists will all be equal in length. Their indices refer to data of the same building in view.
        List<GameObject> buildings = new List<GameObject>();
        List<int> numBuildingOcc = new List<int>();
        List<Vector2> mostCenteredPoint = new List<Vector2>();

        for (int i = 0; i < _visionGridPoints.GetLength(0); i++)
        {
            for (int j = 0; j < _visionGridPoints.GetLength(1); j++)
            {
                Ray ray = _xrCam.ScreenPointToRay(_visionGridPoints[i, j]);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _raycastDistance, _layerMask))
                {
                    if(hit.collider.gameObject.layer == _targetLayer)
                    {
                        GameObject building = hit.collider.gameObject;
                        int buildingIndex = buildings.IndexOf(building);

                        Vector2 gridPoint = new Vector2(i, j);

                        if (buildingIndex < 0)
                        {
                            // Add the building ID
                            buildings.Add(building);
                            numBuildingOcc.Add(1);
                            mostCenteredPoint.Add(gridPoint);
                        }
                        else
                        {
                            numBuildingOcc[buildingIndex]++;    // Increase amount of times this ID has occurred

                            // Track the closest grid point of this building to the center of the screen
                            if (Vector2.Distance(gridPoint, _gridCenterPoint) < Vector2.Distance(mostCenteredPoint[buildingIndex], _gridCenterPoint))
                            {
                                mostCenteredPoint[buildingIndex] = gridPoint;
                            }
                        }
                    }
                }
            }
        }

        if (buildings.Count > 0)
        {
            int mostOcc = numBuildingOcc[0];
            int indexOfMostOcc = 0;
            for (int i = 1; i < numBuildingOcc.Count; i++)
            {
                if (numBuildingOcc[i] > mostOcc)
                {
                    mostOcc = numBuildingOcc[i];
                    indexOfMostOcc = i;
                }
            }

            _curPoiBuilding = buildings[indexOfMostOcc];
            _curPoiScreenPoint = _visionGridPoints[(int)mostCenteredPoint[indexOfMostOcc].x, (int)mostCenteredPoint[indexOfMostOcc].y];

            Ray ray = _xrCam.ScreenPointToRay(_curPoiScreenPoint);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _raycastDistance, _layerMask))
            {
                _curPoiWorldPoint = hit.point;
            }
            else
            {
                Debug.LogError("Chosen centered raycast point did not hit building");
            }

            return true;
        }
        else
        {
            //print("No buildings found"); 
            _curPoiBuilding = null;
            return false;
        }
    }

    // Determine whether a Foursquare API call needs to be made, or if cached data exists that can be used
    private void DeterminePoiMethod()
    {
        if (_curPoiBuilding != null)
        {
            if (_curPoiBuilding.GetComponent<PoiDataProvider>() == null)
            {
                // Call Foursquare API to get POI data

                _gettingFoursquareData = true;

                //print("Building Size: " + _curPoiBuilding.GetComponent<Renderer>().bounds.size);
                //print("Building Extents X: " + _curPoiBuilding.GetComponent<Renderer>().bounds.extents.x);
                //print("Building Extents Y: " + _curPoiBuilding.GetComponent<Renderer>().bounds.extents.y);
                //print("Building Extents Z: " + _curPoiBuilding.GetComponent<Renderer>().bounds.extents.z);

                GetComponent<FoursquareService>().GetVenueData(_curPoiWorldPoint, OnVenueDataReceived, true);
            }
            else    
            {
                // Primary building already has data to access
                DisplayClosestPoi(_curPoiBuilding.GetComponent<PoiDataProvider>(), _curPoiWorldPoint);
            }
        }
    }

    private void DisplayClosestPoi(PoiDataProvider poiData, Vector3 raycastPoint)
    {
        int closestVenueIndex = 0;
        float shortestDist = Vector3.Distance(raycastPoint, poiData.Venues[0].unityCoord);
        for (int i = 1; i < poiData.Venues.Count; i++)
        {
            float venueDist = Vector3.Distance(raycastPoint, poiData.Venues[i].unityCoord);
            if (venueDist < shortestDist)
            {
                closestVenueIndex = i;
                shortestDist = venueDist;
            }
        }

        PoiText.text = poiData.Venues[closestVenueIndex].name;
        PoiCastPointVisual.position = _curPoiScreenPoint;
        PoiCastPointVisual.gameObject.SetActive(true);
    }

    // Callback from Foursquare call
    private void OnVenueDataReceived(List<FoursquareAPI.Venue> venues)
    {
        if (venues != null)
        {
            PoiDataProvider poiData = _curPoiBuilding.AddComponent<PoiDataProvider>();
            poiData.Venues = venues;
            //poiData.RaycastedPoint = _curPoiWorldPoint;

            //CreateVectorLocationData(poiData);
            DisplayClosestPoi(poiData, _curPoiWorldPoint);

            _gettingFoursquareData = false;

            // Debugging
            //Instantiate(RaycastHitPoint, _curPoiWorldPoint, Quaternion.identity);
        }

    }
    #endregion

    #region PRESET_POI


    #endregion

    //private void CreateVectorLocationData(PoiDataProvider poiData)
    //{
    //    for(int i = 0; i < poiData.Venues.Count; i++)
    //    {
    //        var gpsPos = new Sturfee.Unity.XR.Core.Models.Location.GpsPosition();
    //        gpsPos.Latitude = poiData.Venues[i].location.lat;
    //        gpsPos.Longitude = poiData.Venues[i].location.lng;
    //        gpsPos.Height = 0;
    //        poiData.Venues[i].unityCoord = XRSessionManager.GetSession().GpsToLocalPosition(gpsPos);
    //    }
    //}


    // OLD IDEA
    // TODO: Change this to have variables? So that you only use this call for all raycasting in this script
    //private void CenterScreenRaycast()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.forward, out hit, 1000, _layerMask))
    //    {
    //        // Check if this building has data already
    //        //  apply that data
    //        // if not, Call foursquare service
    //        //  Save this data to the building

    //        if(hit.collider.GetComponent<PoiDataProvider>() != null)
    //        {
    //            // TODO: Do computations based on the existing data
    //        }
    //        else
    //        {
    //            _numRaycasts++;
    //            // TODO: Do a foursquare call and save the data to the building object
    //                // Maybe don't even apply the data...Just wait for next frame's raycast to hit it and then apply

    //            //_numCompletedRaycasts++
    //        }

    //    }
    //    else
    //    {
    //        _numEmptyRaycasts++;
    //    }
    //}
}
