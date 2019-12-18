using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PhotoBillboardController : MonoBehaviour
{
    public RawImage BillboardImage;
    public RawImage BackgroundImage;

    [SerializeField]
    private float _baseScale;

    private bool _photoMode = true;
    private float _curPicWidth;
    private float _curPicHeight;
    private Texture _curPicTexture;

    private bool _sideBoard = false;
    private Vector3 _connectPoint;

    private RectTransform _canvasRectTrans;

    private float _placedScale;

    private void Awake()
    {
        _canvasRectTrans = GetComponent<RectTransform>();
    }

    void Start()
    {
        //gameObject.SetActive(false);
    }

    public void PlaceBillboard(RaycastHit hitPoint, Texture picTexture, float picWidth, float picHeight)
    {

        float lookAngle = Vector3.SignedAngle(hitPoint.normal, PlayerController.Instance.transform.position - hitPoint.point, Vector3.up);

        //      print("ANGLE: " + lookAngle);

        // Good angle, keep billboard flat on wall
        if (Mathf.Abs(lookAngle) < 45)
        {
            _canvasRectTrans.pivot = new Vector2(0.5f, 0.5f);
            BackgroundImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            BackgroundImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            BackgroundImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _sideBoard = false;
        }  // Billboard needs to pop out from the left
        else if (lookAngle >= 45)
        {

            _canvasRectTrans.pivot = new Vector2(1, 0.5f);
            BackgroundImage.rectTransform.pivot = new Vector2(1, 0.5f);
            BackgroundImage.rectTransform.anchorMin = new Vector2(1f, 0.5f);
            BackgroundImage.rectTransform.anchorMax = new Vector2(1f, 0.5f);

            _sideBoard = true;
        }  // Billboard needs to pop out from the right
        else  // lookAngle <= -45
        {
            _canvasRectTrans.pivot = new Vector2(0, 0.5f);
            BackgroundImage.rectTransform.pivot = new Vector2(0, 0.5f);
            BackgroundImage.rectTransform.anchorMin = new Vector2(0, 0.5f);
            BackgroundImage.rectTransform.anchorMax = new Vector2(0, 0.5f);
            _sideBoard = true;
        }

        BackgroundImage.rectTransform.localPosition = Vector3.zero;

        _connectPoint = hitPoint.point;

        transform.position = hitPoint.point;
        transform.rotation = Quaternion.LookRotation(hitPoint.normal);

        if (_sideBoard)
        {
            if (lookAngle <= -45)
            {
                transform.Rotate(Vector3.down * 90);
            }
            else if (lookAngle >= 45)
            {
                transform.Rotate(Vector3.up * 90);
            }
        }
        else
        {
            transform.position += transform.forward;
        }

        //      MapRepresentation.rotation = Quaternion.Euler(Vector3.right * 90);

        ChangePicture(picTexture, picWidth, picHeight);

    }

    public void ChangePicture(Texture picTexture, float picWidth, float picHeight)
    {
        _curPicWidth = picWidth;
        _curPicHeight = picHeight;
        _curPicTexture = picTexture;

        float distance = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);


        float scale = _baseScale * distance * (distance / Mathf.Pow(distance, 1.2f));
        _placedScale = scale;


        //      PlayerArTouchController.Instance.PhotoNavUi.SetActive (true);
        BillboardImage.texture = picTexture;
        ResizeAspectRatio(_curPicWidth, _curPicHeight);
    }

    private void ResizeAspectRatio(float width, float height)
    {
        Vector2 backgroundSize = new Vector2(_placedScale, _placedScale); // newSize;

        _canvasRectTrans.sizeDelta = backgroundSize;

        Rect newRect = BillboardImage.rectTransform.rect;
        Vector2 newSize = backgroundSize;
        float borderAmount = backgroundSize.x * 0.1f;

        if (width > height)
        {
            newSize.y *= height / width;
            //borderAmount = width * 0.1f;
        }
        else
        {
            newSize.x *= width / height;
            //borderAmount = height * 0.1f;

        }

        BillboardImage.rectTransform.sizeDelta = newSize;

        BackgroundImage.rectTransform.sizeDelta = new Vector2(newSize.x + borderAmount, newSize.y + borderAmount);
        //BackgroundImage.rectTransform.rect.size

        if (_sideBoard)
        {
            transform.position = _connectPoint;
        }
    }

}
