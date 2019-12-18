using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using Sturfee.Unity.XR.Package.Utilities;
using System;
using System.Text;
using System.Net;
using System.Linq;
using System.Security.Cryptography;

// Controls the creation, interaction, and removal of AR Items in the environment via player touch on the screen
public class PlayerArTouchController : MonoBehaviour, IPointerDownHandler, IDragHandler{

	public static PlayerArTouchController Instance;

    [HideInInspector]
    public bool Relocalize = false;
    [HideInInspector]
    public bool SaveUiState;

	public RectTransform rectTransform;
	public Image TouchIndicator;

	[Header("Foursquare Touch Point Info")]
	public GameObject FoursquareDataUi;
	//public Text FoursquareDataText;
	public GameObject DropDownArrow;
	public GameObject MoreDataButton;
	public Transform DataList;
	public GameObject NoDataFoundNotice;
//    public GameObject ArExpandingCircle;

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

	[Header("Layer Masks")]
	[SerializeField]
	private LayerMask _interactLayerMask;

	[Header("Other")]
	//	public GameObject FourSquareDataUi;
	//public Text NameText;
	//public Text CheckinsText;

	[SerializeField]
	private GameObject _gridPrefab;
	[SerializeField]
	private GameObject _currentMapItemSelector;
    [SerializeField]
    private LocalizationManager _localizationManager;

	private bool _fourSquareDataOn = true;

	private Transform _currentPlacedItem;
	private int _curVenueNum;

    private Camera _playerXrCam;
    private List<FoursquareAPI.Venue> _venues;
	private FoursquareAPI.Venue[] _sortedVenues;
	private List<int> _sortedDistOfVenues;
	private int _maxVenues;

	private bool _moreDataActive = false; 

	private MapboxPlacesAPI.Features[] _mapboxPlaces;

//	private Text _fourSquareDataText;
	private bool _activePosUi = false;
	private bool _flagMode = false;

	private Vector2 _fullSizePanel;
	private Vector2 _halfSizePanel;

	private Transform _activeFlag;
	private SpriteRenderer _prevMapIcon;
	private int _prevMapIconOrderInLayer;

	private List<GameObject> _gridMarkersList;

	private int _curDataToggleState = 0;
	private FoursquareVenuePhotosAPI.Items[] _foursquarePhotosArray;
	private int _photosIndex;
	private RaycastHit _userTapHitPoint;
	private bool _lastTapGround = true;
    private bool _retrievingPhotos = false;

    // Save touch info
    private Vector2 _savedTouchPos;
    private bool _savedActiveTouchUi;
    //private bool _savedMoreDataActive

    private bool _raycastTouchAfterLocalization;

    private int _maxDataStates;

    private string _clientId = "JSZ9a4c81f31c67|u4Zqk";
    private string _clientSecret = "dBlKwesMS6m1qXVyD2g0LU7KHak";

    private ZenrinAPI.Item[] _items;

    public enum DataState
	{
		Poi, Distance, Photo
	}

    private void Awake()
	{
		Instance = this;
	}

	private void Start () {
		TouchIndicator.gameObject.SetActive (false);
//		_fourSquareDataText = TouchIndicator.GetComponentInChildren<Text> ();

		_currentMapItemSelector.SetActive (true);

		rectTransform = GetComponent<RectTransform> ();
		_fullSizePanel = rectTransform.sizeDelta;
		_halfSizePanel = new Vector2 (-960 /*rectTransform.sizeDelta.x / 2*/, rectTransform.sizeDelta.y);

		_gridMarkersList = new List<GameObject> ();

//		PhotoNavUi.SetActive(false);
		_foursquarePhotosArray = null;
        _playerXrCam = PlayerController.Instance.GetComponent<Camera>();

        _maxDataStates = (AppManager.Instance.AllowPhotoBillboard) ? 3 : 2;
	}

	private void Update()
	{
		if (_activePosUi)
		{
			float angle = Vector3.Angle (_playerXrCam.transform.forward, _currentPlacedItem.position - _playerXrCam.transform.position);
			//print ("!! ANGLE: " + angle);

			if (angle <= 90)
			{
				TouchIndicator.gameObject.SetActive (true);
				Vector3 screenPos = _playerXrCam.WorldToScreenPoint (_currentPlacedItem.position);
				TouchIndicator.rectTransform.position = screenPos;
			}
			else
			{
				TouchIndicator.gameObject.SetActive (false);
			}
		}
	}

