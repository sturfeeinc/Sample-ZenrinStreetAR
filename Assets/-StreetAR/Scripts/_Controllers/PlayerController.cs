using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Package.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Transform MapPlayer;

    [HideInInspector]
    public bool NeedToRelocalize = false;

    [Header("Map Sprites")]
    [SerializeField]
    private GameObject _fullConeViewSprite;
    [SerializeField]
    private GameObject _halfConeViewSprite;

    [Header("Scripts")]
    [SerializeField]
    private LocalizationManager _localizationManager;

    [Header("Testing/Debug")]
    public Text UpdateText;
    public Text FUText;

    private bool _mapView = false;

    //private bool _localized = false;
    //private Vector3 _lastLocalizedPos;

    //private float _totalDistToRelocalize = 15;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _fullConeViewSprite.SetActive(false);

        GetComponent<AutoPoiController>().enabled = false;
    }

    // If signal is given to relocalize, waits for user to set the camera upright and then triggers relocalization call
    private void Update()
    {
        //UpdateText.text = "Position: " + transform.position;
        //UpdateText.text += "\nOrientation: " + transform.rotation.eulerAngles;

        // TODO: Fix this situation
        if (AppManager.Instance.AllowRadiusRelocalize && AppManager.Localized /*_localized*/)
        {
            //RelocalizationManager.Instance.RadiusRelocalizeCheck();
            if(RelocalizationManager.Instance.PastRelocDist())
            {
                PlayerArTouchController.Instance.Relocalize = true;
            }

            //float moveDist = Vector3.Distance(_lastLocalizedPos, transform.position);

            //UpdateText.text = "Distance: " + moveDist;
            //UpdateText.text += "\n(Relocalize at 5+)";

            //if (moveDist > _totalDistToRelocalize)
            //{
            //    PlayerArTouchController.Instance.Relocalize = true;
            //}
        }
        else if (AppManager.Instance.AllowPathing && NeedToRelocalize)
        {
            if (transform.rotation.x < 0.5f && transform.rotation.x > -16)
            {
                ToastManager.Instance.HideToast();
                NeedToRelocalize = false;

                //_localizationManager.PerformLocalization();
                RelocalizationManager.Instance.PerformRelocalization(true);

            }
            else
            {
                ToastManager.Instance.ShowToast("Please hold camera upright");
            }
        }

        // TODO: Move map representation control to here
    }


    public void Initialize()
    {
        _fullConeViewSprite.SetActive(true);
        //_lastLocalizedPos = transform.position;
        //_localized = true;
        GetComponent<Collider>().enabled = true;

        //if (AppManager.Instance.AllowRadiusRelocalize || AppManager.Instance.AllowPathing)
        //{
        //    SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;
        //}

        if (AppManager.Instance.AllowAutoPOI)
        {
            GetComponent<AutoPoiController>().enabled = true;
        }
    }

    public void ToggleConeViewSize()
    {
        bool val = _fullConeViewSprite.activeSelf;
        _fullConeViewSprite.SetActive(!val);
        _halfConeViewSprite.SetActive(val);
    }
}
