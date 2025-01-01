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
    }

    public enum DataType { Int, Bool, Float, String }

    [SerializeField]
    public List<PlayerDataEntry> Data = new List<PlayerDataEntry>();


    //dont use these 4 functions in the Constructor of your game stages(so far)!!!
    #region basic function
    // Get Value by Key
    public T GetValue<T>(string key)
    {
        var entry = Data.Find(e => e.Key == key);
        if (entry == null)
        {
            Debug.LogError($"Key '{key}' not found in PlayerData.");
            return default(T);
        }

        try
        {
            return entry.Type switch
            {
                DataType.Int when typeof(T) == typeof(int) => (T)(object)entry.IntValue,
                DataType.Bool when typeof(T) == typeof(bool) => (T)(object)entry.BoolValue,
                DataType.Float when typeof(T) == typeof(float) => (T)(object)entry.FloatValue,
                DataType.String when typeof(T) == typeof(string) => (T)(object)entry.StringValue,
                _ => throw new InvalidCastException($"Cannot convert type {entry.Type} to {typeof(T)}.")
            };
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting value for key '{key}': {ex.Message}");
            return default(T);
        }
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error setting value for key '{key}': {ex.Message}");
        }
    }
    #endregion

    #region counting
    public void AddValue(string key, object value)
    {
        var entry = Data.Find(e => e.Key == key);

        if (entry == null)
        {
            Debug.LogError($"Key '{key}' not found in PlayerData.");
            return;
        }

        if (!(value is int) && !(value is float))
        {
            Debug.LogError($"AddValue supports only int or float types. Provided value type: {value.GetType()}");
            return;
        }

        try
        {
            switch (entry.Type)
            {
                case DataType.Int:
                    if (value is int intValue)
                    {
                        entry.IntValue += intValue;
                    }
                    else if (value is float floatValue)
                    {
                        // Optionally handle float addition to int by rounding or truncating
                        entry.IntValue += Mathf.RoundToInt(floatValue);
                        Debug.LogWarning($"Adding a float to an int. Float value {floatValue} was rounded to {Mathf.RoundToInt(floatValue)}.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot add value of type {value.GetType()} to DataType.Int.");
                    }
                    break;

                case DataType.Float:
                    if (value is float fValue)
                    {
                        entry.FloatValue += fValue;
                    }
                    else if (value is int iValue)
                    {
                        entry.FloatValue += iValue;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot add value of type {value.GetType()} to DataType.Float.");
                    }
                    break;

                default:
                    Debug.LogError($"AddValue is only supported for Int and Float data types. Current data type: {entry.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding value for key '{key}': {ex.Message}");
        }
    }

    public void SubValue(string key, object value)
    {
        var entry = Data.Find(e => e.Key == key);

        if (entry == null)
        {
            Debug.LogError($"Key '{key}' not found in PlayerData.");
            return;
        }

        if (!(value is int) && !(value is float))
        {
            Debug.LogError($"SubValue supports only int or float types. Provided value type: {value.GetType()}");
            return;
        }

        try
        {
            switch (entry.Type)
            {
                case DataType.Int:
                    if (value is int intValue)
                    {
                        entry.IntValue -= intValue;
                    }
                    else if (value is float floatValue)
                    {
                        // Optionally handle float subtraction from int by rounding or truncating
                        entry.IntValue -= Mathf.RoundToInt(floatValue);
                        Debug.LogWarning($"Subtracting a float from an int. Float value {floatValue} was rounded to {Mathf.RoundToInt(floatValue)}.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot subtract value of type {value.GetType()} from DataType.Int.");
                    }
                    break;

                case DataType.Float:
                    if (value is float fValue)
                    {
                        entry.FloatValue -= fValue;
                    }
                    else if (value is int iValue)
                    {
                        entry.FloatValue -= iValue;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot subtract value of type {value.GetType()} from DataType.Float.");
                    }
                    break;

                default:
                    Debug.LogError($"SubValue is only supported for Int and Float data types. Current data type: {entry.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error subtracting value for key '{key}': {ex.Message}");
        }
    }
    #endregion

}
