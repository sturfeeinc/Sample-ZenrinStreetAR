using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchIndicator : MonoBehaviour {

    public static TouchIndicator Instance;

    public enum DataState
    { 
        Poi, Distance, Photo
    }

    // TODO: Maybe change this from AppManager (UNUSED CURRENTLY)
    public bool usePoiPhotos;

    [Header("Foursquare POI UI Components")]
    public GameObject FoursquareDataUi;
    public GameObject DropDownArrow;
    public GameObject MoreDataButton;
    public Transform DataList;
    public GameObject NoDataFoundNotice;

    [Header("3d Touch Point Info")]
    public GameObject ThreeDDataUi;
    public Text DistanceText;
    public Text ElevationText;
    public GameObject foursquareDataIcon;
    public GameObject threeDDataIcon;

    [Header("Photo Billboard")]
    public GameObject PhotoBillboard;
    public GameObject PhotoIcon;
    public GameObject PhotoNavUi;

    [HideInInspector]
    public bool ActivePosUi;    // Whether the POI or elevation data is on


    private int _curDataState;

    private Transform _player;


    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        _player = PlayerController.Instance.transform;
	}

    private void Update()
    {

    }

    public void NextDataState()
    {
        //_curDataState++;
        //if (_curDataState > 2)
        //{
        //    _curDataState = 0;
        //}

        //if (_curDataState == (int)DataState.Poi)
        //{
        //    // Foursquare POI

        //    ActivePosUi = true;
        //    //foursquareDataIcon.SetActive(true);
        //    //PhotoIcon.SetActive(false);
        //    //FoursquareDataUi.SetActive(true);
        //    PhotoNavUi.SetActive(false);
        //    PhotoBillboard.SetActive(false);
        //}
        //else if (_curDataState == (int)DataState.Distance)
        //{
        //    // Distance/Elevation

        //    //          _fourSquareDataOn = false;

        //    foursquareDataIcon.SetActive(false);
        //    threeDDataIcon.SetActive(true);

        //    FoursquareDataUi.SetActive(false);
        //    ThreeDDataUi.SetActive(true);
        //}
        //else
        //{
        //    // Foursquare photos

        //    threeDDataIcon.SetActive(false);
        //    ActivePosUi = false;
        //    TouchIndicator.gameObject.SetActive(false);
        //    PhotoIcon.SetActive(true);
        //    ThreeDDataUi.SetActive(false);

        //    StartCoroutine(WaitForPhotos());
        //}

        //if (_lastTapGround)
        //{
        //    ActivePosUi = false;
        //    TouchIndicator.gameObject.SetActive(false);
        //    PhotoNavUi.SetActive(false);
        //    PhotoBillboard.SetActive(false);
        //}

    }


}
