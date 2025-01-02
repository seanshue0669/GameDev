using System.Collections.Generic;
using System.IO;
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

    public delegate void OnDataChanged();
    public event OnDataChanged DataChanged;

    public PlayerData playerData;

    private string saveFilePath;

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
}
