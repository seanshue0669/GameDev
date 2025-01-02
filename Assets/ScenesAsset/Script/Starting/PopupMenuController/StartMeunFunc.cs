using UnityEngine;

public class StartMeunFunc:MonoBehaviour
{
    [SerializeField]
    public GameObject settingsUI;
    public GameObject CreaterUI;

    public void SettingsUIController(bool p_open)
    {
        Debug.Log("Show");
        if(p_open)
            settingsUI.SetActive(true);
        else
            settingsUI.SetActive(false);
    }
    public void CreaterUIController(bool p_open)
    {
        if(p_open)
            CreaterUI.SetActive(true);
        else
            CreaterUI.SetActive(false);
    }
}