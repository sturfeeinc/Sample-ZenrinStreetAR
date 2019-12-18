using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZenrinAPI : MonoBehaviour {

    [Serializable]
    public class RootObject
    {
        public Status status;
        public Info info;
        public Item[] item;
    }

    [Serializable]
    public class Status
    {
        public string code;
        public string text;
    }

    [Serializable]
    public class Info
    {
        public int hit;
        //public Facet facet;
    }

    [Serializable]
    public class Item
    {
        public Poi poi;
        public double distance;
    }

    [Serializable]
    public class Poi
    {
        public string text;
        public string code;
        public Point point;
        public string kana;
        public string addressText;
        public string zipcode;
        public string phoneNumber;
        public Status genre;
        public string detail;   // Unsure about this;
        public Language language;
        public ArrivalInfo[] arrivalInfoArray;

    }

    //[Serializable]
    //public class Genre
    //{
    //    string code;
    //    string text;
    //}

    [Serializable]
    public class ArrivalInfo
    {
        public string text;
        public string kana;
        public Status exitType;
        public ArrivalPoint arrivalPoint;
        public ArrivalPoint exitPoint;
    }

    //[Serializable]
    //public class ExitType
    //{
    //    string code;
    //    string text;
    //}

    [Serializable]
    public class ArrivalPoint
    {
        public double lat;
        public double lon;
    }

    [Serializable]
    public class Point
    {
        public double lat;
        public double lon;
    }

    [Serializable]
    public class Language
    {
        public En en;
    }

    [Serializable]
    public class En
    {
        public string text;
        public string[] addressParts;
    }
}
