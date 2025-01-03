using UnityEngine;

public class QuitApp : MonoBehaviour 
{
    public void QuitApplication()
    {
        Debug.Log("QuitApp");
        #if UNITY_EDITOR
                // Exit play mode in the Unity Editor
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // Quit the application in a build
                Application.Quit();
        #endif
    }
}
