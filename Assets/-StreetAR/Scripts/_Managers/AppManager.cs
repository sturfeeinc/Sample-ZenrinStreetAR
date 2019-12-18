using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Package.Utilities;

// TODO: Change to SetupManager - handles initial components of setting up game until localization completes
public class AppManager : MonoBehaviour
{
    public static AppManager Instance;

    public bool MapBuildingsOnSessionReady;
    public bool Debug;
    [Header("Features")]
    public bool AllowRadiusRelocalize;  // Relocalizes when player moves X amount from last localization point and taps on building
    public bool AllowPhotoBillboard;
    // TODO: Possibly create a single variable with 3 states (none, old flag, pathing) or remove old flag entirely
    [Tooltip("Should NOT be on when AllowPathing is on")]
    public bool AllowOldFlag;           // Flag that can be placed on terrain/buildings via map/AR and pulses
    [Tooltip("Should NOT be on when AllowOldFlag is on")]
    public bool AllowPathing;
    public bool AllowAutoPOI;
    public bool AllowPresetPOI;

    [Header("Other")]
    //[SerializeField]
    //private GameObject _updateGpsButton;
    [SerializeField]
    private GameObject _mapTouchPanel;  // TODO: Change this to UiManager...probably
    [SerializeField]
    private Material _buildingMaterial;

    //[HideInInspector]
    public static bool SessionReady;
    //[HideInInspector]
    public static bool Localized; // = false;

    //[HideInInspector]
    private List<GameObject> BuildingObjects;
    //[HideInInspector]
    //public List<GameObject> TerrainObjects;


    private void Awake()
    {
        Instance = this;

        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        //SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        //SturfeeEventManager.Instance.OnRequestAddedForScan += OnRequestAddedForScan;
    }

    private void OnDestroy()
    {
        SessionReady = false;
        Localized = false;

        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        //SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        //SturfeeEventManager.Instance.OnRequestAddedForScan -= OnRequestAddedForScan;

    }

    private void Start()
    {
        print("!!! APP MANAGER IS LOCALIZED: " + Localized);

        // Need these to be initialized for access and then turned off
        PlayerMapTouchController.Instance.gameObject.SetActive(false);
        PlayerArTouchController.Instance.gameObject.SetActive(false);
    }

    //private void OnRequestAddedForScan(int requestNum, int requestLength)
    //{
    //    if(requestNum == 1)
    //    {
    //        XRSessionManager.GetSession().ForceLocationUpdate();
    //        print("***Updated GPS on Scan Click");
    //    }

    //    //print("***********ORAFS: " + requestNum + ", " + requestLength);
    //}

    //public void OnScanClick()
    //{
    //    XRSessionManager.GetSession().ForceLocationUpdate();
    //}

    private void OnSessionReady()
    {
        //UiManager.Instance.UpdateGpsButton.SetActive(true);
        //XRSessionManager.GetSession().ForceLocationUpdate();

        //StartCoroutine(WaitAFrame());

        StartCoroutine(PostSessionReady());
    }

    private IEnumerator PostSessionReady()
    {
        //yield return null;
        //XRSessionManager.GetSession().ForceLocationUpdate();
        yield return null;

        SessionReady = true;

        MapManager.Instance.InitializeMap();

        if (AllowPathing || MapBuildingsOnSessionReady)
        {
            CreateMapBuildings();
        }

        yield return null;
        // TODO: use mapbox events to set this
        PlayerMapTouchController.Instance.ComputeMapboxCamBounds();
        UiManager.Instance.SetSessionReadyUi();

    }

    //private void OnLocalizationLoading()
    //{
    //    UiManager.Instance.UpdateGpsButton.SetActive(false);
    //}

    private void OnLocalizationSuccessful()
    {
        /*
        if (Localized)
        {
            if (AllowRadiusRelocalize)
            {
                PlayerArTouchController.Instance.RaycastAfterLocalization();
            }
            else if(AllowPathing)
            {
                // TODO: Atleast move this over to player controller? If not all of relocalization into their own scripts
                PathingManager.Instance.SetNextArrow();
            }
        }
        else*/ 

        //if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.Done)
        //{
            StartGame();
        //}
        //else if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.ScoreError)
        //{
        //    if (Debug)
        //    {
        //        StartGame();
        //    }
        //    else
        //    {
        //        ToastManager.Instance.ShowToastTimed("Insufficient Features for Localization");
        //    }
        //}
    }

    private void StartGame()
    {
        Localized = true;

        if (!AllowPathing)
        {
            CreateMapBuildings();
        }

        //PlayerMapTouchController.Instance.ComputeMapboxCamBounds();


        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        //SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;

        //SturfeeEventManager.Instance.OnRequestAddedForScan -= OnRequestAddedForScan;


        StartCoroutine(InitializeGame());
        //InitializeGame();
    }

    private void  CreateMapBuildings()
    {
        //yield return null;

        //Buildings
        BuildingObjects = new List<GameObject>();

        GameObject _mapBuildingsParent = new GameObject("MapBuildings");

        int buildingCount = 0;

        foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
        {
            if (LayerMask.LayerToName(mr.gameObject.layer) == SturfeeLayers.Building)
            {

                GameObject building2 = Instantiate(mr.gameObject);
                building2.transform.position = mr.transform.position;
                building2.transform.rotation = mr.transform.rotation;
                building2.GetComponent<Renderer>().material = _buildingMaterial;
                building2.layer = LayerMask.NameToLayer("mapBuilding");
                building2.transform.parent = _mapBuildingsParent.transform;

                buildingCount++;
            }
        }

        print("* Num Counted Buildings: " + buildingCount);
        print("* Num Instantiated Buildings: " + _mapBuildingsParent.transform.childCount);

        // TODO: Remove this
        //Terrain
        //TerrainObjects = new List<GameObject>();

        //foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
        //{
        //    if (LayerMask.LayerToName(mr.gameObject.layer) == SturfeeLayers.Terrain)
        //    {
        //        TerrainObjects.Add(mr.gameObject);
        //    }
        //}
    }

    private /*void*/ IEnumerator InitializeGame()
    {
        // Need to wait one frame for corrected GPS position to update for alls
        yield return null;

        UiManager.Instance.SetLocalizedUi();

        // OLD STUFF
        // TODO: Adjust this in one call
        _mapTouchPanel.SetActive(true);
        PlayerMapTouchController.Instance.gameObject.SetActive(false);

        PlayerController.Instance.Initialize();

        if(AllowPathing || AllowRadiusRelocalize)
        {
            RelocalizationManager.Instance.Initialize();

            if (AllowPathing)
            {
                PathingManager.Instance.CreatePath();
            }
        }

    }

}