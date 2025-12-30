using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    private const float ButtonWidth = 20f;
    private const float Spacing = 4f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var items = property.FindPropertyRelative("items");
        float height = EditorGUIUtility.singleLineHeight * 2; // label + add button
        for (int i = 0; i < items.arraySize; i++)
        {
            var element = items.GetArrayElementAtIndex(i);
            var value = element.FindPropertyRelative("Value");
            if (IsNestedDictionary(value))
            {
                var innerItems = value.FindPropertyRelative("items");
                height += EditorGUIUtility.singleLineHeight * (innerItems.arraySize + 2);
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var items = property.FindPropertyRelative("items");
        var lineHeight = EditorGUIUtility.singleLineHeight;

        position.height = lineHeight;
        EditorGUI.LabelField(position, label);
        position.y += lineHeight + Spacing;

        for (int i = 0; i < items.arraySize; i++)
        {
            var element = items.GetArrayElementAtIndex(i);
            var key = element.FindPropertyRelative("Key");
            var value = element.FindPropertyRelative("Value");

            var keyRect = new Rect(position.x, position.y, position.width * 0.45f, lineHeight);
            var valueRect = new Rect(position.x + position.width * 0.45f + Spacing, position.y, position.width * 0.5f - ButtonWidth - Spacing, lineHeight);
            var buttonRect = new Rect(position.x + position.width * 0.95f - ButtonWidth, position.y, ButtonWidth, lineHeight);

            EditorGUI.PropertyField(keyRect, key, GUIContent.none);

            if (IsNestedDictionary(value))
            {
                // Draw nested dictionary with its own add/remove buttons
                DrawNestedDictionary(valueRect, value);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, value, GUIContent.none);
            }

            if (GUI.Button(buttonRect, "-"))
            {
                items.DeleteArrayElementAtIndex(i);
                break;
            }

            position.y += lineHeight + Spacing;
        }

        if (GUI.Button(new Rect(position.x, position.y, position.width, lineHeight), "Add"))
        {
            items.arraySize++;
            var newElement = items.GetArrayElementAtIndex(items.arraySize - 1);
            // Optionally set default key/value here
        }

        EditorGUI.EndProperty();
    }

    private void DrawNestedDictionary(Rect rect, SerializedProperty property)
    {
        var items = property.FindPropertyRelative("items");
        var lineHeight = EditorGUIUtility.singleLineHeight;
        var innerRect = new Rect(rect.x, rect.y, rect.width, lineHeight);

        for (int i = 0; i < items.arraySize; i++)
        {
            var element = items.GetArrayElementAtIndex(i);
            var key = element.FindPropertyRelative("Key");
            var value = element.FindPropertyRelative("Value");

            var keyRect = new Rect(innerRect.x, innerRect.y, innerRect.width * 0.45f, lineHeight);
            var valueRect = new Rect(innerRect.x + innerRect.width * 0.45f + Spacing, innerRect.y, innerRect.width * 0.5f - ButtonWidth - Spacing, lineHeight);
            var buttonRect = new Rect(innerRect.x + innerRect.width * 0.95f - ButtonWidth, innerRect.y, ButtonWidth, lineHeight);

            EditorGUI.PropertyField(keyRect, key, GUIContent.none);
            EditorGUI.PropertyField(valueRect, value, GUIContent.none);

            if (GUI.Button(buttonRect, "-"))
            {
                items.DeleteArrayElementAtIndex(i);
                break;
            }

            innerRect.y += lineHeight + Spacing;
        }

        if (GUI.Button(new Rect(innerRect.x, innerRect.y, innerRect.width, lineHeight), "Add"))
        {
            items.arraySize++;
        }
    }

    private bool IsNestedDictionary(SerializedProperty property)
    {
        return property.type.Contains("SerializableDictionary");
    }
}
