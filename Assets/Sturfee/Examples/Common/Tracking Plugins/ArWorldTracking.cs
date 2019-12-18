using System.Collections;

using UnityEngine;

public class ArWorldTracking : MonoBehaviour {

    public static float Theta { get; private set; }
    public static float TrueHeading { get; private set; }

    private void Awake()
    {
        Input.compass.enabled = true;
    } 

    private IEnumerator Start()
    {
        while(Input.compass.trueHeading == 0)
        {
            yield return null;
        }

        TrueHeading = Input.compass.trueHeading;
        Theta = -((Mathf.PI * TrueHeading) / 180);        
    }


    private void OnDestroy()
    {
        Theta = TrueHeading = 0;
    }
}
