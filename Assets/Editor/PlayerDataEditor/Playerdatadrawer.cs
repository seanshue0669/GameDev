using UnityEngine;
using UnityEditor;
using PlayerDataSO;
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PlayerData.PlayerDataEntry))]
public class PlayerDataEntryDrawer : PropertyDrawer
{
    private const float padding = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin property drawing
        EditorGUI.BeginProperty(position, label, property);

        // Save original indent
        int originalIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate field positions
        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect keyRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect typeRect = new Rect(position.x, position.y + lineHeight + padding, position.width, lineHeight);
        Rect valueRect = new Rect(position.x, position.y + 2 * (lineHeight + padding), position.width, lineHeight);

        // Draw Key field
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("Key"), new GUIContent("Key"));

        // Draw Type enum field
        SerializedProperty typeProp = property.FindPropertyRelative("Type");
        EditorGUI.PropertyField(typeRect, typeProp, new GUIContent("Type"));

        // Determine which value field to draw based on DataType
        PlayerData.DataType dataType = (PlayerData.DataType)typeProp.enumValueIndex;

        switch (dataType)
        {
            case PlayerData.DataType.Int:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("IntValue"), new GUIContent("Value (Int)"));
                break;
            case PlayerData.DataType.Bool:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("BoolValue"), new GUIContent("Value (Bool)"));
                break;
            case PlayerData.DataType.Float:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("FloatValue"), new GUIContent("Value (Float)"));
                break;
            case PlayerData.DataType.String:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("StringValue"), new GUIContent("Value (String)"));
                break;
        }

        // Restore original indent
        EditorGUI.indentLevel = originalIndent;

        // End property drawing
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Base height: Key + Type + Value each on a separate line
        return 3 * EditorGUIUtility.singleLineHeight + 2 * padding;
    }
}
#endif