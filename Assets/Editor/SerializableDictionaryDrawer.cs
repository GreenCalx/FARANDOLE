using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    private const float RowPadding = 2f;
    private const float ButtonWidth = 22f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var items = property.FindPropertyRelative("items");

        float height = EditorGUIUtility.singleLineHeight; // label

        for (int i = 0; i < items.arraySize; i++)
        {
            var element = items.GetArrayElementAtIndex(i);
            var key = element.FindPropertyRelative("Key");
            var value = element.FindPropertyRelative("Value");

            float keyHeight = EditorGUI.GetPropertyHeight(key, GUIContent.none, true);
            float valueHeight = EditorGUI.GetPropertyHeight(value, GUIContent.none, true);

            height += Mathf.Max(keyHeight, valueHeight) + RowPadding;
        }

        // Add button
        height += EditorGUIUtility.singleLineHeight + RowPadding;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var items = property.FindPropertyRelative("items");

        // Label
        Rect row = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(row, label);
        row.y += row.height + RowPadding;

        // Entries
        for (int i = 0; i < items.arraySize; i++)
        {
            var element = items.GetArrayElementAtIndex(i);
            var key = element.FindPropertyRelative("Key");
            var value = element.FindPropertyRelative("Value");

            float keyWidth = (row.width - ButtonWidth) * 0.4f;
            float valueWidth = (row.width - ButtonWidth) * 0.6f;

            float keyHeight = EditorGUI.GetPropertyHeight(key, GUIContent.none, true);
            float valueHeight = EditorGUI.GetPropertyHeight(value, GUIContent.none, true);

            float rowHeight = Mathf.Max(keyHeight, valueHeight);

            Rect keyRect = new Rect(row.x, row.y, keyWidth, rowHeight);
            Rect valueRect = new Rect(row.x + keyWidth + 4, row.y, valueWidth - 4, rowHeight);
            Rect removeRect = new Rect(row.x + row.width - ButtonWidth, row.y, ButtonWidth, rowHeight);

            EditorGUI.PropertyField(keyRect, key, GUIContent.none, true);
            EditorGUI.PropertyField(valueRect, value, GUIContent.none, true);

            if (GUI.Button(removeRect, "–"))
            {
                items.DeleteArrayElementAtIndex(i);
                break;
            }

            row.y += rowHeight + RowPadding;
        }

        // Add button
        Rect addRect = new Rect(
            position.x,
            row.y,
            position.width,
            EditorGUIUtility.singleLineHeight);

        if (GUI.Button(addRect, "Add"))
        {
            items.arraySize++;
        }

        EditorGUI.EndProperty();
    }
}
