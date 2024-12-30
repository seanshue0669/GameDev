using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SharedDataSO.SharedDataEntry))]
public class SharedDataEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        EditorGUI.BeginProperty(position, label, property);

        
        float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        Rect typeRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect keyRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
        Rect valueRect = new Rect(position.x, position.y + 2 * lineHeight, position.width, lineHeight);


        SerializedProperty keyProp = property.FindPropertyRelative("Key");
        SerializedProperty typeProp = property.FindPropertyRelative("Type");
        SerializedProperty stringValueProp = property.FindPropertyRelative("StringValue");
        SerializedProperty intValueProp = property.FindPropertyRelative("IntValue");
        SerializedProperty floatValueProp = property.FindPropertyRelative("FloatValue");

        EditorGUI.PropertyField(typeRect, typeProp, new GUIContent("Data Type"));
        EditorGUI.PropertyField(keyRect, keyProp, new GUIContent("Key"));

        
        SharedDataSO.DataType dataType = (SharedDataSO.DataType)typeProp.enumValueIndex;
        switch (dataType)
        {
            case SharedDataSO.DataType.String:
                EditorGUI.PropertyField(valueRect, stringValueProp, new GUIContent("Value (String)"));
                break;
            case SharedDataSO.DataType.Int:
                EditorGUI.PropertyField(valueRect, intValueProp, new GUIContent("Value (Int)"));
                break;
            case SharedDataSO.DataType.Float:
                EditorGUI.PropertyField(valueRect, floatValueProp, new GUIContent("Value (Float)"));
                break;
        }

        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        
        return 3 * (EditorGUIUtility.singleLineHeight + 2);
    }
}
