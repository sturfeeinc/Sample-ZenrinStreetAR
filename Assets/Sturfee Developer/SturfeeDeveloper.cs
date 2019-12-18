using UnityEngine;
using UnityEngine.UI;

using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Package.Utilities;

namespace Sturfee.Internal.Developer
{
    public class SturfeeDeveloper : MonoBehaviour
    {
        [SerializeField]
        private GameObject _reScanButton;
        private bool _localized;

        private void Awake()
        {
            SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        }

        private void OnDestroy()
        {
            SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        }

        public void ReScan()
        {
            XRSessionManager.GetSession().PerformLocalization();
        }

        private void OnLocalizationSuccessful()
        {
            if(!_localized)
            {
                _localized = true;
            }

            _reScanButton.SetActive(true);
        }
    }
}
