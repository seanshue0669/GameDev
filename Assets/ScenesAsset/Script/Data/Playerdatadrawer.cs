using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PlayerData.PlayerDataEntry))]
public class SharedDataEntryDrawer : PropertyDrawer
{
    private const float padding = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 开始绘制属性
        EditorGUI.BeginProperty(position, label, property);

        // 保存原始缩进
        int originalIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // 计算各个字段的位置
        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect keyRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect typeRect = new Rect(position.x, position.y + lineHeight + padding, position.width, lineHeight);
        Rect valueRect = new Rect(position.x, position.y + 2 * (lineHeight + padding), position.width, lineHeight);

        // 绘制 Key 字段
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("Key"), new GUIContent("Key"));

        // 绘制 Type 枚举字段
        SerializedProperty typeProp = property.FindPropertyRelative("Type");
        EditorGUI.PropertyField(typeRect, typeProp, new GUIContent("Type"));

        // 根据 Type 显示相应的值字段
        PlayerData.DataType dataType = (PlayerData.DataType)typeProp.enumValueIndex;

        switch (dataType)
        {
            case PlayerData.DataType.String:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("StringValue"), new GUIContent("Value (String)"));
                break;
            case PlayerData.DataType.Int:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("IntValue"), new GUIContent("Value (Int)"));
                break;
            case PlayerData.DataType.Bool:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("BoolValue"), new GUIContent("Value (Bool)"));
                break;
            case PlayerData.DataType.Float:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("FloatValue"), new GUIContent("Value (Float)"));
                break;
            case PlayerData.DataType.IntList:
                {
                    SerializedProperty intListProp = property.FindPropertyRelative("IntList");
                    Rect listRect = new Rect(position.x, position.y + 2 * (lineHeight + padding), position.width, EditorGUI.GetPropertyHeight(intListProp, true));
                    EditorGUI.PropertyField(listRect, intListProp, new GUIContent("Value (List<int>)"), includeChildren: true);
                    break;
                }
            case PlayerData.DataType.FloatList:
                {
                    SerializedProperty floatListProp = property.FindPropertyRelative("FloatList");
                    Rect listRect = new Rect(position.x, position.y + 2 * (lineHeight + padding), position.width, EditorGUI.GetPropertyHeight(floatListProp, true));
                    EditorGUI.PropertyField(listRect, floatListProp, new GUIContent("Value (List<float>)"), includeChildren: true);
                    break;
                }
            case PlayerData.DataType.task:
                {
                    SerializedProperty taskProp = property.FindPropertyRelative("task");
                    Rect taskRect = new Rect(position.x, position.y + 2 * (lineHeight + padding), position.width, EditorGUI.GetPropertyHeight(taskProp, true));
                    EditorGUI.PropertyField(taskRect, taskProp, new GUIContent("Value (Task)"), includeChildren: true);
                    break;
                }
        }

        // 恢复原始缩进
        EditorGUI.indentLevel = originalIndent;

        // 结束绘制属性
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 基础高度：Key + Type + Value，每个一行
        float height = 3 * EditorGUIUtility.singleLineHeight + 2 * padding;

        // 获取当前选择的类型
        SerializedProperty typeProp = property.FindPropertyRelative("Type");
        PlayerData.DataType dataType = (PlayerData.DataType)typeProp.enumValueIndex;

        // 如果是 IntList, FloatList 或 Task，增加其内容的高度
        switch (dataType)
        {
            case PlayerData.DataType.IntList:
                {
                    SerializedProperty intListProp = property.FindPropertyRelative("IntList");
                    height += EditorGUI.GetPropertyHeight(intListProp, true) - EditorGUIUtility.singleLineHeight;
                    break;
                }
            case PlayerData.DataType.FloatList:
                {
                    SerializedProperty floatListProp = property.FindPropertyRelative("FloatList");
                    height += EditorGUI.GetPropertyHeight(floatListProp, true) - EditorGUIUtility.singleLineHeight;
                    break;
                }
            case PlayerData.DataType.task:
                {
                    SerializedProperty taskProp = property.FindPropertyRelative("task");
                    height += EditorGUI.GetPropertyHeight(taskProp, true) - EditorGUIUtility.singleLineHeight;
                    break;
                }
        }

        return height;
    }
}
