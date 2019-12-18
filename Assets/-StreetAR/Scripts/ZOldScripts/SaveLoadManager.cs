using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Models.Location;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;


public class SaveLoadManager : MonoBehaviour {

	public static List<ArItemData> ArItemList = new List<ArItemData>();

	private static string filename;// = "/street-ar-save-data.txt";

	private void Start()
	{
		filename = AwsHelper.Instance.SampleFileName;
	}

	// CURRENTLY UNUSED - WOULD NEED TO ADD SLASH
	public static bool HasSaveData()
	{
		return (File.Exists (Application.persistentDataPath + filename));
	}

	public static void LoadGameData()
	{
		for (int i = 0; i < ArItemList.Count; i++)
		{
			// Convert serialized data back into usaarItemDatable data objects

			ArItemData arItemData = ArItemList [i];

			Quaternion quat = arItemData.Orientation.GetQuaternion;

			GpsSerializable gpsPos = arItemData.GpsPos;
			var gps = new GpsPosition
			{
				Latitude = gpsPos.Latitude,
				Longitude = gpsPos.Longitude,
				Height = gpsPos.Height
			};
			Vector3 pos = XRSessionManager.GetSession ().GpsToLocalPosition (gps);


//			GameObject spawnItem;
//			if (arItemData.ItemType == (int)ArItemType.tier1)
//			{
//				spawnItem = GameManager.Instance.Tier1ItemPrefab;
//			}
//			else if (arItemData.ItemType == (int)ArItemType.tier2)
//			{
//				spawnItem = GameManager.Instance.Tier2ItemPrefab;
//			}
//			else
//			{
//				spawnItem = GameManager.Instance.Tier3ItemPrefab;
//			}
//

			//PlayerMapTouchController.Instance.SpawnSavedFlag (pos, quat, arItemData.Id);

//			GameObject arItem = Instantiate (PlayerMapTouchController.Instance.FlagPrefab, pos, quat);
//			arItem.GetComponent<ArItem> ().Id = arItemData.Id;
		}
	}

	public static void SaveArItem(Transform arItem)
	{
		// Convert the AR item's Unity data into serializable format that can be saved

		ArItemData arItemData = new ArItemData ();

//		arItemData.ItemType = (int) arItem.GetComponent<ArItem>().ItemType;

		QuaternionSerializable newQuat = new QuaternionSerializable ();
		newQuat.Fill (arItem.rotation);
		arItemData.Orientation = newQuat;

		var rawGpsPos = XRSessionManager.GetSession ().LocalPositionToGps (arItem.position);
		GpsSerializable gpsPos = new GpsSerializable ();
		gpsPos.Height = rawGpsPos.Height;
		gpsPos.Latitude = rawGpsPos.Latitude;
		gpsPos.Longitude = rawGpsPos.Longitude;
		arItemData.GpsPos = gpsPos;

		// Create (almost) unique ID for this AR item
		string itemId = arItem.GetInstanceID().ToString() + "." + rawGpsPos.Latitude.ToString() + "." + rawGpsPos.Longitude.ToString();
		arItemData.Id = itemId;
		arItem.GetComponent<ArItem> ().Id = itemId;

		Debug.Log ("Added AR item to save data");
		ArItemList.Add (arItemData);
		Save ();
	}

	public static void ClearFile()
	{
		ArItemList.Clear ();
		Save ();
	}

	// CURRENTLY UNUSED
	public static void RemoveArItem(string arItemId)
	{
		bool foundItem = false;
		int count = 0;
		while (!foundItem && count < ArItemList.Count)
		{
			if (ArItemList [count].Id.Equals(arItemId))
			{
				ArItemList.RemoveAt (count);
				foundItem = true;
				Debug.Log ("Removed AR item from save data");
			}
			count++;
		}

		if (!foundItem)
		{
			Debug.LogError ("Could not find removed AR item's ID in save data. Nothing was removed from save data.");
		}
		else
		{
			Save ();
		}
	}

	public static void LoadFile() {

		AwsHelper.Instance.GetObject ();


		if(File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + filename))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + Path.DirectorySeparatorChar + filename, FileMode.Open);

			if(file.Length > 0)
			{
				ArItemList = (List<ArItemData>)bf.Deserialize(file);
			}
			file.Close();

			Debug.Log ("Loaded File");
		}
	}

	public static void Unload()
	{
		ArItemList = new List<ArItemData>();
	}

	private static void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + Path.DirectorySeparatorChar + filename);
		bf.Serialize(file, ArItemList);
		file.Close();

		AwsHelper.Instance.PostObject ();

		Debug.Log ("Saved Game Data");
	}
}

[Serializable]
public struct ArItemData
{
	public string Id;
//	public int ItemType;
	public GpsSerializable GpsPos;
	public QuaternionSerializable Orientation;
}

[Serializable]
public struct GpsSerializable 
{
	public double Latitude;
	public double Longitude;
	public double Height;
}

[Serializable]
public struct QuaternionSerializable
{
	public float x;
	public float y;
	public float z;
	public float w;

	public void Fill(Quaternion q)
	{
		x = q.x;
		y = q.y;
		z = q.z;
		w = q.w;
	}

	public Quaternion GetQuaternion { get { return new Quaternion (x, y, z, w); } }	
}

