using UnityEngine;
using UnityEngine.UI;

public class EnableLocalization : MonoBehaviour
{
    public Button LocalizeButton;

    private void Awake()
    {
        LookUpTrigger.OnUserLookedUp += HandleOnLookUpComplete;

        LocalizeButton.interactable = false;
    }

    private void OnDestroy()
    {
        LookUpTrigger.OnUserLookedUp -= HandleOnLookUpComplete;
    }

    private void HandleOnLookUpComplete()
    {
        LocalizeButton.interactable = true;
    }
}
