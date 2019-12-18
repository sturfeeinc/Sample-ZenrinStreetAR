using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class ToastManager : MonoBehaviour {

        [SerializeField]
        private Canvas _displayCanvas;

        public static ToastManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = Instantiate(Resources.Load<GameObject>("Prefabs/ToastManager")).GetComponent<ToastManager>();
                }


                return _instance;
            }
        }

        private static ToastManager _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }

            _instance = this;
//    		DontDestroyOnLoad(gameObject);
        }
              

        public void ShowToastTimed(string toastMessage, float durationInSeconds = 2.0f)
        {
            _displayCanvas.gameObject.SetActive(true);
            _displayCanvas.gameObject.GetComponentInChildren<Text>().text = toastMessage;
            StartCoroutine(HideToast(durationInSeconds));
        }

		public void ShowToast(string toastMessage)
		{
			_displayCanvas.gameObject.SetActive(true);         
            _displayCanvas.gameObject.GetComponentInChildren<Text>().text = toastMessage;        	
		}

        public void HideToast()
        {
            _displayCanvas.gameObject.SetActive(false);
        }
//
        private IEnumerator HideToast(float duration)
        {
            yield return new WaitForSeconds(duration);

            _displayCanvas.gameObject.SetActive(false);
            _displayCanvas.gameObject.GetComponentInChildren<Text>().text = "";
        }             

        private void CreateToastGO()
        {
            if(_instance == null)
            {

            }
        }
    }
}