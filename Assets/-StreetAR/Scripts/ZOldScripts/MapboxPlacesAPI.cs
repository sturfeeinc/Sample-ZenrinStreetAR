using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapboxPlacesAPI{

	[System.Serializable]
	public class Response
	{
		public string type;
//		public List<string> query;
//		public List<Features> features;
		public string[] query;
		public Features[] features;

		public string attribution;

	}

	[System.Serializable]
	public class Features
	{
		public string id;
		public string type;
		public string[] place_type;
		public float relevance;
		public Properties properties;
		public string text;
		public string place_name;
		public float[] bbox;
		public float[] center;
	}

	[System.Serializable]
	public class Properties
	{
		public string wikidata;
		public string category;
	}

	[System.Serializable]
	public class Geometry
	{
		public string type;
		public float[] coordinates;

	}

	[System.Serializable]
	public class Context
	{
		public string id;
		public string short_code;
		public string wikidata;
		public string text;
	}
}
