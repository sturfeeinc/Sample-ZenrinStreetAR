using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants;

public class LocalizationUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _scanUI;
    [SerializeField]
    private GameObject _scanFX;
    [SerializeField]
    private GameObject _scanButton;
    [SerializeField]
    private GameObject _stopScanButton;
    [SerializeField]
    private GameObject _scanAnimation;

    private GameObject _gazeTarget;
    private GameObject[] _gazetargets;
    private LocalizationManager _localizationManager;

    private int _gazeTargetSize = 75;
    private int _scanCursorSize;

    private float _padding;
    private float _factor;

    private int _startYaw;
    private bool _resetCalled;

    private void Start()
    {
        _localizationManager = GetComponent<LocalizationManager>();
        _gazeTarget = Resources.Load<GameObject>("UI/Prefabs/Gaze Target");
        _scanCursorSize = (int)(_gazeTargetSize * 0.75f);
        _padding = _scanUI.GetComponent<RectTransform>().rect.width * 0.2f;
    }

    public void ShowScanUI()
    {
        _scanUI.SetActive(true);
        _stopScanButton.SetActive(true);
        _scanFX.SetActive(true);
    }

    public void HideScanUI()
    {
        _scanUI.SetActive(false);
        _stopScanButton.SetActive(false);
        _scanFX.SetActive(false);
    }

    public void SetScanUIVisisbility(bool visible)
    {
        _scanUI.GetComponent<CanvasGroup>().alpha = (visible ? 1 : 0);
        _scanUI.GetComponent<CanvasGroup>().blocksRaycasts = visible;
    }

    public void ShowScanButton()
    {
        if (_scanButton != null)
        {
            _scanButton.SetActive(true);
        }
    }

    public void HideScanButton()
    {
        if (_scanButton != null)
        {
            _scanButton.SetActive(false);
        }
    }

    public void PlayScanLoadingAnimation()
    {
        if (_scanAnimation != null)
        {
            _scanAnimation.SetActive(true);
        }
    }

    public void StopScanLoadingAnimation()
    {
        if (_scanAnimation != null)
        {
            _scanAnimation.SetActive(false);
        }
    }             

    public void StartScanning()
    {
        float width = _scanUI.GetComponent<RectTransform>().rect.width;
        _factor = (width - (_padding * 2)) / (ScanProperties.TargetCount - 1);
        _resetCalled = false;
        _scanFX.SetActive(true);

        //Capture first request
        XRSessionManager.GetSession().PerformLocalization();

        //Set Yaw to start from where our current Yaw is 
        _startYaw = (int)XRSessionManager.GetSession().GetXRCameraOrientation().eulerAngles.y;

        //Create Scan UI and get reference to scan cursor
        GameObject scanCursor = CreateScanCaptureUI();

        //Move scan cursor based on yaw and capture localization requests
        StartCoroutine(CheckGazeTargetHit(scanCursor.GetComponent<RectTransform>()));
    }              

    public void StopScanning()
    {
        _resetCalled = true;
    }
     
    private GameObject CreateScanCaptureUI()
    {
        int width = (int)_scanUI.GetComponent<RectTransform>().rect.width;

        //Create Scan-Line
        GameObject scanLine = new GameObject("Scan-Line");
        scanLine.transform.parent = _scanUI.transform;
        scanLine.AddComponent<Image>().color = Color.cyan;
        scanLine.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        scanLine.GetComponent<RectTransform>().sizeDelta = new Vector2(width - _padding * 2, 1);
        scanLine.transform.localScale = new Vector3(1, (float)width / Screen.width, 1);


        //Create Gaze targets
        _gazetargets = new GameObject[ScanProperties.TargetCount];
        float multiplier = (width - _padding * 2) / (ScanProperties.TargetCount - 1);

        for (int i = 1; i < _gazetargets.Length; i++)
        {
            _gazetargets[i] = Instantiate<GameObject>(_gazeTarget);
            _gazetargets[i].transform.parent = _scanUI.transform;
            _gazetargets[i].GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            _gazetargets[i].GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
            _gazetargets[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(i * multiplier + _padding, 0);
            _gazetargets[i].GetComponent<RectTransform>().sizeDelta = new Vector2(_gazeTargetSize, _gazeTargetSize);
        }

        //Create Scan Cursor
        GameObject scanCursor = new GameObject("Scan Cursor").AddComponent<Image>().gameObject;
        scanCursor.transform.parent = _scanUI.transform;
        scanCursor.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/UI_Gaze-Cursor");
        scanCursor.GetComponent<Image>().color = Color.white;
        scanCursor.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        scanCursor.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
        scanCursor.GetComponent<RectTransform>().sizeDelta = new Vector2(_scanCursorSize, _scanCursorSize);
        scanCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(_padding, 0);

        return scanCursor;
    }

    private IEnumerator CheckGazeTargetHit(RectTransform scanCursorRT)
    {
        int i = 1;
        var cursorStart = _padding;
        float end = _factor * (ScanProperties.TargetCount - 1) + _padding;

        bool done = false;
        while (!_resetCalled && !done)
        {
            float cursorPos = GetCursorPosition();

            if (cursorPos < cursorStart)
            {
                //Don't move cursor to left
            }
            else
            {
                scanCursorRT.anchoredPosition = new Vector2(cursorPos, 0);
            }

            float cursor = scanCursorRT.anchoredPosition.x;
            float target = i * _factor + _padding;

            if ((target - cursor) < 1)
            {
                Destroy(_gazetargets[i++]);
                XRSessionManager.GetSession().PerformLocalization();
            }

            done |= (i >= _gazetargets.Length && scanCursorRT.anchoredPosition.x > end);

            yield return null;
        }

        //If we came out of while loop because "Stop Scanning" was called
        if (_resetCalled)
        {
            XRSessionManager.GetSession().StopScan();
        }

        HideScanUI();
        ResetScanUI();
    }

    private float GetCursorPosition()
    {
        float width = _scanUI.GetComponent<RectTransform>().rect.width;
        int yaw = (int)XRSessionManager.GetSession().GetXRCameraOrientation().eulerAngles.y;

        int yawDiff = yaw - _startYaw;
        int absYawDiff = Mathf.Abs(yawDiff);

        if (absYawDiff > 180)
        {
            yawDiff = yawDiff > 0 ? -(360 - absYawDiff) : 360 - absYawDiff;
        }

        //If our capture range goes above 180
        float captureRange = (ScanProperties.TargetCount - 1) * ScanProperties.YawAngle;
        if (yawDiff < 0 && captureRange > 180)
        {
            if (yawDiff > -180 && yawDiff <= captureRange - 360 + 5)    // + 5 is added for sanity just in case we want cursorPos beyond last gaze target
            {
                yawDiff += 360;
            }
        }

        float multiplier = _factor / ScanProperties.YawAngle;

        return (yawDiff * multiplier) + _padding;
    }

    private void ResetScanUI()
    {
        foreach (Transform child in _scanUI.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
