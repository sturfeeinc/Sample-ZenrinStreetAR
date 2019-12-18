using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Session;
using UnityEngine;
using UnityEngine.Networking;
using System;


public class FoursquareService : MonoBehaviour {

	void Start () {
		
	}

    public void GetVenueData(Vector3 pos, Action<List<FoursquareAPI.Venue>> callback, bool addUnityCoord = false)
    {
        StartCoroutine(GetFoursquareVenueData (pos, addUnityCoord, (myReturnValue) => {
            callback(myReturnValue);
        }));
    }


    private IEnumerator GetFoursquareVenueData(Vector3 pos, bool addUnityCoord, Action<List<FoursquareAPI.Venue>> callback)
    {
        var gps = XRSessionManager.GetSession().LocalPositionToGps(pos);

        string url = "https://api.foursquare.com/v2/venues/search?ll=" + gps.Latitude.ToString()/*Lat*/ + ","
            + gps.Longitude.ToString()/*Lng*/ + "&radius=30&oauth_token=VXIREDRYNSJEZL3OOQ3GLUF03HZXQQLRENNA5ERWUF1DYBMH&v=20160419&intent=browse";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var d = JsonUtility.FromJson<FoursquareAPI.RootObject>(www.downloadHandler.text);
                List<FoursquareAPI.Venue> venues = d.response.venues;

                if(addUnityCoord)
                {
                    AddUnityCoord(venues);
                }

                callback(venues);
            }
        }
    }

    // Computes and adds the Unity (Vector3) coordinates to the venues via  the lat/long values of the venue locations
    private List<FoursquareAPI.Venue> AddUnityCoord(List<FoursquareAPI.Venue> venues)
    {
        for (int i = 0; i < venues.Count; i++)
        {
            var gpsPos = new Sturfee.Unity.XR.Core.Models.Location.GpsPosition();
            gpsPos.Latitude = venues[i].location.lat;
            gpsPos.Longitude = venues[i].location.lng;
            gpsPos.Height = 0;
            venues[i].unityCoord = XRSessionManager.GetSession().GpsToLocalPosition(gpsPos);
        }
        return venues;
    }

}
