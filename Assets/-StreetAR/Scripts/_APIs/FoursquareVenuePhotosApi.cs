using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoursquareVenuePhotosAPI {

	[System.Serializable]
	public class RootObject
	{
		public Meta meta;
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
		public Photos photos;
	}

	[System.Serializable]
	public class Photos
	{
		public int count;
		public Items[] items;
		public int dupesRemoved;
	}

	[System.Serializable]
	public class Items
	{
		public string id;
		public int createdAt;
		public Source source;
		public string prefix;
		public string suffix;
		public int width;
		public int height;
		public User user;
		public Checkin checkin;
		public string visibility;
	}

	[System.Serializable]
	public class Source
	{
		public string name;
		public string url;
	}

	[System.Serializable]
	public class User
	{
		public string id;
		public string firstName;
		public string lastName;
		public string gender;
		public Photo photo;
	}

	[System.Serializable]
	public class Photo
	{
		public string prefix;
		public string suffix;
	}

	[System.Serializable]
	public class Checkin
	{
		public string id;
		public int createdAt;
		public string type;
		public int timeZoneOffset;
	}
}
