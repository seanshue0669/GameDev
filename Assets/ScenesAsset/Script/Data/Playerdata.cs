using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Serializable]
    public class PlayerDataEntry
    {
        public string Key;
        public DataType Type;
        public int IntValue;
        public bool BoolValue;
        public float FloatValue;
        public string StringValue;
        public List<int> IntList = new List<int>();
        public List<float> FloatList = new List<float>();
        public task test;
    }

    public enum DataType { Int, Bool, Float, String, IntList, FloatList, task }

    [SerializeField]
    public List<PlayerDataEntry> Data = new List<PlayerDataEntry>();

    // Get Value by Key
    public object GetValue(string key)
    {
        var entry = Data.Find(e => e.Key == key);
        if (entry == null)
        {
            Debug.LogError($"Key '{key}' not found in PlayerData.");
            return null;
        }

        return entry.Type switch
        {
            DataType.Int => entry.IntValue,
            DataType.Bool => entry.BoolValue,
            DataType.Float => entry.FloatValue,
            DataType.String => entry.StringValue,
            DataType.IntList => entry.IntList,
            DataType.FloatList => entry.FloatList,
            DataType.task => entry.test,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    // Set Value by Key
    public void SetValue(string key, object value)
    {
        var entry = Data.Find(e => e.Key == key);
        if (entry == null)
        {
            Debug.LogError($"Key '{key}' not found in PlayerData.");
            return;
        }

        try
        {
            switch (entry.Type)
            {
                case DataType.Int:
                    entry.IntValue = Convert.ToInt32(value);
                    break;
                case DataType.Bool:
                    entry.BoolValue = Convert.ToBoolean(value);
                    break;
                case DataType.Float:
                    entry.FloatValue = Convert.ToSingle(value);
                    break;
                case DataType.String:
                    entry.StringValue = value.ToString();
                    break;
                case DataType.IntList:
                    entry.IntList = (List<int>)value;
                    break;
                case DataType.FloatList:
                    entry.FloatList = (List<float>)value;
                    break;
                case DataType.task:
                    entry.test = (task)value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error setting value for key '{key}': {ex.Message}");
        }
    }
}

[System.Serializable]
public class task
{
    public string id; // 与 AchievementDefinition 的 id 对应
    public bool isUnlocked; // 是否已解锁
    public int currentValue; // 当前进度值
}
