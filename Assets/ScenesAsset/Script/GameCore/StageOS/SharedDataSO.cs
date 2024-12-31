using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSharedData", menuName = "GameModule/Shared_Data")]
public class SharedDataSO : ScriptableObject
{
    [Serializable]
    public class SharedDataEntry
    {
        public string Key;
        public DataType Type;
        public string StringValue;
        public int IntValue;
        public float FloatValue;
    }
    [SerializeField]
    public string m_gameName;

    public enum DataType { String, Int, Float }

    [SerializeField]
    public List<SharedDataEntry> Data = new List<SharedDataEntry>();

    public void SetInt(string key, int value)
    {
        var entry = GetEntry(key, DataType.Int);
        if (entry == null)
        {
            entry = new SharedDataEntry { Key = key, Type = DataType.Int, IntValue = value };
            Data.Add(entry);
        }
        else
        {
            entry.IntValue = value;
        }
    }

    public int GetInt(string key)
    {
        var entry = GetEntry(key, DataType.Int);
        return entry?.IntValue ?? 0;
    }

    public void SetString(string key, string value)
    {
        var entry = GetEntry(key, DataType.String);
        if (entry == null)
        {
            entry = new SharedDataEntry { Key = key, Type = DataType.String, StringValue = value };
            Data.Add(entry);
        }
        else
        {
            entry.StringValue = value;
        }
    }

    public string GetString(string key)
    {
        var entry = GetEntry(key, DataType.String);
        return entry?.StringValue ?? string.Empty;
    }
    public void SetFloat(string key, float value)
    {
        var entry = GetEntry(key, DataType.Float);
        if (entry == null)
        {
            entry = new SharedDataEntry { Key = key, Type = DataType.Float, FloatValue = value };
            Data.Add(entry);
        }
        else
        {
            entry.FloatValue = value;
        }
    }

    public float GetFloat(string key)
    {
        var entry = GetEntry(key, DataType.Float);
        return entry?.FloatValue ?? 0f;
    }

    public bool Remove(string key)
    {
        var entry = Data.Find(e => e.Key == key);
        if (entry != null)
        {
            Data.Remove(entry);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        Data.Clear();
    }

    private SharedDataEntry GetEntry(string key, DataType type)
    {
        var entry = Data.Find(e => e.Key == key);
        if (entry != null && entry.Type != type)
        {
            Debug.LogError($"Key '{key}' exists but with a different type: {entry.Type}");
            return null;
        }
        return entry;
    }
}
