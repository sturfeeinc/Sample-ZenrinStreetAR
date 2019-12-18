using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using Amazon;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public bool UseS3;

	[Header("UI")]
	public GameObject ScanButton;
	public GameObject ExitButton;
	//public GameObject AlignmentInstrText;
	public GameObject UpdateGpsButton;
//	public GameObject DebugBuildingsToggle;
	public GameObject OptionsButton;
	public GameObject OptionsPopOutButtons;
	public GameObject ExpandMapButton;

	public GameObject ScanAnimation;
    public Material BuildingMaterial;

	[SerializeField]
	private GameObject _mapTouchPanel;

    [SerializeField]
    private GameObject _multiframeCamera;

    [HideInInspector]
    public List<GameObject> BuildingObjects;
    [HideInInspector]
    public List<GameObject> TerrainObjects;
	[HideInInspector]
	public bool PostScan = false;

	private GameObject _mapBuildingsParent;

    private Transform _sturfeeMapData;
    public Transform GetSturfeeMapData { get { return _sturfeeMapData; } }

	private float _scanTime;
	private bool _debugBuildings = false;

	void Awake()
	{
		Instance = this;

		//SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
		//SturfeeEventManager.Instance.OnSessionFailed += OnSessionFailed;
		//SturfeeEventManager.Instance.OnCoverageCheckComplete += OnCoverageCheckComplete;
		//SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
		//SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;

		ScanButton.SetActive (false);
		ScanAnimation.SetActive (false);
		ExitButton.SetActive (false);
		UpdateGpsButton.SetActive (false);
//		DebugBuildingsToggle.SetActive (false);
		OptionsButton.SetActive(true);
		OptionsPopOutButtons.SetActive (false);

		_scanTime = Time.time - 20;
//		PlayerMapTouchController.Instance.gameObject.SetActive (true);

//		UnityInitializer.AttachToGameObject(this.gameObject);


	}

	private void Start () {
//		PlayerMapTouchController.Instance.gameObject.SetActive (false);

		ScreenMessageController.Instance.SetText ("Initializing Session...");
	}


	public void OnScanClick()
	{
		if ((Time.time - _scanTime) < 10)
		{
			ScreenMessageController.Instance.SetText ("Can't rescan yet\n" +
				"Please wait " + (10 - (Time.time - _scanTime)).ToString("F1") + " more seconds", 3);
		}
		else
		{

			_mapBuildingsParent = new GameObject ("MapBuildings");

	//		ExitButton.SetActive (false);
			ScreenMessageController.Instance.SetText ("Align the circle towards the center of the rings");
			ScanButton.SetActive (false);
//			ScanAnimation.SetActive (true);
			UpdateGpsButton.SetActive (false);
			ExpandMapButton.SetActive (false);

			CustomMultiframeManager.Instance.OnMultiframeButtonClick ();
//			XRSessionManager.GetSession ().PerformLocalization ();


//			StartCoroutine (AlignmentStuckErrorTimer());



	//		print ("*** Performing Localization");
		}
	}

	public void OnExitClick()
	{
//		SceneManager.LoadScene ("Game");
		Application.Quit();

//		XRSessionManager.GetSession ().LocalPositionToGps (transformPos);
	}

	public void OnOptionsClick()
	{
		if (!OptionsPopOutButtons.activeSelf)
		{
			OptionsPopOutButtons.SetActive (true);
		}
		else
		{
			OptionsPopOutButtons.SetActive (false);
		}
	}

//	public void OnRescanClick()
//	{
//		// Sturfee server has a 10 second timer between scan calls to prevent 
//		if ((Time.time - _scanTime) < 10)
//		{
//			ScreenMessageController.Instance.SetText ("Can't rescan yet\n" +
//				"Please wait " + (10 - (Time.time - _scanTime)).ToString("F1") + " more seconds", 3);
//		}
//		else
//		{
////			OptionsButton.GetComponent<Button> ().interactable = false;
//			OptionsButton.SetActive(true);
//			OptionsPopOutButtons.SetActive (false);

//			if (PlayerMapTouchController.Instance != null)
//			{
////				print ("!!! MAP TOUCH CONTROLLER has been set");
//				PlayerMapTouchController.Instance.RemoveMarkers(); //OnRemoveMarkersClick ();
//			}
//			else
//			{
////				print ("!!! MAP TOUCH CONTROLLER IS NULL");
//				PlayerArTouchController.Instance.RemoveGridMarkers ();
//			}

