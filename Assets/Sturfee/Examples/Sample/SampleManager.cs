using System;
using System.IO;
using System.Collections;

using UnityEngine;
using UnityEditor;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Utilities;
using Sturfee.Unity.XR.Core.Providers.Base;

public class SampleManager : MonoBehaviour {

    public string SampleName;
    public bool Cache;
    public int Index { get; private set; }

    public delegate void SampleDataReadyevent();
    public event SampleDataReadyevent OnSampleDataReady;

    private GpsProviderBase _sampleGpsProvider;
    private ImuProviderBase _sampleImuProvider;
    private VideoProviderBase _sampleVideoProvider;

    private SampleData[] _sampleDatas;

    public string CacheDirectory
    {
        get
        {
            string directory = Path.Combine(Application.persistentDataPath, "Sample Scene Frames", SampleName);
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }    
    }

    private IEnumerator Start()
    {
        _sampleGpsProvider = GetComponent<GpsProviderBase>();
        _sampleImuProvider = GetComponent<ImuProviderBase>();
        _sampleVideoProvider = GetComponent<VideoProviderBase>();

        yield return new WaitForEndOfFrame();

        // First check Cache
        if(File.Exists(CacheDirectory + "/Data.txt"))
        {
            string dataJson = File.ReadAllText(CacheDirectory + "/Data.txt");
            _sampleDatas = JsonHelper.FromJson<SampleData>(dataJson);

            OnSampleDataReady?.Invoke();
        }
        else
        {
            // Download from AWS
            StartCoroutine(DownloadSampleData((data) =>
            {
                string dataJson = data;
                _sampleDatas = JsonHelper.FromJson<SampleData>(dataJson);                

                OnSampleDataReady?.Invoke();
            }));
        }
    }

    private void Update()
    {

        if(_sampleGpsProvider.GetProviderStatus() != ProviderStatus.Ready ||
            _sampleImuProvider.GetProviderStatus() != ProviderStatus.Ready ||
            _sampleVideoProvider.GetProviderStatus() != ProviderStatus.Ready)
        {
            return;
        }

        //Detect Left swipe
        if (Input.GetAxis("Mouse X") < -0.5f)
        {
            Index = (Index == 0) ? GetDataLength() - 1 : Index - 1;
        }

        //Detect Right swipe
        if (Input.GetAxis("Mouse X") > 0.5f)
        {
            Index = (Index == (GetDataLength() - 1)) ? 0 : Index + 1;
        }
    }

    public SampleData GetDataAtCurrentIndex()
    {
        return _sampleDatas[Index];
    }

    public SampleData GetDataAtIndex(int index)
    {
        return _sampleDatas[index];
    }

    public int GetDataLength()
    {
        return _sampleDatas.Length;
    }

    public void ClearCache()
    {
        if (Directory.Exists(CacheDirectory))
        {
            Directory.Delete(CacheDirectory, true);

            Debug.Log("Cache cleared");
        }
    }

    private IEnumerator DownloadSampleData(Action<string> callback)
    {
        string path = "https://sdk-sample-scene.s3-us-west-1.amazonaws.com/" + SampleName;

        WWW www = new WWW(path + "/Data.txt");
        Debug.Log(www.url);
        yield return www;

        if(string.IsNullOrEmpty(www.error))
        {
            // Save to Cache
            File.WriteAllText(CacheDirectory + "/Data.txt", www.text);

            callback(www.text);
        }
        else
        {
            Debug.LogError(www.error);
        }
    }


    [System.Serializable]
    public class SampleData
    {
        // GPS
        public double latitude;
        public double longitude;
        public double height;
        // IMU
        public Quaternion quaternion;
        // Video
        public string imName;
        public float sceneWidth;
        public float sceneHeight;
        public Matrix4x4 projectionMatrix;
        public float fov;
        public bool isPortrait;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SampleManager))]
public class SampleManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear cache"))
        {
            ((SampleManager)target).ClearCache();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }
}

#endif
