using UnityEngine;

using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Constants;

/// <summary>
/// This script raises an event when the user looks upward (to view AR content and buildings)
/// </summary>
public class LookUpTrigger : MonoBehaviour
{
    #region EVENTS
    public delegate void HandleUserLookedUp();
    public static event HandleUserLookedUp OnUserLookedUp;
    #endregion

    #region PUBLIC FIELDS
    [Header("Configuration")]
    public bool IsEnabled = true;
    [Tooltip("Min angle with respect to UP to trigger")]
    public float ViewAngle = 80.0f;

    [Header("UI")]
    public bool IsUiEnabled = true;
    public Canvas UiCanvas;
    public RectTransform Cursor;
    public RectTransform Target;
    public RectTransform HelperArrow;

    [Header("Dependencies")]
    public Camera XrCamera;
    #endregion

    #region PUBLIC PROPERTIES
    #endregion

    #region PRIVATE SERIALIZED FIELDS
    #endregion

    #region PRIVATE VARIABLES
    private bool _sessionReady = false;
    #endregion

    #region UNITY OVERRIDES
    private void Awake()
    {
        SturfeeEventManager.Instance.OnSessionReady += HandleSessionReady;
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnSessionReady -= HandleSessionReady;
    }

    private void Start()
	{        
        UiCanvas.gameObject.SetActive(false);
        if(XrCamera == null)
        {
            XrCamera = GameObject.FindWithTag(SturfeeObjects.XRCamera).GetComponent<Camera>();
        }
    }
	
	private void Update()
	{
        if (!_sessionReady)
        {
            return;
        }

        if (IsEnabled)
        {
            if (IsLookingUp())
            {
                //Debug.Log("User is looking up");
                if (OnUserLookedUp != null)
                {
                    OnUserLookedUp();
                }
            }

            if (IsUiEnabled)
            {
                UiCanvas.gameObject.SetActive(true);
                SetTargetPosition();
            }
        }
	}

    #endregion

    #region PUBLIC METHODS
    #endregion

    #region PRIVATE METHODS
    private void HandleSessionReady()
    {
        _sessionReady = true;
    }

    private bool IsLookingUp()
    {
        if(Vector3.Angle(XrCamera.transform.forward, Vector3.up) < ViewAngle)
        {
            UiCanvas.gameObject.SetActive(false);
            IsUiEnabled = false;
            return true;
        }

        return false;
    }

    private void SetTargetPosition()
    {
        var radialPosition = (180.0f - Vector3.Angle(XrCamera.transform.forward, Vector3.down) - ViewAngle);

        var positionY = (Screen.height * Mathf.Sin(radialPosition * Mathf.Deg2Rad));

        if (positionY > (Screen.height / 2))
        {
            HelperArrow.gameObject.SetActive(true);
        }
        else
        {
            HelperArrow.gameObject.SetActive(false);
        }

        positionY = Mathf.Clamp(positionY, - Screen.height / 2, Screen.height / 2);

        Target.anchoredPosition = new Vector2(0, positionY);
    }
    #endregion
}
