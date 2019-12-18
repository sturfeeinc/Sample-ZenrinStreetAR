using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants.Enums;
using Sturfee.Unity.XR.Core.Exceptions;
//using Sturfee.Unity.XR.Core.Events;


public class CustomMultiframeManager : MonoBehaviour{//, IScanManager {

	public static CustomMultiframeManager Instance;

	public static readonly int  Angle = 50;
	public static readonly int TargetCount = 3;

    [Header("UI")]
    [SerializeField]
    private GameObject _scanButton;
	[SerializeField]
	private GameObject _scanAnimation;
	[SerializeField]
	private GameObject _gazeTarget;
	[SerializeField]
	private GameObject _cursor;

	[Header("Other")]
	[SerializeField]
	private LayerMask _circleCheckLayerMask;

	private List<GameObject> _gazeTargets;

	private int _multiframeRequestId;
	private int _centerYaw;
	private int _targetCount;
	private bool _isScanning;
	private bool _resetCalled;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		_targetCount = TargetCount - 1;
	}

	public void OnScanButtonClick()
	{
		_isScanning = true;
		StartCoroutine(MultiframeCallAsync());
	}

	public void PlayScanAnimation()
	{
		if(_scanAnimation != null)
		{
			_scanAnimation.SetActive(true);
		}
	}

	public void StopScanAnimation()
	{
		if (_scanAnimation != null)
		{
			_scanAnimation.SetActive(false);
		}
	}

	public void ResetMultiframe()
	{
		_resetCalled = true;

        _scanButton.SetActive(true);
		//FindObjectOfType<Sturfee.Unity.XR.Package.Utilities.AlignmentManager>().ScanButton.SetActive(true);
	}

	public List<GameObject> GetGazeTargets()
	{
		return _gazeTargets;
	}

	public void OnMultiframeButtonClick()
	{
		_isScanning = true;
        GetComponent<GazeTargetArrow>().enabled = true;
		StartCoroutine(MultiframeCallAsync());
	}

	private IEnumerator MultiframeCallAsync()
	{
		//Setup variables for starting multiframe requests
		SetupMultiframe();

		//Create gaze targets
		CreateGazeTargets();

		//wait till all the requests are complete
		while (_multiframeRequestId != TargetCount)
		{
			yield return null;
		}

		_cursor.SetActive(false);

		_isScanning = false;
        GetComponent<GazeTargetArrow>().TurnOff();
        //GetComponent<GazeTargetArrow> ().enabled = false;
    }

	private void SetupMultiframe()
	{
		_resetCalled = false;

		_gazeTargets = new List<GameObject>();

		_cursor.SetActive(true);
		_centerYaw = (int)XRSessionManager.GetSession().GetXRCameraOrientation().eulerAngles.y;
		_multiframeRequestId = 0;

		AddToMultiframeLocalizationCall();
	}

	private void CreateGazeTargets()
	{

		for (int i = 1; i <= _targetCount; i++)
		{
            Vector3 dir;

			var gaze = Instantiate(_gazeTarget);
			dir = Quaternion.AngleAxis(Angle * i, Vector3.up) * (XRSessionManager.GetSession().GetXRCameraOrientation() * Vector3.forward);
			gaze.transform.position = XRSessionManager.GetSession().GetXRCameraPosition() + (dir * 10.0f);
			gaze.transform.LookAt(GameObject.FindGameObjectWithTag("XRCamera").transform);

			gaze.name = "Gaze target (" + (Angle * i) + ")";

			_gazeTargets.Add(gaze);

			gaze.SetActive (false);

			//			StartCoroutine (CheckCirclePlacement (gaze));
			//			StartCoroutine(CheckYaw(gaze, Angle * i));

		}

        StartCoroutine(CheckCirclePlacement(_gazeTargets[0]));
	}

	private IEnumerator CheckCirclePlacement(GameObject target)
	{
		_gazeTargets [_multiframeRequestId - 1].SetActive (true);
		GetComponent<GazeTargetArrow> ().SetTarget (target.transform);

		bool alignedCircle = false;
		RaycastHit hit;

		while(!alignedCircle)
		{

			if (Physics.Raycast (PlayerController.Instance.transform.position, PlayerController.Instance.transform.forward, out hit, 1000, _circleCheckLayerMask))
			{
				//				print ("SENT Target: " + target.transform.name + ", " + target.transform.GetInstanceID());
				//				print ("HIT Target: " + hit.transform.parent.parent.name + ", " + hit.transform.parent.parent.GetInstanceID());

				if (hit.transform.Equals (target.transform))
				{
					alignedCircle = true;

					//					print ("!* CHECK CIRCLE PLACEMENT: Destroy");

					//hit.transform.GetComponent<GazeTarget> ().TargetAlignedSuccess ();
					//					Destroy (target);

					AddToMultiframeLocalizationCall ();
				}
				else
				{
					//					print ("!* HIT CIRCLE, BUT NOT EQUAL");
					yield return null;
				}

			}
			else
			{
				yield return null;
			}
		}


		////		Ray ray = PlayerController.Instance.transform.forward;
		//		RaycastHit hit;
		//
		//		if(Physics.Raycast(PlayerController.Instance.transform, PlayerController.Instance.transform.forward, out hit, 1000, _circleCheckLayerMask))
		////		if (Physics.Raycast ( ray, out hit, 1000, _circleCheckLayerMask))
		//		{
		//
		//		}
		//		else
		//		{
		//
		//		}
	}

	private IEnumerator CheckYaw(GameObject target, int offsetAngle)
	{

		int targetYaw = (int)_centerYaw + offsetAngle;
		if(targetYaw < 0)
		{
			targetYaw = 360 + targetYaw;
		}

		while(((int)XRSessionManager.GetSession().GetXRCameraOrientation().eulerAngles.y != targetYaw % 360) && !_resetCalled)
		{
			yield return null;
		}

//		print ("!* CHECK YAW: Destroy");

		Destroy(target);

		AddToMultiframeLocalizationCall();
	}

	private void AddToMultiframeLocalizationCall()
	{
		_multiframeRequestId++;



        //If reset is called, send -1 otherwise the count
        //XRSessionManager.GetSession().PerformLocalization((_resetCalled) ? -1 : TargetCount);
        PerformLocalization((_resetCalled) ? -1 : TargetCount);

		if (_multiframeRequestId > 1 && _multiframeRequestId < TargetCount)
		{
			// Activate the next gaze target
			StartCoroutine( CheckCirclePlacement(_gazeTargets[_multiframeRequestId - 1]) );
		}

	}

    public void PerformLocalization(int requestCount = 1)
    {
        try
        {
            XRSessionManager.GetSession().PerformLocalization();// .PerformLocalization(requestCount);
        }


        catch (InvalidRequestLengthException e)
        {
            Debug.Log(e.Message);
            //ScreenMessageController.Instance.SetText
            ScreenMessageController.Instance.SetText("[RequestError] :: Request length is invalid", 5);
        }

        catch (PitchRequestException e)
        {
            Debug.Log(e.Message);
            ScreenMessageController.Instance.SetText("[RequestError] :: Please look straight up while scanning", 5);
        }

        catch (YawRequestException e)
        {
            Debug.Log(e.Message);
            ScreenMessageController.Instance.SetText("[RequestError] :: Yaw difference between frames is incorrect", 5);
        }
        catch (UserMovedRequestException e)
        {
            Debug.Log(e.Message);
            ScreenMessageController.Instance.SetText("[RequestError] :: Please do not move while scannning.", 5);
        }

        catch (RollRequestException e)
        {
            Debug.Log(e.Message);
            ScreenMessageController.Instance.SetText("[RequestError] :: Please do not tilt your phone while scannning.", 5);
        }
    }


}
