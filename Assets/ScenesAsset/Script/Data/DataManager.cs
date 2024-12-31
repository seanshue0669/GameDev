using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static PlayerData;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中查找 DataManager
                _instance = FindObjectOfType<DataManager>();
                if (_instance == null)
                {
                    // 如果场景中没有 DataManager，则创建一个新的 GameObject 并添加 DataManager 组件
                    GameObject obj = new GameObject("DataManager");
                    _instance = obj.AddComponent<DataManager>();
                }
            }
            return _instance;
        }
    }

    public delegate void OnDataChanged();
    public event OnDataChanged DataChanged;

    public PlayerData playerData;
}
