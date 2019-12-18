using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Models.Location;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Utilities;
using UnityEngine;

using System.IO;
using System;
using Sturfee.Unity.XR.Core.Events;

public class Logger : MonoBehaviour
{
    private List<Data> _datas;
    private string _path;

    void Start()
    {
        _datas = new List<Data>();

        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;

        var directory = Path.Combine(Application.persistentDataPath, "Log Xr Camera Points");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        string fileName = DateTime.Now.Ticks.ToString() + ".txt";
        _path = Path.Combine(directory, fileName);

        StartCoroutine(LogStuff());
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
    }

    private void OnLocalizationSuccessful()
    {
        Log(true);
    }

    private IEnumerator LogStuff()
    {
        yield return new WaitForSeconds(5);
        Log(false);
        StartCoroutine(LogStuff());
    }

    private void Log(bool isLocalizationRequest)
    {
        Data data = new Data();
        data.XrCamPos = XRSessionManager.GetSession().GetXRCameraPosition();
        data.XrCamOrient = XRSessionManager.GetSession().GetXRCameraOrientation();
        data.GpsPos = XRSessionManager.GetSession().GetXRCameraLocation();
        data.GpsOrient = OrientationUtils.UnityToWorld(XRSessionManager.GetSession().GetXRCameraOrientation());
        data.ProjectionMatrix = XRSessionManager.GetSession().VideoProvider.GetProjectionMatrix();
        data.TimeStamp = DateTime.Now.ToString("O");
        data.IsLocalizationRequest = isLocalizationRequest;


        _datas.Add(data);

        string jsonData = JsonHelper.ToJson<Data>(_datas.ToArray());

        File.WriteAllText(_path, jsonData);

        Debug.Log("XR Cam Position: " + XRSessionManager.GetSession().GetXRCameraPosition());
        Debug.Log("XR Cam Orientation: " + XRSessionManager.GetSession().GetXRCameraOrientation());
        Debug.Log("GPS Position: " + XRSessionManager.GetSession().GetXRCameraLocation());
        Debug.Log("GPS Orientation" + OrientationUtils.UnityToWorld(XRSessionManager.GetSession().GetXRCameraOrientation()));
        Debug.Log("XR Cam Projection Matrix: " + XRSessionManager.GetSession().GetXRCameraPosition());
        Debug.Log("Localization Request: " + isLocalizationRequest);
        Debug.Log("Time Stamp: " + DateTime.Now.ToString("O"));
    }


    [Serializable]
    private class Data
    {
        public Vector3 XrCamPos;
        public Quaternion XrCamOrient;
        public GpsPosition GpsPos;
        public Quaternion GpsOrient;
        public string TimeStamp;
        public bool IsLocalizationRequest;
        public Matrix4x4 ProjectionMatrix;
    }

}


