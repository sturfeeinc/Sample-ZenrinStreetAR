using UnityEngine;

// TODO: later this can be set to use an Animator with States

public class HologramController : MonoBehaviour
{
    [SerializeField]
    private Transform _mainTransform;
    [SerializeField]
    private Transform _glowTransform;
    [SerializeField]
    private float _scaleSpeed = 3.0f;

    private bool _showHologram = false;
    private Vector3 _initialScale = Vector3.zero;
    private Vector3 _initialGlowScale = Vector3.zero;

    private void Start()
    {
        if(_mainTransform == null)
        {
            _mainTransform = transform;
        }

        if(_glowTransform != null)
        {
            _initialGlowScale = _glowTransform.localScale;
            _glowTransform.localScale = Vector3.zero;
        }

        _initialScale = _mainTransform.localScale;

        _mainTransform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_showHologram)
        {
            // show the hologram (play "show" animation)
            if(_mainTransform.localScale.magnitude < _initialScale.magnitude)
            {
                _mainTransform.localScale += (_scaleSpeed * Vector3.one * Time.deltaTime);
            }
            else
            {
                _mainTransform.localScale = _initialScale;
            }

            // GLOW
            if(_glowTransform != null)
            {
                if (_glowTransform.localScale.magnitude < _initialGlowScale.magnitude)
                {
                    _glowTransform.localScale += (_scaleSpeed * Vector3.one * Time.deltaTime);
                }
                else
                {
                    _glowTransform.localScale = _initialGlowScale;
                }
            }            
        }
        else
        {
            // hide the hologram (play "hide" animation)
            if (_mainTransform.localScale.magnitude > (0.1f * Vector3.one.magnitude) && _mainTransform.localScale.x > 0)
            {
                _mainTransform.localScale -= (_scaleSpeed * Vector3.one * Time.deltaTime);
            }
            else
            {
                _mainTransform.localScale = Vector3.zero;
            }

            // GLOW
            if (_glowTransform != null)
            {
                if (_glowTransform.localScale.magnitude > (0.1f * Vector3.one.magnitude) && _glowTransform.localScale.x > 0)
                {
                    _glowTransform.localScale -= (_scaleSpeed * Vector3.one * Time.deltaTime);
                }
                else
                {
                    _glowTransform.localScale = Vector3.zero;
                }
            }                
        }
    }

    public void ShowHologram()
    {
        _showHologram = true;
    }

    public void HideHologram()
    {
        _showHologram = false;
    }
}