	private void OnDisable()
	{
        if(SaveUiState)
        {
            _savedActiveTouchUi = _activePosUi;
        }
        else
        {
            _moreDataActive = false;
        }

        TouchIndicator.gameObject.SetActive (false);
		PhotoNavUi.SetActive (false);
		PhotoBillboard.SetActive (false);

//		_currentMapItemSelector.SetActive (false);
		_currentMapItemSelector.GetComponentInChildren<MapPlacementAnimator>().EndAnimation();

        _activePosUi = false;

        if (_prevMapIcon != null)
		{
			_prevMapIcon.sortingOrder = _prevMapIconOrderInLayer;
		}
		_prevMapIcon = null;
	}

    private void OnEnable()
    {
        if(SaveUiState)
        {
            _activePosUi = _savedActiveTouchUi;

            SaveUiState = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
	{
		ScreenTouchRaycast (eventData.pressPosition);
	}	

	public void OnDrag(PointerEventData data)
	{
		if (_flagMode)
		{
			ScreenTouchRaycast (data.position);
		}
	}

	public void ActivateHalfPanel(Transform flag)
	{
		rectTransform.sizeDelta = _halfSizePanel;
		gameObject.SetActive (true);
		_flagMode = true;
		_activeFlag = flag;
		print (" *** ACTIVATED AR HALF PANEL");
	}

	public void ActivateFullPanel()
	{
		rectTransform.sizeDelta = _fullSizePanel;
		gameObject.SetActive (true);
		_flagMode = false;
		print (" *** ACTIVATED AR FULL PANEL");
	}

	public void RemoveGridMarkers()
	{
		for (int i = 0; i < _gridMarkersList.Count; i++)
		{
			Destroy (_gridMarkersList [i]);
		}
		_gridMarkersList = new List<GameObject> ();
	}

	private void ScreenTouchRaycast(Vector2 touchPos)
	{
		Ray ray = _playerXrCam.ScreenPointToRay (touchPos);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1000, _interactLayerMask))
		{
			if (!_flagMode)
			{
           
                PlayerMapTouchController.Instance.FlagPulsate = false;

				NoDataFoundNotice.SetActive (false);
				DropDownArrow.SetActive (false);
				DropDownArrow.transform.rotation = Quaternion.Euler (Vector3.back * 90);
				MoreDataButton.SetActive (false);
				_moreDataActive = false;

				TurnOffFoursquareDataList (0);
				FoursquareDataUi.SetActive (false);
				ThreeDDataUi.SetActive (false);

				if (_prevMapIcon != null)
				{
					_prevMapIcon.sortingOrder = _prevMapIconOrderInLayer;
				}

                if(AppManager.Instance.AllowPathing && !_raycastTouchAfterLocalization)
                {
                    Relocalize = RelocalizationManager.Instance.RelocalizeCheck(true);
                }
                else    // Prevent infinite raycast/relocalizing
                {   
                    _raycastTouchAfterLocalization = false;
                }

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("sturfeeTerrain") || !Relocalize)
                {
                    Transform grid = Instantiate(_gridPrefab).transform;
                    grid.position = hit.point;
                    grid.rotation = Quaternion.LookRotation(hit.normal);

                    // Move selected map icon layer to top temporarily
                    _prevMapIcon = grid.GetComponent<ArItem>().MapRepresentation.GetComponent<SpriteRenderer>();
                    _prevMapIconOrderInLayer = _prevMapIcon.sortingOrder;
                    _prevMapIcon.sortingOrder = 8;  // Player sprites are on 10 and 9 currently

                    _gridMarkersList.Add(grid.gameObject);

                    _currentMapItemSelector.SetActive(true);
                    _currentMapItemSelector.transform.position = grid.position;
                    _currentMapItemSelector.transform.position -= Vector3.down * 0.1f;

                    _currentMapItemSelector.GetComponent<SizeScaleByDistance>().Activate();
                    _currentMapItemSelector.GetComponentInChildren<MapPlacementAnimator>().PlacementAnimation();

                    ArExpandingCircle.Instance.Activate(hit.point, Quaternion.LookRotation(hit.normal));

                    if (_currentPlacedItem != null)
                    {
                        _currentPlacedItem.GetComponentInChildren<GridItem>().FadeGrid();
                    }

                    _currentPlacedItem = grid;

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("sturfeeBuilding"))
                    {
                        _lastTapGround = false;
                        _userTapHitPoint = hit;

                        Prepare3dData(hit.point);

                        _retrievingPhotos = true;

                        StartCoroutine(GetZenrinData());
                        //StartCoroutine(GetCSharpZenrin());
                        //StartCoroutine(GetZenrinData());
                        //StartCoroutine(GetZenrinData2());


                        //StartCoroutine(GetFoursquareVenueData());

                        if (_curDataToggleState == (int)DataState.Poi)
                        {
                            TouchIndicator.gameObject.SetActive(true);
                            _activePosUi = true;

                            FoursquareDataUi.SetActive(true);
                        }
                        else if (_curDataToggleState == (int)DataState.Distance)
                        {
                            TouchIndicator.gameObject.SetActive(true);
                            _activePosUi = true;

                            ThreeDDataUi.SetActive(true);
                        }
                        else
                        {
                            ToastManager.Instance.ShowToast("Retrieving photos...");
                            //ScreenMessageController.Instance.SetText("Retrieving photos...");
                        }
                    }
                    else
                    {
                        _lastTapGround = true;
                        TouchIndicator.gameObject.SetActive(false);
                        _activePosUi = false;
                        PhotoNavUi.SetActive(false);
                        PhotoBillboard.SetActive(false);
                        //				FourSquareDataUi.SetActive (false);
                    }
                }
                else
                {
                    // save touch pos
                    // call relocalization

                    GetComponent<Image>().enabled = false;
                    _savedTouchPos = touchPos;

                    // TODO: Fix this
                    if (AppManager.Instance.AllowRadiusRelocalize)
                    {
                        RelocalizationManager.Instance.PerformRelocalization(true);
                        //XRSessionManager.GetSession().PerformLocalization();
                        //_localizationManager.PerformLocalization();

                    }
                }
            }
			else
			{
				_activeFlag.position = hit.point;
				_activeFlag.rotation = Quaternion.LookRotation (hit.normal);

				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("sturfeeTerrain") /*_activeFlag.rotation.eulerAngles.x > -45*/)
				{
					Vector3 toBeRot = _activeFlag.rotation.eulerAngles;
					toBeRot.y = PlayerController.Instance.transform.rotation.eulerAngles.y + 90;
					_activeFlag.rotation = Quaternion.Euler (toBeRot);
				}
			}
        }
	}
		
    public void RaycastAfterLocalization()
    {
        StartCoroutine(WaitToRaycast());
    }

    private IEnumerator WaitToRaycast()
    {
        yield return null;
        Relocalize = false;
        _raycastTouchAfterLocalization = true;
        ScreenTouchRaycast(_savedTouchPos);
        GetComponent<Image>().enabled = true;
    }

    private void Prepare3dData(Vector3 touchPos3d)
	{
		float distanceFromPlayer = Vector3.Magnitude(touchPos3d - _playerXrCam.transform.position);
		DistanceText.text = "Distance: " + distanceFromPlayer.ToString ("F2") + " m";
		ElevationText.text = "Elevation: " + touchPos3d.y.ToString("F2") + " m";
	}

    private IEnumerator GetZenrinData()
    {
        var gps = XRSessionManager.GetSession().LocalPositionToGps(_currentPlacedItem.position);
        //string url = "https://test.core.its-mo.com/zmaps/api/apicore/core/v1_0/poi/latlon?if_clientid=" +
        //"JSZ9a4c81f31c67|u4Zqk" +
        //"&if_auth_type=oauth&latlon=" + gps.Latitude.ToString() + "," + gps.Longitude.ToString() + "&radius=30";

        string URL = "https://test.core.its-mo.com/zmaps/api/apicore/core/v1_0/poi/latlon";

        List<QueryParameter> queryParams = new List<QueryParameter>();

        var if_clientid = new QueryParameter("if_clientid", _clientId);
        var if_auth_type = new QueryParameter("if_auth_type", "oauth");
        var latlon = new QueryParameter("latlon", gps.Latitude.ToString() + "," + gps.Longitude.ToString()); // <=== PUT ACTUAL GPS VALUES HERE
        var radius = new QueryParameter("radius", "50"); // <=== PUT ACTUAL RADIUS VALUE HERE

        queryParams.Add(if_clientid);
        queryParams.Add(if_auth_type);
        queryParams.Add(latlon);
        queryParams.Add(radius);

        var oauth_consumer_key = new QueryParameter("oauth_consumer_key", _clientId);
        var oauth_nonce = new QueryParameter("oauth_nonce", Guid.NewGuid().ToString("N") /*"1UQ4M9"*/);
        var oauth_timestamp = new QueryParameter("oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        var oauth_signature_method = new QueryParameter("oauth_signature_method", "HMAC-SHA1");
        var oauth_version = new QueryParameter("oauth_version", "1.0");

        queryParams.Add(oauth_consumer_key);
        queryParams.Add(oauth_nonce);
        queryParams.Add(oauth_timestamp);
        queryParams.Add(oauth_signature_method);
        queryParams.Add(oauth_version);

        var httpMethod = "GET";
        var encodedUri = Uri.EscapeDataString(URL);

        var urlEncodedParams = queryParams.Select(param => new QueryParameter(param.Key, param.Value)).ToList();
        var urlEncodedParamsString = string.Empty;

        int totalKeys = urlEncodedParams.Count;
        int keyCount = 0;

        foreach (var parameter in urlEncodedParams)
        {
            // must URL-encode before doing byte-order
            parameter.Key = Uri.EscapeDataString(parameter.Key);
            parameter.Value = Uri.EscapeDataString(parameter.Value);

            // also build the find querystrings to send via UnityWebRequest
            urlEncodedParamsString += parameter.Key + "=" + parameter.Value;
            if (++keyCount != totalKeys) // indicates the last item is found
                urlEncodedParamsString += "&";
        }

        Debug.Log("Querystrings:\n" + urlEncodedParamsString);

        // PART 1-2
        var orderedQueryParameters = urlEncodedParams
            .OrderBy(parm => parm, new QueryParameterComparer());

        // PART 1-3
        string parameterString = String.Empty;
        totalKeys = queryParams.Count;
        keyCount = 0;
        foreach (QueryParameter parameter in orderedQueryParameters)
        {
            parameterString += Uri.EscapeDataString(parameter.Key + "=" + parameter.Value);

            if (++keyCount != totalKeys) // indicates the last item is found
                parameterString += Uri.EscapeDataString("&");
        }

        // this is the final base string:
        var baseString = httpMethod + "&" + encodedUri + "&" + parameterString;
        Debug.Log("OAuth Base String:\n" + baseString);

        // PART 2: Generate OAuth signature using Signature Base String
        HMACSHA1 hmacsha1 = new HMACSHA1(new ASCIIEncoding().GetBytes(_clientSecret + "&"));
        string signature = Convert.ToBase64String(hmacsha1.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));
        Debug.Log("OAuth Signature:\n" + signature);



        // PART 3: make the request
        // based on here: https://support.e-map.ne.jp/manuals/v3/?q=auth#oauth
        string requestURL = URL + "?" + urlEncodedParamsString + "&oauth_signature=" + Uri.EscapeDataString(signature);
        Debug.Log("requestURL:\n" + requestURL);

        UnityWebRequest request = UnityWebRequest.Get(requestURL);
        // TODO: decide which headers we actually want to keep
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
        request.SetRequestHeader("Accept-Language", "en-US,en;q=0.9");
        request.SetRequestHeader("Cache-Control", "no-cache");
        request.SetRequestHeader("X-Zdc-Auth-Cache", "0");
        request.SetRequestHeader("X-Zdc-Auth-Expire", "3600");

        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // the data from the API
            Debug.Log(request.downloadHandler.text);
            var d = JsonUtility.FromJson<ZenrinAPI.RootObject>(request.downloadHandler.text);

            print("JSON Convert: " + d.item);
            print("Status text: " + d.status.text);
            print("POI List Length: " + d.item.Length);  // .poiList.Count);

            _items = d.item;

            _maxVenues = _items.Length;
            if (_maxVenues > 5)
            {
                _maxVenues = 5;
            }

            if (_items.Length > 0)
            {
                //SortFoursquareData();

                print("POI Text: " + _items[0].poi.text);
                print("POI EN: " + _items[0].poi.language.en.text);

                DataList.GetChild(0).GetComponent<ListItem>().SetLocationData(_items[0].poi.text, _items[0].poi.language.en.text);
                //DataList.GetChild(0).GetComponent<ListItem>().SetLocationData(_sortedVenues[0].name, _sortedVenues[0].stats.checkinsCount);


                if (_items.Length > 1)
                {
                    DropDownArrow.SetActive(true);
                    MoreDataButton.SetActive(true);
                }
            }
            else
            {
                NoDataFoundNotice.SetActive(true);
                DoneRetrievingPhotos(false);
            }
        }

    }

    //private void SortFoursquareData()
    //{
    //    _maxVenues = _venues.Count;
    //    if (_maxVenues > 5)
    //    {
    //        _maxVenues = 5;
    //    }

    //    _sortedDistOfVenues = new List<int>();

    //    // Make a copy of the venues, just holding the distances
    //    for (int i = 0; i < _venues.Count /*_maxVenues*/; i++)
    //    {
    //        _sortedDistOfVenues.Add(_venues[i].location.distance);
    //    }
    //    _sortedDistOfVenues.Sort();

    //    // Find the corresponding venue by the distance found in the sorted distance of venues array
    //    _sortedVenues = new FoursquareAPI.Venue[_maxVenues];
    //    for (int i = 0; i < _maxVenues; i++)
    //    {
    //        int count = 0;
    //        bool foundVenue = false;
    //        while (!foundVenue && count < _venues.Count /*_maxVenues*/)
    //        {

    //            if (_sortedDistOfVenues[i] == _venues[count].location.distance)
    //            {
    //                _sortedVenues[i] = _venues[count];
    //                foundVenue = true;

    //                _venues.RemoveAt(count);
    //            }
    //            count++;
    //        }
    //    }
    //}


    private IEnumerator GetFoursquareVenueData()
	{
		var gps = XRSessionManager.GetSession ().LocalPositionToGps (_currentPlacedItem.position);
		//string Lat = "37.7896208543775";
		//string Lng = "-122.38893210641514";
		string url = "https://api.foursquare.com/v2/venues/search?ll=" + gps.Latitude.ToString()/*Lat*/ + "," 
			+ gps.Longitude.ToString()/*Lng*/ + "&radius=30&oauth_token=VXIREDRYNSJEZL3OOQ3GLUF03HZXQQLRENNA5ERWUF1DYBMH&v=20160419&intent=browse";
		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			yield return www.SendWebRequest();

			if (www.isNetworkError)
			{
				print ("!!! ERROR");
				Debug.Log(www.error);
                DoneRetrievingPhotos(false, false);
            }
			else
			{
				var d = JsonUtility.FromJson<FoursquareAPI.RootObject>(www.downloadHandler.text);
                print("Foursquare data: " + www.downloadHandler.text);
				_venues = d.response.venues;
				_curVenueNum = 0;

				if (_venues.Count > 0)
				{
					// TODO: Sort by distance here

					SortFoursquareData ();

                    if (AppManager.Instance.AllowPhotoBillboard)
                    {
                        StartCoroutine(GetFoursquareVenuePhotos());
                    }
					DataList.GetChild (0).GetComponent<ListItem> ().SetLocationData (_sortedVenues[0].name, _sortedVenues[0].stats.checkinsCount);
//					DataList.GetChild (0).GetComponent<ListItem> ().SetLocationData (_venues [0].name, _venues [0].stats.checkinsCount);

//					FoursquareDataText.text = "Name: " + _venues [0].name + "\nCheckins: " + _venues [0].stats.checkinsCount;


//					FourSquareDataUi.SetActive (true);
//					NameText.text = "Name:\n" + _venues [0].name;
//					CheckinsText.text = "Checkins:\n" + _venues [0].stats.checkinsCount;


//							_maxVenues = _venues.Count;
//							if (_maxVenues > 5)
//							{
//								_maxVenues = 5;
//							}
//

					if (_sortedVenues.Length /*_venues.Count*/ > 1)
					{
						DropDownArrow.SetActive (true);
						MoreDataButton.SetActive (true);
					}
				}
				else
				{
					NoDataFoundNotice.SetActive (true);
                    DoneRetrievingPhotos(false);
                }
            }
		}
	}

	// Access the photos of a venue
	private IEnumerator GetFoursquareVenuePhotos()
	{
		print ("VENUES COUNT: " + _sortedVenues.Length);
		print ("VENUE NAME: " + _sortedVenues[0].name + "VENUE ID: " + _sortedVenues[0].id);

		string url = "https://api.foursquare.com/v2/venues/" + _sortedVenues[0].id + "/photos?oauth_token=VXIREDRYNSJEZL3OOQ3GLUF03HZXQQLRENNA5ERWUF1DYBMH&v=20160419&intent=browse";
		using (UnityWebRequest www = UnityWebRequest.Get (url))
		{
			yield return www.SendWebRequest ();

			if (www.isNetworkError)
			{
				print ("!!! Venue Photos ERROR");
				Debug.Log (www.error);
                DoneRetrievingPhotos(false, false);
                //_retrievingPhotos = false;

                ToastManager.Instance.ShowToastTimed("Network error:\n" + www.error, 3);
                //ScreenMessageController.Instance.SetText ("Network error:\n" + www.error, 3);
			}
			else
			{
				print ("FOURSQUARE VENUE PHOTOS SUCCESS");
				var d = JsonUtility.FromJson<FoursquareVenuePhotosAPI.RootObject> (www.downloadHandler.text);
				print ("PHOTOS COUNT: " + d.response.photos.count);

				if (d.response.photos.count != 0)
				{
					// TODO: Do an angle check here, then do a new raycast check if angle is bad fit

					_foursquarePhotosArray = d.response.photos.items;
					_photosIndex = 0;

					PhotoNavUi.GetComponentInChildren<Text> ().text = (_photosIndex + 1).ToString () + " / " + _foursquarePhotosArray.Length.ToString ();

					StartCoroutine (GetFoursquarePhoto (_foursquarePhotosArray[0] /*photoUrl*/));
				}
				else
				{
                    _foursquarePhotosArray = null;
                    DoneRetrievingPhotos(false);
                }
			}
		}
	}

	private IEnumerator GetFoursquarePhoto(FoursquareVenuePhotosAPI.Items item /*string photoUrl*/, bool placeBillboard = true)
	{
		string photoUrl = item.prefix + item.width.ToString () + "x" + item.height.ToString () + item.suffix;

		using (WWW www = new WWW (photoUrl))
		{
			// Wait for download to complete
			yield return www;

            ToastManager.Instance.HideToast();
            //ScreenMessageController.Instance.ClearText ();

            if (_curDataToggleState == (int)DataState.Photo)
			{
				PhotoBillboard.SetActive (true);
				PhotoNavUi.SetActive (true);
			}

			if (placeBillboard)
			{
                PhotoBillboard.GetComponent<PhotoBillboardController>().PlaceBillboard(_userTapHitPoint, www.texture, item.width, item.height);
            }
			else
			{
                PhotoBillboard.GetComponent<PhotoBillboardController>().PlaceBillboard(_userTapHitPoint, www.texture, item.width, item.height);
            }

            DoneRetrievingPhotos(true);
        }
	}

	private void SortFoursquareData()
	{
		_maxVenues = _venues.Count;
		if (_maxVenues > 5)
		{
			_maxVenues = 5;
		}

		_sortedDistOfVenues = new List<int> ();

		// Make a copy of the venues, just holding the distances
		for (int i = 0; i < _venues.Count /*_maxVenues*/; i++)
		{
			_sortedDistOfVenues.Add(_venues [i].location.distance);
		}
		_sortedDistOfVenues.Sort ();

		// Find the corresponding venue by the distance found in the sorted distance of venues array
		_sortedVenues = new FoursquareAPI.Venue[_maxVenues];
		for (int i = 0; i < _maxVenues; i++)
		{
			int count = 0;
			bool foundVenue = false;
			while (!foundVenue && count < _venues.Count /*_maxVenues*/)
			{

				if (_sortedDistOfVenues [i] == _venues [count].location.distance)
				{
					_sortedVenues [i] = _venues [count];
					foundVenue = true;

					_venues.RemoveAt (count);
				}
				count++;
			}
		}
	}

	public void ToggleFoursquareData(bool active)
	{
		_fourSquareDataOn = active;
		FoursquareDataUi.SetActive (active);
		ThreeDDataUi.SetActive(!active);
		foursquareDataIcon.SetActive (active);
		threeDDataIcon.SetActive(!active);
	}

	public void ToggleData()
	{
		_curDataToggleState++;
		if (_curDataToggleState >= _maxDataStates)
		{
			_curDataToggleState = 0;
		}

		if (_curDataToggleState == (int)DataState.Poi)
		{
			// Foursquare POI

			_activePosUi = true;
			foursquareDataIcon.SetActive (true);
            threeDDataIcon.SetActive(false);
            PhotoIcon.SetActive (false);

			FoursquareDataUi.SetActive (true);
            ThreeDDataUi.SetActive(false);

            PhotoNavUi.SetActive (false);
			PhotoBillboard.SetActive (false);
		}
		else if (_curDataToggleState == (int)DataState.Distance)
		{
			// Distance/Elevation

			foursquareDataIcon.SetActive (false);
			threeDDataIcon.SetActive(true);

			FoursquareDataUi.SetActive (false);
			ThreeDDataUi.SetActive(true);
		}
		else
		{
			// Foursquare photos

			threeDDataIcon.SetActive(false);
			_activePosUi = false;
			TouchIndicator.gameObject.SetActive (false);
			PhotoIcon.SetActive (true);
			ThreeDDataUi.SetActive(false);

            StartCoroutine(WaitForPhotos());
		}

		if (_lastTapGround)
		{
			_activePosUi = false;
			TouchIndicator.gameObject.SetActive (false);
			PhotoNavUi.SetActive (false);
			PhotoBillboard.SetActive (false);
		}
	}

    private IEnumerator WaitForPhotos()
    {
        print("!! In wait for photos");

        if(_retrievingPhotos)
        {
            ToastManager.Instance.ShowToast("Retrieving photos...");
            //ScreenMessageController.Instance.SetText("Retrieving photos...");
        }

        while (_retrievingPhotos)
        {
            yield return null;
        }

        if (_foursquarePhotosArray != null && _foursquarePhotosArray.Length > 0)
        {
            PhotoNavUi.SetActive(true);
            PhotoBillboard.SetActive(true);
        }
        else
        {
            print("!! Didn't find photos");
            ToastManager.Instance.ShowToastTimed("No photos found here", 3);
            //ScreenMessageController.Instance.SetText("No photos found here", 3);
            PhotoBillboard.SetActive(false);
            PhotoNavUi.SetActive(false);
        }
    }

	public void OnMoreDataDropDownClick()
	{
		_moreDataActive = !_moreDataActive;

		if (_moreDataActive)
		{
			DropDownArrow.transform.rotation = Quaternion.Euler(Vector3.back * 270);

			//print ("!* MAX VENUES: " + maxVenues);
			for (int i = 1; i < /*_mapboxPlaces.Length*/ _maxVenues; i++)
			{
                DataList.GetChild(i).GetComponent<ListItem>().SetLocationData(_items[i].poi.text, _items[i].poi.language.en.text);
                //DataList.GetChild (i).GetComponent<ListItem> ().SetLocationData (_sortedVenues [i].name, _sortedVenues [i].stats.checkinsCount);
            }
		}
		else
		{
			DropDownArrow.transform.rotation = Quaternion.Euler(Vector3.back * 90);
			TurnOffFoursquareDataList (1);
		}
	}

	private void TurnOffFoursquareDataList(int pos)
	{
		for (int i = pos; i < DataList.childCount; i++)
		{
			DataList.GetChild (i).GetComponent<ListItem> ().ClearData ();
			//DataList.GetChild (i).gameObject.SetActive (false);
		}
	}

	public void OnPhotoNavClick(int dir)
	{
		if (dir >= 0)
		{
			_photosIndex++;
			if(_photosIndex >= _foursquarePhotosArray.Length)
			{
				_photosIndex = 0;
			}
		}
		else
		{
			_photosIndex--;
			if(_photosIndex < 0)
			{
				_photosIndex = _foursquarePhotosArray.Length - 1;
			}
		}

		PhotoNavUi.GetComponentInChildren<Text> ().text = (_photosIndex + 1).ToString () + " / " + _foursquarePhotosArray.Length.ToString ();

		StartCoroutine (GetFoursquarePhoto (_foursquarePhotosArray [_photosIndex], false));
	}

    private void DoneRetrievingPhotos(bool success, bool error = false)
    {
        _retrievingPhotos = false;
        //ScreenMessageController.Instance.ClearText();

        if (_curDataToggleState == (int)DataState.Photo)
        {
            if (error)
            {
                ToastManager.Instance.ShowToastTimed("Error retrieving photos", 3);
                //ScreenMessageController.Instance.SetText("Error retrieving photos", 3);
            }
            else
            {
                if (success)
                {
                    ToastManager.Instance.HideToast();
                    //ScreenMessageController.Instance.ClearText();
                }
                else
                {
                    ToastManager.Instance.ShowToastTimed("No photos found here", 3);
                    //ScreenMessageController.Instance.SetText("No photos found here", 3);
                    PhotoBillboard.SetActive(false);
                    PhotoNavUi.SetActive(false);
                }
            }
        }
    }

    #region UNUSED
    //private IEnumerator GetMapboxGeocodingData()
    //{
    //    var gps = XRSessionManager.GetSession().LocalPositionToGps(_currentPlacedItem.position);

    //    //      string url = "https://api.mapbox.com/v4/mapbox.mapbox-streets-v7/tilequery/" + gps.Longitude.ToString () + "," + gps.Latitude.ToString () +
    //    //                   ".json?radius=100&layers=poi_label,building&access_token=pk.eyJ1IjoibmV3bWFudyIsImEiOiJjamZrMTVoeTUwMGhpMndrMjNwOGpubWNvIn0.97Afy3acMY3j2Hsg7P4e3Q";

    //    string url = "https://api.mapbox.com/geocoding/v5/mapbox.places/" + gps.Longitude.ToString() + "," + gps.Latitude.ToString() +
    //        ".json?types=poi&limit=5&reverseMode=distance&access_token=pk.eyJ1IjoibmV3bWFudyIsImEiOiJjamZrMTVoeTUwMGhpMndrMjNwOGpubWNvIn0.97Afy3acMY3j2Hsg7P4e3Q";


    //    //      // Attempt
    //    //      string url = "https://api.mapbox.com/geocoding/v5/mapbox.places/" + gps.Longitude.ToString() + "," +  gps.Latitude.ToString() + 
    //    //          ".json?types=poi&bbox=0.000001,0.000001,0.000001,0.000001&limit=5&reverseMode=distance&access_token=pk.eyJ1IjoibmV3bWFudyIsImEiOiJjamZrMTVoeTUwMGhpMndrMjNwOGpubWNvIn0.97Afy3acMY3j2Hsg7P4e3Q";

    //    //      // Attempt
    //    //      string url = "https://api.mapbox.com/geocoding/v5/mapbox.places/" + gps.Longitude.ToString() + "," +  gps.Latitude.ToString() + 
    //    //          ".json?types=poi&bbox=" + (gps.Longitude - 0.0001f).ToString() + "," + (gps.Latitude - 0.0001f).ToString() + "," +
    //    //          (gps.Longitude + 0.0001f).ToString() + "," + (gps.Latitude + 0.0001f).ToString() +
    //    //          "&limit=5&reverseMode=distance&access_token=pk.eyJ1IjoibmV3bWFudyIsImEiOiJjamZrMTVoeTUwMGhpMndrMjNwOGpubWNvIn0.97Afy3acMY3j2Hsg7P4e3Q";


    //    // Filtered to just Points of interest
    //    //      string url = "https://api.mapbox.com/geocoding/v5/mapbox.places/" + gps.Longitude.ToString() + "," +  gps.Latitude.ToString() 
    //    //          /*+ "," + gps.Height.ToString()*/ + ".json?types=poi&access_token=pk.eyJ1IjoibmV3bWFudyIsImEiOiJjamZrMTVoeTUwMGhpMndrMjNwOGpubWNvIn0.97Afy3acMY3j2Hsg7P4e3Q";

    //    // Basic
    //    //      string url = "https://api.mapbox.com/geocoding/v5/mapbox.places/" + gps.Longitude.ToString() + "," +  gps.Latitude.ToString() 
    //    //          /*+ "," + gps.Height.ToString()*/ + ".json?access_token=pk.eyJ1IjoibmV3bWFudyIsImEiOiJjamZrMTVoeTUwMGhpMndrMjNwOGpubWNvIn0.97Afy3acMY3j2Hsg7P4e3Q";


    //    using (UnityWebRequest www = UnityWebRequest.Get(url))
    //    {
    //        yield return www.Send();

    //        if (www.isNetworkError)
    //        {
    //            print("!!! MAPBOX PLACES ERROR");
    //            Debug.Log(www.error);
    //        }
    //        else
    //        {
    //            print("*** MAPBOX PLACES URL SUCCEEDED");

    //            var d = JsonUtility.FromJson<MapboxPlacesAPI.Response>(www.downloadHandler.text);
    //            _mapboxPlaces = d.features;

    //            print("*** PLACE NAME: " + d.features[0].place_name + ", " + d.features[0].text);

    //            print("FEATURES COUNT: " + d.features.Length);

    //            for (int i = 0; i < d.features.Length; i++)
    //            {
    //                print("*** " + i + ", " + d.features[i].text);
    //            }

    //            if (_mapboxPlaces.Length > 0)
    //            {
    //                DataList.GetChild(0).GetComponent<ListItem>().SetLocationData2(_mapboxPlaces[0].text, _mapboxPlaces[0].properties.category);

    //                if (_mapboxPlaces.Length > 1)
    //                {
    //                    DropDownArrow.SetActive(true);
    //                    MoreDataButton.SetActive(true);
    //                }
    //            }
    //            else
    //            {
    //                NoDataFoundNotice.SetActive(true);
    //            }
    //        }
    //    }
    //}
    #endregion

}