//			//PlayerController.Instance.RescanReset ();
////			PlayerController.touch
////			PlayerArTouchController.Instance.Reset ();
//			PlayerArTouchController.Instance.gameObject.SetActive (false);
////			PlayerArTouchController.Instance.threeDDataIcon.SetActive (false);
	//		OnScanClick ();
	//	}
	//}

	public void OnHomeClick()
	{
		SceneManager.LoadScene ("Game");

//		OptionsButton.SetActive(false);
//		OptionsPopOutButtons.SetActive (false);
//		UpdateGpsButton.SetActive (true);
//
//		if (_debugBuildings)
//		{
//			XRSessionManager.GetSession ().ToggleDebugBuildings ();
//			_debugBuildings = false;
//		}
//
//		if (PlayerMapTouchController.Instance != null)
//		{
//			PlayerMapTouchController.Instance.RemoveMarkers (); //OnRemoveMarkersClick ();
//		}
//		else
//		{
//			PlayerArTouchController.Instance.RemoveGridMarkers ();
//		}
//
//		MiniMapCamera.Instance.CenterCamOverPos (PlayerController.Instance.transform.position);
//		PlayerController.Instance.RescanReset ();
//
//		PlayerArTouchController.Instance.gameObject.SetActive (false);
//
//		Destroy (_mapBuildingsParent);
//
//		PostScan = false;
//		ScanButton.SetActive (true);
	}

	public void OnUpdateGpsClick()
	{
		XRSessionManager.GetSession ().ForceLocationUpdate ();
		MiniMapCamera.Instance.CenterCamOverPos (PlayerController.Instance.transform.position);
		FullMapCamera.Instance.CenterCamOverPos (PlayerController.Instance.transform.position);
//		PlayerController.Instance.PreScanInitialize ();

	}

	public void DebugBuildingsBool()
	{
		_debugBuildings = !_debugBuildings;
	}

	public void OnDestroy()
	{
		//SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
		//SturfeeEventManager.Instance.OnSessionFailed -= OnSessionFailed;
		//SturfeeEventManager.Instance.OnCoverageCheckComplete -= OnCoverageCheckComplete;
		//SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;
		//SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
	}

	// Sturfee event called when the Sturfee XR Session is ready to be used
	private void OnSessionReady()
	{



//		ExitButton.SetActive (true);
//		ScreenMessageController.Instance.ClearText ();
		ScreenMessageController.Instance.SetText("Checking location coverage...");

		XRSessionManager.GetSession ().CheckCoverage ();

////        SetupSturfeeData();
//		MapManager.Instance.InitializeMap ();
//		PlayerController.Instance.PreScanInitialize ();
//
//		UpdateGpsButton.SetActive (true);
//		ScanButton.SetActive (true);
////		DebugBuildingsToggle.SetActive (true);
//
//
//		if (UseS3)
//		{
//			SaveLoadManager.LoadFile ();
//		}
			

	}

	private void OnSessionFailed(string error)
	{
		ScreenMessageController.Instance.SetText("Session initialization failed: " + error + "\nRestart App");
	}

	private void OnCoverageCheckComplete(bool result)
	{
		//if (result == false)
		//{
		//	ScreenMessageController.Instance.SetText ("Localization not available at this location\nRestart app in covered location");
		//	//return;
		//}
		//else
		//{
		//	ScreenMessageController.Instance.ClearText ();

		//	//        SetupSturfeeData();
		//	MapManager.Instance.InitializeMap ();
		//	PlayerController.Instance.PreScanInitialize ();

		//	UpdateGpsButton.SetActive (true);
		//	ScanButton.SetActive (true);
		//	//		DebugBuildingsToggle.SetActive (true);


		//	if (UseS3)
		//	{
		//		SaveLoadManager.LoadFile ();
		//	}
		//}

		////Debug.Log("Localization available");
	}

    private void SetupSturfeeData()
    {
        // TODO: 25 is based on 5x5 tiles built into the SDK

        /*
        _sturfeeMapData = GameObject.Find("Session Game Objects").transform.Find("SturfeeTiles_Building");
        for(int i = 0; i < 25; i++)
        {
            Transform buildingTile = _sturfeeMapData.GetChild(i);
            for(int j = 0; j < buildingTile.childCount; j++)
            {
                buildingTile.GetChild(j).GetComponent<Renderer>().material = BuildingMaterial;
                //buildingTile.GetChild(j).gameObject.layer = LayerMask.NameToLayer("map");
            }

        }*/

        //Buildings
        BuildingObjects = new List<GameObject>();

        foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
        {
            if (LayerMask.LayerToName(mr.gameObject.layer) ==  SturfeeLayers.Building)
            {

				GameObject building2 = Instantiate (mr.gameObject);
				building2.transform.position = mr.transform.position;
				building2.transform.rotation = mr.transform.rotation;
				building2.GetComponent<Renderer>().material = BuildingMaterial;
				building2.layer = LayerMask.NameToLayer ("map");
				building2.transform.parent = _mapBuildingsParent.transform;

                //print("*! BUILDING OBJECT");
//                mr.gameObject.GetComponent<Renderer>().material = BuildingMaterial;
//                BuildingObjects.Add(mr.gameObject);
            }
        }

        //Terrain
        TerrainObjects = new List<GameObject>();

        foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
        {
            if (LayerMask.LayerToName(mr.gameObject.layer) == SturfeeLayers.Terrain)
            {
                //print("*! Terrain OBJECT");
                TerrainObjects.Add(mr.gameObject);
            }
        }


    }

	private void OnLocalizationLoading()
	{
		ScreenMessageController.Instance.SetText ("Connecting to Sturfee VPS...");

//		ScreenMessageController.Instance.ClearText ();
		ScanAnimation.SetActive (true);
	}

	// Sturfee event called when camera alignment completes
	private void OnLocalizationSuccessful()
	{
//		ScanAnimation.SetActive (false);
////		ExitButton.SetActive (true);

//		if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.Done)
//		{
//            //			SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
//            //			SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
//            _multiframeCamera.SetActive(false);

//			ScreenMessageController.Instance.SetText ("Camera Alignment Complete!", 3);

//			PostScan = true;
//			_scanTime = Time.time;

//			SetupSturfeeData ();

//			UpdateGpsButton.SetActive (false);
////			ExitButton.SetActive (true);
//			OptionsButton.SetActive (true);

////			MapManager.Instance.InitializeMap ();

////			_mapTouchPanel.SetActive (true);

		//	// TODO: Temporary coroutine until next SDK fixes this issue (Current: v 0.9.1)
		//	// ....Now this frame wait is necessary though
		//	StartCoroutine (InitializeGame ());
		//}
		//else
		//{
		//	// Error occurred

		//	if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.IndoorsError)
		//	{
		//		ScreenMessageController.Instance.SetText ("Indoors Error. Please try again outside.", 3);
		//	}
  //          else if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.OutOfCoverage)
  //          {
  //              ScreenMessageController.Instance.SetText("Localization not available at this location");
  //          }
  //          else if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.Error)
		//	{
		//		ScreenMessageController.Instance.SetText ("Alignment Failed", 3);
		//	}
  //          else if (status == Sturfee.Unity.XR.Core.Constants.Enums.LocalizationStatus.RequestError)
  //          {
  //              ScreenMessageController.Instance.SetText("Request Error", 3);
  //          }
  //          else
		//	{
		//		ScreenMessageController.Instance.SetText ("Error Occurred", 3);
		//	}

		//	ScanButton.SetActive (true);
		//	UpdateGpsButton.SetActive (true);
		//}

	}

	private IEnumerator InitializeGame()
	{
		// Need to wait one frame for corrected GPS position to update for map calls
		yield return null;

		//MapManager.Instance.InitializeMap();

//		string CLIENT_ID = "";
//		string CLIENT_SECRET = "";
//		string Lat = "37.7896208543775";
//		string Lng = "-122.38893210641514";
//		string Query = "donuts";
//		//string url = "https://api.foursquare.com/v2/venues/search?v=20161016&ll=" + Lat + "," + Lng + "&intent=checkin&query="+Query+"&client_id=" + CLIENT_ID + "&client_secret=" + CLIENT_SECRET;
//		string url = "https://api.foursquare.com/v2/venues/search?ll=" + Lat + "," + Lng + "&radius=100&oauth_token=VXIREDRYNSJEZL3OOQ3GLUF03HZXQQLRENNA5ERWUF1DYBMH&v=20160419&intent=browse";
//		//"https://api.foursquare.com/v2/venues/search?ll=37.7896208543775,-122.38893210641514&radius=100&oauth_token=VXIREDRYNSJEZL3OOQ3GLUF03HZXQQLRENNA5ERWUF1DYBMH&v=20160419&intent=browse\\"
//		using (UnityWebRequest www = UnityWebRequest.Get(url))
//		{
//			yield return www.Send();
//
//			if (www.isNetworkError)
//			{
//				print ("!!! ERROR");
//				Debug.Log(www.error);
//			}
//			else
//			{
//				print ("***");
//				// Show results as text
//				Debug.Log(www.downloadHandler.text);
//
//			}
//		}
//		PlayerController.Instance.PreScanInitialize ();

//		MapManager.Instance.InitializeMap ();

		_mapTouchPanel.SetActive (true);

		PlayerMapTouchController.Instance.gameObject.SetActive(false);

		PlayerController.Instance.Initialize ();
		//PlayerUi.Initialize ();



		if (UseS3)
		{
			SaveLoadManager.LoadGameData ();
		}
	}

	private IEnumerator AlignmentStuckErrorTimer()
	{
//		print ("CAlled alignment TIMER");
		_scanTime = Time.time;

		float endTimer = Time.time + 11;
		while (ScanAnimation.activeSelf && Time.time < endTimer)
		{
			yield return null;
		}

		if (ScanAnimation.activeSelf)
		{
//			Reset ();
			ScanAnimation.SetActive (false);
			ScanButton.SetActive (true);
			UpdateGpsButton.SetActive (true);
			ScreenMessageController.Instance.SetText ("Camera alignment timed out", 3);
		}
	}

}
