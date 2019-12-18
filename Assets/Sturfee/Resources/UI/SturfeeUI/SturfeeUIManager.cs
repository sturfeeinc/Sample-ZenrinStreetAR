using UnityEngine;
using Sturfee.Unity.XR.Core.Session;

public class SturfeeUIManager : MonoBehaviour
{

    public delegate void OnApplicationQuitDelegate();
    public static event OnApplicationQuitDelegate OnApplicationQuit;

    public void Quit()
    {
        OnApplicationQuit?.Invoke();
#if !UNITY_ANDROID || UNITY_EDITOR
        Application.Quit();
#else
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    public void Reload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void UpdateLocation()
    {
        XRSessionManager.GetSession().ForceLocationUpdate();
    }
}
