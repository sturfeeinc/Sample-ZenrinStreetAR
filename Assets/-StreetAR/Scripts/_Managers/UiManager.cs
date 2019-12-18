using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Session;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// TODO: Remove OnArClick and OnFullMapClick from PlayerController

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Menu Buttons")]
    //public GameObject UpdateGpsButton;
    public GameObject OptionsPopOutButtons;
    public GameObject ExpandMapButton;
    public GameObject ArButton;
    public GameObject DataToggle;

    [Header("Map Buttons")]
    public GameObject FlagPlacementButton;
    public GameObject FlagSetButton;
    public GameObject FlagCancelButton;
    public GameObject RemoveMarkersButton;

    [Header("Additional Buttons")]
    public GameObject NextArrowButton;
    public GameObject ScanButton;

    [Header("Other")]
    [SerializeField]
    private RectTransform _canvas;

    //private bool _localized = false;
    private bool _arMode;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //UpdateGpsButton.SetActive(false);
        OptionsPopOutButtons.SetActive(false);
        ExpandMapButton.SetActive(false);
        SetMapButtons(false, false);
        NextArrowButton.SetActive(false);

        ExpandMapButton.GetComponent<RectTransform>().sizeDelta = new Vector2(
            _canvas.sizeDelta.x * MapManager.Instance.MiniMapCam.GetComponent<Camera>().rect.width /*0.32f*/,
            _canvas.sizeDelta.y * MapManager.Instance.MiniMapCam.GetComponent<Camera>().rect.height /*0.45f*/);

        _arMode = true;

    }

    public void SetSessionReadyUi()
    {
        //_localized = true;  // TODO: Change this variable name or something. Just a quick fix to place the destination before localization
        //UpdateGpsButton.SetActive(true);
        ExpandMapButton.SetActive(true);
    }

    public void SetLocalizingUi()
    {
        //UpdateGpsButton.SetActive(false);
        ExpandMapButton.SetActive(false);
    }

    public void SetLocalizedUi()
    {
        // TODO: Adjust this later...turns off after scan button click...but needs to turn back on from other triggers
        ////UpdateGpsButton.SetActive(false);

        //_localized = true;


        ExpandMapButton.SetActive(true);

        if (!AppManager.Instance.AllowAutoPOI)
        {
            PlayerArTouchController.Instance.gameObject.SetActive(true);
            DataToggle.SetActive(true);
        }

        //OnChangeModeClick(/*true*/);
    }

    public void FlagSetButtonInteractability(bool val)
    {
        FlagSetButton.GetComponent<Button>().interactable = val;
    }

    public void OnUpdateGpsClick()
    {
        XRSessionManager.GetSession().ForceLocationUpdate();
    }

    public void OnOptionsClick()
    {
        OptionsPopOutButtons.SetActive(!OptionsPopOutButtons.activeSelf);
    }

    public void OnRestartClick()
    {
        //SceneManager.LoadScene("Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitClick()
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
        //Application.Quit();
    }

    // Change between AR and Map mode
    public void OnChangeModeClick(/*bool firstTime = false*/)
    {
        if(AppManager.SessionReady /*_localized*/)
        {
            _arMode = !_arMode;

            if(AppManager.Localized)
            {
                DataToggle.SetActive(_arMode);
                PlayerController.Instance.ToggleConeViewSize();

                RemoveMarkersButton.SetActive(!_arMode);
                PlayerArTouchController.Instance.gameObject.SetActive(_arMode);
            }
            else
            {
                ScanButton.SetActive(_arMode);
                //UpdateGpsButton.SetActive(_arMode);

                if (AppManager.Instance.AllowPathing)
                {
                    FlagPlacementButton.SetActive(!_arMode);
                }
            }

            ArButton.SetActive(!_arMode);
            ExpandMapButton.SetActive(_arMode);
            MapManager.Instance.SwapCams();

            PlayerMapTouchController.Instance.gameObject.SetActive(!_arMode);



            // TODO: Adjust this so remove markers is still avialable when none are true
            //if (AppManager.Instance.AllowOldFlag || AppManager.Instance.AllowPathing)
            //{
            //    FlagPlacementButton.SetActive(!_arMode);
            //    RemoveMarkersButton.SetActive(!_arMode);

            //    // TODO: Adjust this code in the future
            //    if (_arMode)
            //    {
            //        FlagController.Instance.DestroyCurrentFlag();
            //        PlayerArTouchController.Instance.ActivateFullPanel();
            //    }
            //}
        }
        //else
        //{
        //    Debug.LogWarning("Trying to change mode before localization");
        //}
    }

    public void OnRemoveMarkersClick()
    {
        PlayerArTouchController.Instance.RemoveGridMarkers();
        FlagController.Instance.DestroyFlags();
    }

    // This buttons text changes depending on the scenario
    public void OnFlagPlacementClick()  // TODO: Clear Path button too
    {
        //FlagCancelButton.SetActive(true);

        // OLD, AFTER lOCALIZATION SETUP
        if (AppManager.Instance.AllowOldFlag)
        {
            FlagController.Instance.AutoPlaceFlag();
            SetMapButtons(false, true);
        }
        else if(AppManager.Instance.AllowPathing)
        {
            if (!PathingManager.Instance.ActivePath)
            {
                PathingManager.Instance.PrepareDestinationMarker();
                SetMapButtons(false, true);
            }
            else
            {
                // Deletes the path
                NextArrowButton.SetActive(false);
                PathingManager.Instance.DeletePath();
                FlagPlacementButton.GetComponentInChildren<Text>().text = "Set Destination";
            }


            // TODO: Add more options here - the button should change rto reset if active path, so remove path in this scenario
            //PathingManager.Instance.PrepareDestinationMarker();
            //PathingManager.PlacementMode = true;
        }
    }

    public void OnFlagSetClick()
    {
        PathingManager.Instance.ActivePath = true;
        PathingManager.PlacementMode = false;
        SetMapButtons(true, false/*, true*/);
        FlagPlacementButton.GetComponentInChildren<Text>().text = "Remove";

        OnChangeModeClick();

        //FlagSetButton.SetActive(false);
        //FlagCancelButton.SetActive(false);
        //FlagPlacementButton.SetActive(true);




        // OLD PATH SETUP
        //if (AppManager.Instance.AllowOldFlag)
        //{
        //    FlagController.Instance.SetFlag();
        //}
        //else if (AppManager.Instance.AllowPathing)
        //{
        //    PathingManager.Instance.CreatePath();
        //    FlagPlacementButton.GetComponentInChildren<Text>().text = "Clear Path";
        //    FlagSetButtonInteractability(false);
        //}

        //SetMapButtons(true, false/*, true*/);
        //NextArrowButton.SetActive(true);
    }

    public void OnFlagCancelClick()
    {
        if (AppManager.Instance.AllowOldFlag)
        {
            PlayerArTouchController.Instance.gameObject.SetActive(false);
            FlagController.Instance.DestroyCurrentFlag();
        }
        else if (AppManager.Instance.AllowPathing)
        {
            PathingManager.Instance.DeletePath();
            FlagSetButtonInteractability(false);
            //PathingManager.PlacementMode = false;
        }

        SetMapButtons(true, false/*, true*/);
    }

    public void OnNextPathArrowButtonClick()
    {
        //PathingManager.Instance.SetNextArrow();
        PathingManager.Instance.SkipToNextArrow();
    }

    public void ButtonInteractibility(bool val)
    {
        ExpandMapButton.GetComponent<Button>().interactable = val;
        ArButton.GetComponent<Button>().interactable = val;
        DataToggle.GetComponent<Toggle>().interactable = val;

        FlagSetButton.GetComponent<Button>().interactable = val;
        FlagCancelButton.GetComponent<Button>().interactable = val;
        FlagPlacementButton.GetComponent<Button>().interactable = val;
        RemoveMarkersButton.GetComponent<Button>().interactable = val;
    }

    private void SetMapButtons(bool initialMapMenu, bool flagEdit/*, bool arButton*/)
    {
        if (AppManager.Localized)
        {
            RemoveMarkersButton.SetActive(initialMapMenu);
        }
        else
        {
            FlagPlacementButton.SetActive(initialMapMenu);
        }

        FlagSetButton.SetActive(flagEdit);
        FlagCancelButton.SetActive(flagEdit);

        ArButton.SetActive(initialMapMenu);
    }
}