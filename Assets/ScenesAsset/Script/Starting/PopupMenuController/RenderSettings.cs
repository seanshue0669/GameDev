using UnityEngine;
using TMPro;
public class RenderSettings : MonoBehaviour
{
    private static RenderSettings _instance;
    public static RenderSettings Instance;
    [SerializeField]
    public bool GI = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log($"GI:{GI}");
    }
    #region CallMethod
    public void ChangeGI(bool Enable)
    {
        GI = Enable;
    }
    #endregion
}
