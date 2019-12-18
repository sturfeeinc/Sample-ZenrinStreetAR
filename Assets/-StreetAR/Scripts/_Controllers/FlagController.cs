using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script deals with the old flag setup (User places flags anywhere in the environment and it pulses)
public class FlagController : MonoBehaviour {

    public static FlagController Instance;

    public static bool PlacementMode = false;
    public static bool Pulsating = false;

    public Transform FlagPrefab;

    [HideInInspector]
    public Transform CurrentFlag;

    [Header("Other")]
    [SerializeField]
    private Transform _flagArExpandingCircle;

    private LayerMask _flagPlacementLayerMask;
    private List<GameObject> _flagsList;
    private bool _newFlagPlaced = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
		_flagPlacementLayerMask |= (1 << LayerMask.NameToLayer("sturfeeTerrain"));
        _flagPlacementLayerMask |= (1 << LayerMask.NameToLayer("sturfeeBuilding"));

        _flagsList = new List<GameObject>();
    }

    public void AutoPlaceFlag()
    {
        CurrentFlag = Instantiate(FlagPrefab).transform;
        CurrentFlag.parent = this.transform;

        Transform _player = PlayerController.Instance.transform;
        Vector3 flagPos = (_player.position + (_player.forward * 25) + (_player.right * 7))
             - MapManager.Instance.HalfMapCam.position;
        Ray ray = new Ray(MapManager.Instance.HalfMapCam.position, flagPos);
        PlayerArTouchController.Instance.ActivateHalfPanel(CurrentFlag);

        PlacementMode = true;
        FlagPlacementRaycast(ray);
    }

    public void FlagPlacementTouchPos(Vector2 touchPos)
    {
        Ray ray = MapManager.Instance.HalfMapCam.GetComponent<Camera>().ScreenPointToRay(touchPos);
        FlagPlacementRaycast(ray);
    }

    public void FlagPlacementRaycast(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, _flagPlacementLayerMask))
        {
            CurrentFlag.position = hit.point;
            CurrentFlag.rotation = Quaternion.LookRotation(hit.normal);

            if (Mathf.Abs(hit.normal.y) > Mathf.Abs(hit.normal.x))
            {
                Vector3 toPlayerDir = (PlayerController.Instance.transform.position - CurrentFlag.position);
                toPlayerDir.y = 0;

                float angle = Vector3.SignedAngle(CurrentFlag.right, toPlayerDir, CurrentFlag.forward);
                CurrentFlag.Rotate(Vector3.forward * (angle));
            }
        }
    }

    public void DestroyCurrentFlag()
    {
        _newFlagPlaced = false;
        Pulsating = false;

        PlacementMode = false;
        if (CurrentFlag != null)
        {
            Destroy(CurrentFlag.gameObject);
            CurrentFlag = null;
        }
    }

    public void DestroyFlags()
    { 
        _newFlagPlaced = false;
        Pulsating = false;

        for (int i = 0; i < _flagsList.Count; i++)
        {
            Destroy(_flagsList[i]);
        }
        _flagsList = new List<GameObject>();
    }

    public void SetFlag()
    {
        if (Pulsating)
        {
            _newFlagPlaced = true;
        }

        Pulsating = true;

        CurrentFlag.GetComponentInChildren<MapIconUpdater>().enabled = false;

        _flagArExpandingCircle.position = CurrentFlag.position;
        _flagArExpandingCircle.rotation = Quaternion.LookRotation(Vector3.up);
        _flagArExpandingCircle.GetComponent<MapPlacementAnimator>().PlacementAnimation(true);

        // TODO: Refactor this script too
        ArExpandingCircle.Instance.Activate(CurrentFlag.position, CurrentFlag.rotation, true);

        _flagsList.Add(CurrentFlag.gameObject);

        CurrentFlag = null;
        PlacementMode = false;

        PlayerArTouchController.Instance.gameObject.SetActive(false);

        Invoke("PulsateFlag", 2);
    }

    private void PulsateFlag()
    {
        if (_newFlagPlaced)
        {
            _newFlagPlaced = false;
            return;
        }

        if (Pulsating)
        {
            ArExpandingCircle.Instance.GetComponent<MapPlacementAnimator>().PlacementAnimation(true);
            Invoke("PulsateFlag", 2);
        }
    }
}
