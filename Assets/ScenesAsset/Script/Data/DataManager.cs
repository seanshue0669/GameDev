using UnityEngine;
using PlayerDataSO;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 創建新的 GameObject 並添加 DataManager 組件
                GameObject obj = new GameObject("DataManager");
                _instance = obj.AddComponent<DataManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public PlayerData playerData;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 保持 DataManager 不被銷毀
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 確保只有一個 DataManager 實例
        }
    }

    public void SetAndDisplayValue(string key, object value)
    {
        try
        {
            playerData.SetValue(key, value);

            if (key == "money" || key == "chips")
            {
                EventSystem.Instance.TriggerEvent("Assetdisplay", "display", 0);
            }
        }
        catch
        {
            Debug.LogError("data type with key is wrong");
        }
    }

    public void AddAndDisplayValue(string key, object value)
    {
        try
        {
            playerData.AddValue(key, value);

            if (key == "money" || key == "chips")
            {
                EventSystem.Instance.TriggerEvent("Assetdisplay", "display", 0);
            }
        }
        catch
        {
            Debug.LogError("data type with key is wrong");
        }
    }

    public void SubAndDisplayValue(string key, object value)
    {
        try
        {
            playerData.SubValue(key, value);

            if (key == "money" || key == "chips")
            {
                EventSystem.Instance.TriggerEvent("Assetdisplay", "display", 0);
            }
        }
        catch
        {
            Debug.LogError("data type with key is wrong");
        }
    }
}
