using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Arrow collider is on separate object from WorldPathArrow script
public class WorldArrowCollider : MonoBehaviour
{
    void Start()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "XRCamera")
        { 
            GetComponentInParent<WorldPathArrow>().ColliderHit();
        }
    }
}
