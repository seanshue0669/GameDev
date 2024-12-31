using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu(fileName = "UIComponentCollectionOS", menuName = "GameModule/UI_Component_Collection")]
public class UIComponentCollectionSO : ScriptableObject
{
    [Serializable]
    public class UIComponentEntry
    {
        public string Key = "NewKey";
        public UIComponentType Type = UIComponentType.Button;
        public UnityEngine.UI.Button ButtonComponent;
        public TMP_Text TextComponent;
        public TMP_InputField InputFieldComponent;
    }
    [SerializeField]
    public string m_gameName;

    public enum UIComponentType
    {
        Button,
        TMP_Text,
        TMP_InputField
    }

    [SerializeField]
    private List<UIComponentEntry> uiComponents = new List<UIComponentEntry>();

    public List<UIComponentEntry> UIComponents => uiComponents;

    public T CreateUIComponent<T>( string key, Transform parent) where T : Component
    {
        var entry = UIComponents.FirstOrDefault(e => e.Key == key);
        if (entry == null)
        {
            Debug.LogError($"No UI component found with key: {key}");
            return null;
        }

        GameObject prefab = null;

        if (typeof(T) == typeof(UnityEngine.UI.Button) && entry.ButtonComponent != null)
            prefab = entry.ButtonComponent.gameObject;
        else if (typeof(T) == typeof(TMP_Text) && entry.TextComponent != null)
            prefab = entry.TextComponent.gameObject;
        else if (typeof(T) == typeof(TMP_InputField) && entry.InputFieldComponent != null)
            prefab = entry.InputFieldComponent.gameObject;

        if (prefab == null)
        {
            Debug.LogError($"Prefab for {typeof(T)} with key {key} is not assigned.");
            return null;
        }
        var instance = GameObject.Instantiate(prefab, parent);
        return instance.GetComponent<T>();
    }
    public Canvas FindCanvasByTag(string tag)
    {
        GameObject canvasObject = GameObject.FindWithTag(tag);
        if (canvasObject != null)
        {
            return canvasObject.GetComponent<Canvas>();
        }
        Debug.LogError($"Canvas with tag {tag} not found!");
        return null;
    }
}
