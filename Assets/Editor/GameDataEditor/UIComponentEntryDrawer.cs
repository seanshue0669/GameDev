using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UIComponentCollectionSO.UIComponentEntry))]
public class UIComponentEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight + 2;

        Rect typeRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect keyRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
        Rect componentRect = new Rect(position.x, position.y + 2 * lineHeight, position.width, lineHeight);

        SerializedProperty keyProp = property.FindPropertyRelative("Key");
        SerializedProperty typeProp = property.FindPropertyRelative("Type");
        SerializedProperty buttonProp = property.FindPropertyRelative("ButtonComponent");
        SerializedProperty textProp = property.FindPropertyRelative("TextComponent");
        SerializedProperty inputFieldProp = property.FindPropertyRelative("InputFieldComponent");


        if (keyProp == null || typeProp == null)
        {
            EditorGUI.HelpBox(position, "Property fields are not initialized correctly!", MessageType.Error);
            return;
        }

        EditorGUI.PropertyField(typeRect, typeProp, new GUIContent("Type"));
        EditorGUI.PropertyField(keyRect, keyProp, new GUIContent("Key"));


        UIComponentCollectionSO.UIComponentType type = (UIComponentCollectionSO.UIComponentType)typeProp.enumValueIndex;

        switch (type)
        {
            case UIComponentCollectionSO.UIComponentType.Button:
                if (buttonProp != null)
                {
                    EditorGUI.PropertyField(componentRect, buttonProp, new GUIContent("Button"));
                }
                break;

            case UIComponentCollectionSO.UIComponentType.TMP_Text:
                if (textProp != null)
                {
                    EditorGUI.PropertyField(componentRect, textProp, new GUIContent("TMP_Text"));
                }
                break;

            case UIComponentCollectionSO.UIComponentType.TMP_InputField:
                if (inputFieldProp != null)
                {
                    EditorGUI.PropertyField(componentRect, inputFieldProp, new GUIContent("TMP_InputField"));
                }   
                break;

            default:
                EditorGUI.HelpBox(componentRect, "Unknown Type!", MessageType.Error);
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 3 * (EditorGUIUtility.singleLineHeight + 2);
    }
}
