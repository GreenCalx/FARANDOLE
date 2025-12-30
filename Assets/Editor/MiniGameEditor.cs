using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomEditor(typeof(MiniGame), true)]
public class MiniGameEditor : Editor
{
    private SerializedProperty extensionsProp;
    private Type[] extensionTypes;

    private void OnEnable()
    {
        extensionsProp = serializedObject.FindProperty("extensions");

        extensionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .Where(t =>
                typeof(MiniGameExtension).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                !t.IsGenericType)
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultPropertiesExceptExtensions();

        EditorGUILayout.Space(10);
        DrawExtensionsSection();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultPropertiesExceptExtensions()
    {
        var iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (iterator.propertyPath == extensionsProp.propertyPath)
                continue;

            EditorGUILayout.PropertyField(iterator, true);
        }
    }

    private void DrawExtensionsSection()
    {
        EditorGUILayout.LabelField("MiniGame Extensions", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        if (extensionsProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No extensions added.", MessageType.Info);
        }

        for (int i = 0; i < extensionsProp.arraySize; i++)
        {
            var element = extensionsProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            DrawExtensionHeader(i, element);
            DrawExtensionBody(element);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);
        }

        DrawAddButton();
    }

    private void DrawExtensionHeader(int index, SerializedProperty element)
    {
        EditorGUILayout.BeginHorizontal();

        var instance = element.managedReferenceValue;
        string typeName = instance != null
            ? instance.GetType().Name
            : "Missing Extension";

        EditorGUILayout.LabelField(typeName, EditorStyles.boldLabel);

        if (GUILayout.Button("Remove", GUILayout.Width(70)))
        {
            extensionsProp.DeleteArrayElementAtIndex(index);
            // Avoid ObjectDisposedException by breaking the loop
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawExtensionBody(SerializedProperty element)
    {
        if (element.managedReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Missing extension instance.", MessageType.Error);
            return;
        }

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(element, true);
        EditorGUI.indentLevel--;
    }

    private void DrawAddButton()
    {
        if (GUILayout.Button("Add Extension"))
        {
            var menu = new GenericMenu();

            foreach (var type in extensionTypes)
            {
                menu.AddItem(
                    new GUIContent(type.Name),
                    false,
                    () => AddExtension(type));
            }

            menu.ShowAsContext();
        }
    }

    private void AddExtension(Type type)
    {
        serializedObject.Update();

        extensionsProp.arraySize++;
        var element = extensionsProp.GetArrayElementAtIndex(extensionsProp.arraySize - 1);
        element.managedReferenceValue = Activator.CreateInstance(type);

        serializedObject.ApplyModifiedProperties();
    }
}
