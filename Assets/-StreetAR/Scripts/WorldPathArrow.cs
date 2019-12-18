using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Package.Utilities;
using UnityEngine;

public class WorldPathArrow : MonoBehaviour {


    public Transform ArrowFacer;
    public Transform Arrow;
    public Collider Collider;
    public Transform MapIcon;

    [HideInInspector]
    public bool Relocalize;

    private Collider _collider;
    private LayerMask _sturfeeTerrainLayerMask;

    private static Vector3 _lastArrowDir;

	void Awake () {
        //_collider = GetComponent<Collider>();
        _sturfeeTerrainLayerMask |= (1 << LayerMask.NameToLayer("sturfeeTerrain"));
    }
	

    public void ColliderHit() //OnTriggerEnter(Collider other)
    {
        //if (other.tag == "XRCamera" )
        //{
            Collider.enabled = false;
            Arrow.gameObject.SetActive(false);


        if(!RelocalizationManager.Instance.RelocalizeCheck())
        {
            PathingManager.Instance.SetNextArrow();
        }

        // TODO: Maybe don't need this?
        StartCoroutine(Wait());

        // TODO: Temp code
        //Destroy(Arrow.gameObject);
    }

    private IEnumerator Wait()
    {
        yield return null;
        Destroy(gameObject);
    }

    public void Initialize(Vector3 pointPos, bool forward)
    {

        pointPos.y = ArrowFacer.position.y;
        ArrowFacer.LookAt(pointPos);
        //pointPos.y = Arrow.position.y;
        //Arrow.LookAt(pointPos);

        //Vector3 arrowDir = pointPos - 

        if (forward)
        {

            Arrow.Rotate(Vector3.forward * 90);

            //MapIcon.Rotate(Vector3.left * 90);
            //MapIcon.rotation = Quaternion.identity;
            SetMapIconUpright();

            //RaycastHit hit;
            //if(Physics.Raycast(transform.position + (Vector3.up * 50), Vector3.down, out hit, 100, _sturfeeTerrainLayerMask))
            //{
                //ArrowFacer.position = hit.point + (Vector3.up * 0.25f);
  


                Vector3 colliderSize = Collider.GetComponent<BoxCollider>().size;//.y = 4;
                float tempXVal = colliderSize.y;
                colliderSize.y = colliderSize.x;
                colliderSize.x = tempXVal;
                Collider.GetComponent<BoxCollider>().size = colliderSize;

            //}
            //else
            //{
            //    Debug.LogError("Error determining forward arrow position");
            //}
        }
        //else
        //{
        //    ArrowFacer.position += Vector3.up;
        //    //Arrow.position += Vector3.up;
        //}
        

        Collider.enabled = true;
        //_collider.enabled = true;
        ArrowFacer.gameObject.SetActive(true);
        //Arrow.gameObject.SetActive(true);


        // Adjust arrow to player position
        //Vector3 relativePoint = ArrowFacer.InverseTransformPoint(PlayerController.Instance.transform.position);
        //if(relativePoint.x < 0)
        //{
        //    print("!! Player is to the left");
        //}
        //else if (relativePoint.x > 0)
        //{
        //    print("!! Player is to the right");
        //}

        //Vector3 arrowDir = poi

        //TODO: Change setup based on if it's a forward or corner arrow





        Vector3 intersectPoint;
        if (forward)
        {

            if (Math3d.LinePlaneIntersection(out intersectPoint, PlayerController.Instance.transform.position, ArrowFacer.forward,
                ArrowFacer.forward, Arrow.transform.position))
            {
                print("!! Intersection happened forward!");
                transform.position = intersectPoint;
            }
            else
            {
                print("!! No Line-Plane intersection forward");
                // Maybe go to next arrow?
            }
        }
        else
        {
            if(_lastArrowDir == null)
            {
                _lastArrowDir = transform.position - PlayerController.Instance.transform.position;
            }

            if (Math3d.LinePlaneIntersection(out intersectPoint, PlayerController.Instance.transform.position, _lastArrowDir,
                _lastArrowDir, Arrow.transform.position))
            {

                print("!! Intersection happened Corner!");
                transform.position = intersectPoint;
            }
            else
            {
                print("!! No Line-Plane intersection corner");
                // Maybe go to next arrow?
            }
        }



        RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.up * 50), Vector3.down, out hit, 100, _sturfeeTerrainLayerMask))
        {
            if (forward)
            {
                ArrowFacer.position = hit.point + (Vector3.up * 0.25f);
            }
            else
            {
                ArrowFacer.position = hit.point + Vector3.up;
            }

        }

        _lastArrowDir = ArrowFacer.forward;
    }

    private void SetMapIconUpright()
    {
        Vector3 rotation = MapIcon.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        MapIcon.rotation = Quaternion.Euler(rotation);
    }


}
