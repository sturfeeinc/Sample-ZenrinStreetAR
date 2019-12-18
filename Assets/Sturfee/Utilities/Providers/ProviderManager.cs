using UnityEngine;
using UnityEditor;

using Sturfee.Unity.XR.Core.Providers.Base;

[ExecuteInEditMode]
public class ProviderManager : MonoBehaviour
{
    private SturfeeXRSession _xrSession;

    private string _currentProviderSetName;
    private string _previousProviderSetName;

    private void OnEnable()
    {
        _xrSession = GetComponent<SturfeeXRSession>();

        _currentProviderSetName = _xrSession.ProviderSetName;
        _previousProviderSetName = _currentProviderSetName;
    }

    private void Update()
    {
        _currentProviderSetName = _xrSession.ProviderSetName;
        if (_currentProviderSetName != _previousProviderSetName)
        {
            OnProviderSetChange();
            _previousProviderSetName = _currentProviderSetName;
        }
    }

    private void OnProviderSetChange()
    {
        // Clear all the providers from previous provider set
        Clear();

        // Apply selected Provider set in inspector to ProvisderSet variable of SturfeeXrSession
        _xrSession.ProviderSet = (Resources.Load("Provider Sets/" + _xrSession.ProviderSetName) as GameObject).GetComponent<ProviderSet>();

        // Copy provider components from Provider set prefab to SturfeeXrSession
        CopyProviderComponents(_xrSession.ProviderSet.gameObject, _xrSession.gameObject);

        // Set SturfeeXrSession provider references
        if (_xrSession.ProviderSet.ImuProvider != null)
        {
            _xrSession.ImuProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.ImuProvider.GetType()) as ImuProviderBase;
        }
        if (_xrSession.ProviderSet.GpsProvider != null)
        {
            _xrSession.GpsProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.GpsProvider.GetType()) as GpsProviderBase;
        }
        if (_xrSession.ProviderSet.VideoProvider != null)
        {
            _xrSession.VideoProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.VideoProvider.GetType()) as VideoProviderBase;
        }
    }

    private void Clear()
    {
        _xrSession.ImuProvider = null;
        _xrSession.GpsProvider = null;
        _xrSession.VideoProvider = null;

        #if UNITY_EDITOR        
        SerializedObject serializedObject = new SerializedObject(gameObject);
        var prop = serializedObject.FindProperty("m_Component");

        //First remove all the components that has "RequireComponent" attributes       
        for (int i=0; i < _xrSession.gameObject.GetComponents<Component>().Length; i++)
        {        
            Component co = _xrSession.gameObject.GetComponents<Component>()[i];

            if (co == null)
            {
                prop.DeleteArrayElementAtIndex(i);
                continue;
            }

            if (co.GetType().IsDefined(typeof(RequireComponent), false) && (co as Camera == null) && (co as SturfeeXRSession == null))
            {
                //TODO: If a component has RequireComponent and is also a part of another component's RequireComponent
                
                DestroyImmediate(co);
            }
        }
        

        foreach (Component co in _xrSession.gameObject.GetComponents<Component>())
        {
            var providersComponent = co as SturfeeXRSession;
            var transformComponent = co as Transform;
            var providerHandler = co as ProviderManager;
            if (providersComponent == null && transformComponent == null && providerHandler == null)
            {
                DestroyImmediate(co);
            }
        }

        serializedObject.ApplyModifiedProperties();
        #endif
    }

    private void CopyProviderComponents(GameObject sourceGO, GameObject targetGO)
    {
        foreach (var component in sourceGO.GetComponents<Component>())
        {
            if(component == null)
            {
                continue;
            }

            var componentType = component.GetType();
            if (componentType != typeof(Transform) &&
                componentType != typeof(ProviderSet)
            )
            {
#if UNITY_EDITOR
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGO);
#endif
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SturfeeXRSession))]
public class XrSessionProvidersEditor : Editor
{
    private string[] _providerSets;

    private SerializedProperty _provType;
    private SerializedProperty _provString;

    private void OnEnable()
    {
        Object[] androidSets = Resources.LoadAll("Provider Sets/Android");
        Object[] iOSSets = Resources.LoadAll("Provider Sets/IOS");
        Object[] editorSets = Resources.LoadAll("Provider Sets/Editor");

        _providerSets = new string[androidSets.Length + iOSSets.Length + editorSets.Length];

        for (int i = 0; i < _providerSets.Length; i++)
        {
            if (i < androidSets.Length)
            {
                _providerSets[i] = "Android/" + androidSets[i].name;
            }
            else if (i < androidSets.Length + iOSSets.Length)
            {
                _providerSets[i] = "IOS/" + iOSSets[i - androidSets.Length].name;
            }
            else
            {
                _providerSets[i] = "Editor/" + editorSets[i - androidSets.Length - iOSSets.Length].name;
            }
        }

        _provType = serializedObject.FindProperty("_provTypeInt");
        _provString = serializedObject.FindProperty("ProviderSetName");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        _provType.intValue = EditorGUILayout.Popup("Provider Set", _provType.intValue, _providerSets, GUILayout.Height(20));
        _provString.stringValue = _providerSets[_provType.intValue];

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
