using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Sturfee.Unity.XR.Core.Session;

public class SessionLoaderScreen : MonoBehaviour {

    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private GameObject _loader;
    [SerializeField]
    private Slider _progressSlider;
    [SerializeField]
    private Text _progressText;
    [SerializeField]
    private Text _statusText;

    public static SessionLoaderScreen Instance
    {
        get
        {
            if (_instance == null)
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/SessionLoaderScreen")).GetComponent<SessionLoaderScreen>();
            }
            return _instance;
        }
    }

    private static SessionLoaderScreen _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _instance = this;
    }

    private void Update()
    {
        if(XRSessionManager.GetSession() != null)
        {
            _progressText.text = ((int)XRSessionManager.GetSession().Progress).ToString() + "%";
            _progressSlider.value = XRSessionManager.GetSession().Progress;

            _statusText.text = XRSessionManager.GetSession().Status;
        }
    }

    public void ShowLoader()
    {
        _loader.SetActive(true);
    }

    public void ShowLoaderSmooth()
    {
        StartCoroutine(FadeIn());
    }

    public void HideLoader()
    {
        _loader.SetActive(false);
    }

    public void HideLoaderSmooth()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            _canvasGroup.alpha = i;
            yield return null;
        }

        _loader.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        _loader.SetActive(true);

        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            _canvasGroup.alpha = i;
            yield return null;
        }
    }

}
