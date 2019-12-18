using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is added to a buiding that has been detected by raycast, containing the Foursquare POI data of when it was detected
public class PoiDataProvider : MonoBehaviour {

    public List<FoursquareAPI.Venue> Venues;
    //public Vector3 RaycastedPoint;
    // TODO: Add venues with their converted lat/long positions

    // Use this for initialization
    void Start () {
		
	}

}
