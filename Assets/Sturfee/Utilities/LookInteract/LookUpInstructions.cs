using UnityEngine;

public class LookUpInstructions : MonoBehaviour
{
    public GameObject LookUpPanel;

    private void Awake()
    {
        LookUpTrigger.OnUserLookedUp += HandleUserLookedUp;
    }

    private void OnDestroy()
    {
        LookUpTrigger.OnUserLookedUp -= HandleUserLookedUp;
    }

    public void ShowInstructions()
    {
        LookUpPanel.SetActive(true);
    }

    private void HandleUserLookedUp()
    {
        LookUpPanel.SetActive(false);
    }
}
