using UnityEngine;

public class StartMeunFunc:MonoBehaviour
{
    [SerializeField]
    public GameObject SettingPopUpUI;
    public GameObject CreaterPopUpUI;
    void Start()
    {
        SettingPopUpUI.SetActive(false);
        CreaterPopUpUI.SetActive(false);
    }
    #region 
    public void SettingUIController(bool show)
    {
        if (show)
        {
            Debug.Log("Show Settings");
            SettingPopUpUI.SetActive(true);
        }
        else
            SettingPopUpUI.SetActive(false);
    }
    public void CreaterUIController(bool show)
    {
        if (show)
        {
            Debug.Log("Creater");
            CreaterPopUpUI.SetActive(true);
        }
        else
            CreaterPopUpUI.SetActive(false);
    }
    #endregion
}