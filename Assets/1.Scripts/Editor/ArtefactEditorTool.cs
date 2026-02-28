using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Editor tool to browse and create artefacts.
/// Access via Tools > Artefact Editor Tool
/// </summary>
public class ArtefactEditorTool : EditorWindow
{
    // ─────────────────────────────────────────────────────
    // CREATION INPUT FIELDS
    // ─────────────────────────────────────────────────────

    private string artefactName = "";
    private string artefactDescription = "";
    private Sprite artefactIcon;

    // ─────────────────────────────────────────────────────
    // PATHS
    // ─────────────────────────────────────────────────────

    private const string ARTEFACTS_FOLDER = "Assets/5.Presets/Artefacts";
    private const string COLLECTION_PATH = "Assets/5.Presets/Artefacts/_Artefacts.asset";

    // ─────────────────────────────────────────────────────
    // INTERNAL STATE
    // ─────────────────────────────────────────────────────

    private ArtefactCollectionSO cachedCollection;
    private Vector2 scrollPos;
    private bool collectionFoldout = true;
    private bool creationFoldout = false;

    private GUIContent soIcon;

    [MenuItem("Tools/Artefact Editor Tool")]
    public static void ShowWindow()
    {
        var window = GetWindow<ArtefactEditorTool>("Artefact Editor");
        window.minSize = new Vector2(340, 400);
    }

    private void OnEnable()
    {
        cachedCollection = AssetDatabase.LoadAssetAtPath<ArtefactCollectionSO>(COLLECTION_PATH);
        soIcon = EditorGUIUtility.IconContent("ScriptableObject Icon");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawHeader();
        DrawCollection();
        EditorGUILayout.Space(10);
        DrawCreationForm();

        EditorGUILayout.EndScrollView();
    }

    // ═════════════════════════════════════════════════════
    //  HEADER
    // ═════════════════════════════════════════════════════

    private void DrawHeader()
    {
        EditorGUILayout.Space(10);
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Label("Artefact Editor Tool", titleStyle);
        EditorGUILayout.Space(10);
    }

    // ═════════════════════════════════════════════════════
    //  COLLECTION BROWSER
    // ═════════════════════════════════════════════════════

