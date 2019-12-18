using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class MapboxAPI : MonoBehaviour
{

    [System.Serializable]
    public class RootObject
    {
        //public Meta meta;
        public Response response;
    }

    [System.Serializable]
    public class Meta
    {
        public int code;
        public string requestId;
    }

    [System.Serializable]
    public class Response
    {
        public Waypoints[] waypoints;
        public Routes[] routes;
        public string code;
    }

    [System.Serializable]
    public class Waypoints
    {
        public string name;
        public float[] location;
    }

    //[System.Serializable]
    //public class Location
    //{
    //    public string name;
    //    public string location;
    //}

    [System.Serializable]
    public class Routes
    {
        public Geometry geometry;
        public Legs[] legs;
        public string weight_name;
        public float weight;
        public float duration;
        public float distance;
        public string voiceLocale;

    }

    [System.Serializable]
    public class Legs
    {
        public string summary;
        //public float weight;
        public float duration;
        public Steps[] steps;
        //public Intersections[] intersections;
        //public string drivingSide;
        //public string geometry;
        //public string mode;
        //public Maneuver maneuver;

        public float distance;

    }

    [System.Serializable]
    public class Steps
    {
        public Intersections[] intersections;
        public string driving_side;
        //public string geometry;
        public string mode;
        public Maneuver maneuver;
        //public string ref;
        public float weight;
        public float duration;
        public string name;
        public float distance;
        //public VoiceInstructions[] voiceInstructions;
        //public BannerInstructions[] bannerInstructions;
        public string destinations;
        public string exit;

    }

    [System.Serializable]
    public class Intersections
    {
        //public int out;
        public bool entry;
        public int[] bearings;
        public float[] location;
    }

    [System.Serializable]
    public class Maneuver
    {
        public int bearing_after;
        public int bearing_before;
        public float[] location;
        public string modifier;
        public string type;
        public string instruction;
    }

    [System.Serializable]
    public class Geometry
    {
        public Vector2[] coordinates;
        //public Coordinates[][] coordinates;
        //public float[][] coordinates;
        //public float[] coordinates;
        //public Coordinates[] coordinates;   // results in length 5 (which is correct)
        //public Coordinates coordinates;
        //public float[,] coordinates;
        public string type;
    }

    [System.Serializable]
    public class Coordinates
    {
        //public Vector2[]
        //public float longitude;
        //public float[] location;
        //public float[][] test;
        //public double[] test;
    }


    //[System.Serializable]
    //public class VoiceInstructions
    //{
    //    public float distanceAlongGeometry;
    //    public string announcement;
    //    public string ssmlAnnouncement;
    //}

    //[System.Serializable]
    //public class BannerInstructions
    //{
    //    public float distanceAlongGeometry;
    //    public Primary primary;
    //    //public Secondary secondary;
    //    //public string then;

    //}

    //[System.Serializable]
    //public class Primary
    //{
    //    public string text;
    //    public Components[] components;
    //    public string type;
    //    public string modifier;
    //}

    //[System.Serializable]
    //public class Components
    //{
    //    public string text;
    //}
}
