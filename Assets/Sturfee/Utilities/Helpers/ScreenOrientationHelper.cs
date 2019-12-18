using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class ScreenOrientationHelper : MonoBehaviour {

        public bool OrientationChanged = false;

        public ScreenOrientation LastOrientation = ScreenOrientation.Unknown;

		public static ScreenOrientationHelper Instance
		{
			get
			{
				return _instance;
			}
		}

		private static ScreenOrientationHelper _instance;

		private void Awake()
		{
			if (_instance != null)
			{
				Destroy(_instance);
			}

			_instance = this;
//			DontDestroyOnLoad(gameObject);
		}

        private void Update()
        {
            if (Screen.orientation != LastOrientation)
            {
                OrientationChanged = true;
//                Debug.Log("ScreenOrientationHelper : ORIENTATION CHANGED");
                LastOrientation = Screen.orientation;
            }
            else
            {
                OrientationChanged = false;
            }
        }

    }
}