    private void DrawCollection()
    {
        collectionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(collectionFoldout, "Collection");

        if (collectionFoldout)
        {
            if (cachedCollection == null || cachedCollection.artefacts == null)
            {
                EditorGUILayout.HelpBox("ArtefactCollection not found.", MessageType.Error);
                if (GUILayout.Button("Refresh"))
                    cachedCollection = AssetDatabase.LoadAssetAtPath<ArtefactCollectionSO>(COLLECTION_PATH);

                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            EditorGUILayout.Space(3);

            // Column headers
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(40); // icon space
                EditorGUILayout.LabelField("Name", EditorStyles.miniLabel);
                GUILayout.Space(28); // button space
            }

            for (int i = 0; i < cachedCollection.artefacts.Count; i++)
            {
                ArtefactSO art = cachedCollection.artefacts[i];
                if (art == null) continue;
                DrawCollectionRow(art);
            }

            EditorGUILayout.Space(3);
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.LabelField($"{cachedCollection.artefacts.Count} artefacts in collection", EditorStyles.centeredGreyMiniLabel);
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawCollectionRow(ArtefactSO art)
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            // Icon preview
            if (art.icon != null)
            {
                Texture2D tex = AssetPreview.GetAssetPreview(art.icon);
                if (tex != null)
                {
                    GUILayout.Label(tex, GUILayout.Width(32), GUILayout.Height(32));
                }
                else
                {
                    GUILayout.Space(36);
                }
            }
            else
            {
                GUILayout.Space(36);
            }

            // Name + description
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField(art.name, EditorStyles.boldLabel);
                if (!string.IsNullOrEmpty(art.descriptiion))
                {
                    EditorGUILayout.LabelField(art.descriptiion, EditorStyles.wordWrappedMiniLabel);
                }
            }

            // Select SO button
            if (GUILayout.Button(soIcon, GUILayout.Width(24), GUILayout.Height(32)))
            {
                Selection.activeObject = art;
                EditorGUIUtility.PingObject(art);
            }
        }
    }

    // ═════════════════════════════════════════════════════
    //  CREATION FORM
    // ═════════════════════════════════════════════════════

    private void DrawCreationForm()
    {
        creationFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(creationFoldout, "Create New Artefact");

        if (creationFoldout)
        {
            EditorGUILayout.Space(5);

            artefactName = EditorGUILayout.TextField("Name", artefactName);
            artefactDescription = EditorGUILayout.TextField("Description", artefactDescription);
            artefactIcon = (Sprite)EditorGUILayout.ObjectField("Icon", artefactIcon, typeof(Sprite), false);

            // Preview
            string safeName = SanitizeName(artefactName);

            if (!string.IsNullOrEmpty(safeName))
            {
                EditorGUILayout.Space(5);
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.TextField("Asset", $"{ARTEFACTS_FOLDER}/{safeName}.asset");
                }
            }

            // Validation
            EditorGUILayout.Space(8);
            List<string> errors = ValidateInput(safeName);
            foreach (string err in errors)
            {
                EditorGUILayout.HelpBox(err, MessageType.Error);
            }

            EditorGUILayout.Space(3);
            using (new EditorGUI.DisabledGroupScope(errors.Count > 0))
            {
                GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
                if (GUILayout.Button("Create Artefact", GUILayout.Height(35)))
                {
                    CreateArtefact(safeName);
                }
                GUI.backgroundColor = Color.white;
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    // ═════════════════════════════════════════════════════
    //  CREATION
    // ═════════════════════════════════════════════════════

    private void CreateArtefact(string safeName)
    {
        string assetPath = $"{ARTEFACTS_FOLDER}/{safeName}.asset";

        // Ensure folder exists
        string fullFolder = Path.Combine(Application.dataPath, ARTEFACTS_FOLDER.Substring("Assets/".Length));
        if (!Directory.Exists(fullFolder))
            Directory.CreateDirectory(fullFolder);

        // Create SO
        ArtefactSO so = ScriptableObject.CreateInstance<ArtefactSO>();
        so.name = artefactName;
        so.descriptiion = artefactDescription;
        so.icon = artefactIcon;

        AssetDatabase.CreateAsset(so, assetPath);
        Debug.Log($"[ArtefactEditorTool] Created: {assetPath}");

        // Add to collection
        ArtefactCollectionSO collection = AssetDatabase.LoadAssetAtPath<ArtefactCollectionSO>(COLLECTION_PATH);
        if (collection != null)
        {
            if (collection.artefacts == null)
                collection.artefacts = new List<ArtefactSO>();

            collection.artefacts.Add(so);
            EditorUtility.SetDirty(collection);
            Debug.Log($"[ArtefactEditorTool] Added to collection");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Refresh cache
        cachedCollection = AssetDatabase.LoadAssetAtPath<ArtefactCollectionSO>(COLLECTION_PATH);

        // Select new asset
        Selection.activeObject = so;
        EditorGUIUtility.PingObject(so);

        // Clear form
        artefactName = "";
        artefactDescription = "";
        artefactIcon = null;
    }

    // ═════════════════════════════════════════════════════
    //  HELPERS
    // ═════════════════════════════════════════════════════

    private string SanitizeName(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        string[] words = input.Split(' ', '_', '-');
        string result = "";
        foreach (string w in words)
        {
            if (w.Length == 0) continue;
            result += char.ToUpper(w[0]) + w.Substring(1);
        }
        return result;
    }

    private List<string> ValidateInput(string safeName)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(artefactName))
            errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(safeName))
            return errors;

        string assetPath = $"{ARTEFACTS_FOLDER}/{safeName}.asset";
        if (AssetDatabase.LoadAssetAtPath<ArtefactSO>(assetPath) != null)
            errors.Add($"Asset already exists: {assetPath}");

        return errors;
    }
}
