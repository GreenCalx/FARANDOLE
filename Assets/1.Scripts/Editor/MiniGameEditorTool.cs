using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Editor tool to browse, create, and manage minigames.
/// Access via Tools > MiniGame Editor Tool
///
/// - Browse the MiniGameBank collection with quick access to prefabs and descriptors.
/// - Create new minigames (script, SO, prefab) in a two-phase flow.
/// </summary>
public class MiniGameEditorTool : EditorWindow
{
    // ─────────────────────────────────────────────────────
    // CREATION INPUT FIELDS
    // ─────────────────────────────────────────────────────

    private string miniGameName = "";
    private string goal = "";
    private Sprite thumbnail;
    private EMiniGameFamily family = EMiniGameFamily.REGULAR;
    private SCREEN_ORIENTATION orientation = SCREEN_ORIENTATION.PORTRAIT;
    private int manualID = -1;

    // ─────────────────────────────────────────────────────
    // PATHS
    // ─────────────────────────────────────────────────────

    private const string SCRIPTS_ROOT = "Assets/1.Scripts/MiniGames";
    private const string PRESETS_ROOT = "Assets/5.Presets/MiniGames";
    private const string PREFABS_ROOT = "Assets/2.Prefabs/_MiniGames";
    private const string BANK_PATH = "Assets/5.Presets/MiniGameBank.asset";

    // ─────────────────────────────────────────────────────
    // PENDING STATE (persists through recompile)
    // ─────────────────────────────────────────────────────

    private const string PREF_PENDING = "MGEditorTool_Pending";
    private const string PREF_CLASS_NAME = "MGEditorTool_ClassName";
    private const string PREF_SO_PATH = "MGEditorTool_SOPath";
    private const string PREF_PREFAB_PATH = "MGEditorTool_PrefabPath";

    // ─────────────────────────────────────────────────────
    // INTERNAL STATE
    // ─────────────────────────────────────────────────────

    private MiniGameBankSO cachedBank;
    private Vector2 scrollPos;
    private bool showAdvanced = false;
    private bool collectionFoldout = true;
    private bool creationFoldout = false;

    // Cached icons
    private GUIContent prefabIcon;
    private GUIContent soIcon;

    [MenuItem("Tools/MiniGame Editor Tool")]
    public static void ShowWindow()
    {
        var window = GetWindow<MiniGameEditorTool>("MiniGame Editor");
        window.minSize = new Vector2(360, 500);
    }

    private void OnEnable()
    {
        cachedBank = AssetDatabase.LoadAssetAtPath<MiniGameBankSO>(BANK_PATH);
        prefabIcon = EditorGUIUtility.IconContent("Prefab Icon");
        soIcon = EditorGUIUtility.IconContent("ScriptableObject Icon");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawHeader();

        // Pending finalization takes priority
        if (SessionState.GetBool(PREF_PENDING, false))
        {
            DrawPendingFinalization();
        }
        else
        {
            DrawCollection();
            EditorGUILayout.Space(10);
            DrawCreationForm();
        }

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
        GUILayout.Label("MiniGame Editor Tool", titleStyle);
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
            if (cachedBank == null || cachedBank.GameBank == null)
            {
                EditorGUILayout.HelpBox("MiniGameBank not found.", MessageType.Error);
                if (GUILayout.Button("Refresh"))
                    cachedBank = AssetDatabase.LoadAssetAtPath<MiniGameBankSO>(BANK_PATH);

                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            EditorGUILayout.Space(3);

            // Column headers
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("ID", EditorStyles.miniLabel, GUILayout.Width(24));
                EditorGUILayout.LabelField("Name", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("Family", EditorStyles.miniLabel, GUILayout.Width(65));
                GUILayout.Space(52); // space for buttons
            }

            // Draw each row
            for (int i = 0; i < cachedBank.GameBank.Count; i++)
            {
                MiniGameSO mg = cachedBank.GameBank[i];
                if (mg == null) continue;
                DrawCollectionRow(mg);
            }

            EditorGUILayout.Space(3);
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.LabelField($"{cachedBank.GameBank.Count} minigames in bank", EditorStyles.centeredGreyMiniLabel);
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawCollectionRow(MiniGameSO mg)
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            // ID
            EditorGUILayout.LabelField(mg.ID.ToString(), GUILayout.Width(24));

            // Name
            EditorGUILayout.LabelField(mg.name, EditorStyles.boldLabel);

            // Family
            EditorGUILayout.LabelField(mg.family.ToString(), EditorStyles.miniLabel, GUILayout.Width(65));

            // Open Prefab button
            using (new EditorGUI.DisabledGroupScope(mg.prefab_MiniGame == null))
            {
                if (GUILayout.Button(prefabIcon, GUILayout.Width(24), GUILayout.Height(20)))
                {
                    AssetDatabase.OpenAsset(mg.prefab_MiniGame);
                }
            }

            // Select SO in inspector button
            if (GUILayout.Button(soIcon, GUILayout.Width(24), GUILayout.Height(20)))
            {
                Selection.activeObject = mg;
                EditorGUIUtility.PingObject(mg);
            }
        }
    }

    // ═════════════════════════════════════════════════════
    //  CREATION FORM (Phase 1)
    // ═════════════════════════════════════════════════════

    private void DrawCreationForm()
    {
        creationFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(creationFoldout, "Create New MiniGame");

        if (creationFoldout)
        {
            EditorGUILayout.Space(5);

            miniGameName = EditorGUILayout.TextField("Name", miniGameName);
            goal = EditorGUILayout.TextField("Goal", goal);
            thumbnail = (Sprite)EditorGUILayout.ObjectField("Thumbnail", thumbnail, typeof(Sprite), false);
            family = (EMiniGameFamily)EditorGUILayout.EnumPopup("Family", family);
            orientation = (SCREEN_ORIENTATION)EditorGUILayout.EnumPopup("Orientation", orientation);

            // Advanced
            EditorGUILayout.Space(3);
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced");
            if (showAdvanced)
            {
                int nextID = GetNextAvailableID();
                EditorGUILayout.HelpBox($"Next available ID: {nextID}", MessageType.Info);
                manualID = EditorGUILayout.IntField("Manual ID (-1 = auto)", manualID);
            }

            // Preview
            string className = SanitizeClassName(miniGameName);

            if (!string.IsNullOrEmpty(className))
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.TextField("Class", $"{className}MiniGame");
                    EditorGUILayout.TextField("Folder", $"{SCRIPTS_ROOT}/{className}_MiniGame/");
                    EditorGUILayout.TextField("SO", $"{PRESETS_ROOT}/MG_{className}.asset");
                    EditorGUILayout.TextField("Prefab", $"{PREFABS_ROOT}/{className}MiniGame.prefab");
                }
            }

            // Validation
            EditorGUILayout.Space(8);
            List<string> errors = ValidateInput(className);
            foreach (string err in errors)
            {
                EditorGUILayout.HelpBox(err, MessageType.Error);
            }

            EditorGUILayout.Space(3);
            using (new EditorGUI.DisabledGroupScope(errors.Count > 0))
            {
                GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
                if (GUILayout.Button("Create MiniGame", GUILayout.Height(35)))
                {
                    ExecutePhase1(className);
                }
                GUI.backgroundColor = Color.white;
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    // ═════════════════════════════════════════════════════
    //  PENDING FINALIZATION (Phase 2)
    // ═════════════════════════════════════════════════════

    private void DrawPendingFinalization()
    {
        string className = SessionState.GetString(PREF_CLASS_NAME, "");
        string soPath = SessionState.GetString(PREF_SO_PATH, "");
        string prefabPath = SessionState.GetString(PREF_PREFAB_PATH, "");

        EditorGUILayout.HelpBox(
            $"Pending finalization for: {className}MiniGame\n\n" +
            "Scripts have been created. After Unity finishes compiling, " +
            "click 'Finalize' to create the prefab and register in the bank.",
            MessageType.Warning);

        EditorGUILayout.Space(10);

        System.Type miniGameType = FindType($"{className}MiniGame");
        bool isCompiled = miniGameType != null;

        if (!isCompiled)
        {
            EditorGUILayout.HelpBox("Waiting for Unity to compile...", MessageType.Info);
            Repaint();
        }

        EditorGUILayout.Space(5);

        using (new EditorGUI.DisabledGroupScope(!isCompiled))
        {
            GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);
            if (GUILayout.Button("Finalize", GUILayout.Height(40)))
            {
                ExecutePhase2(className, soPath, prefabPath, miniGameType);
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.Space(5);
        if (GUILayout.Button("Cancel"))
        {
            ClearPendingState();
        }
    }

    // ═════════════════════════════════════════════════════
    //  PHASE 1: Create script, folder, SO
    // ═════════════════════════════════════════════════════

    private void ExecutePhase1(string className)
    {
        string folderName = $"{className}_MiniGame";
        string scriptFolder = $"{SCRIPTS_ROOT}/{folderName}";
        string scriptPath = $"{scriptFolder}/{className}MiniGame.cs";
        string soPath = $"{PRESETS_ROOT}/MG_{className}.asset";
        string prefabPath = $"{PREFABS_ROOT}/{className}MiniGame.prefab";
        int id = manualID >= 0 ? manualID : GetNextAvailableID();
        string familyInterface = GetFamilyInterface(family);

        // 1. Create script folder
        string fullScriptFolder = Path.Combine(Application.dataPath, scriptFolder.Substring("Assets/".Length));
        if (!Directory.Exists(fullScriptFolder))
        {
            Directory.CreateDirectory(fullScriptFolder);
            Debug.Log($"[MiniGameEditorTool] Created folder: {scriptFolder}");
        }

        // 2. Generate script
        string scriptContent = GenerateScript(className, familyInterface);
        string fullScriptPath = Path.Combine(Application.dataPath, scriptPath.Substring("Assets/".Length));
        File.WriteAllText(fullScriptPath, scriptContent);
        Debug.Log($"[MiniGameEditorTool] Created script: {scriptPath}");

        // 3. Create MiniGameSO asset
        string fullPresetsFolder = Path.Combine(Application.dataPath, PRESETS_ROOT.Substring("Assets/".Length));
        if (!Directory.Exists(fullPresetsFolder))
            Directory.CreateDirectory(fullPresetsFolder);

        AssetDatabase.Refresh();

        MiniGameSO so = ScriptableObject.CreateInstance<MiniGameSO>();
        so.ID = (byte)id;
        so.name = miniGameName;
        so.goal = goal;
        so.thumbNailImg = thumbnail;
        so.family = family;
        so.orientationRequirement = orientation;
        so.compatibleMods = new List<EMiniGameMods>();

        AssetDatabase.CreateAsset(so, soPath);
        AssetDatabase.SaveAssets();
        Debug.Log($"[MiniGameEditorTool] Created SO: {soPath} (ID: {id})");

        // 4. Store pending state for Phase 2
        SessionState.SetBool(PREF_PENDING, true);
        SessionState.SetString(PREF_CLASS_NAME, className);
        SessionState.SetString(PREF_SO_PATH, soPath);
        SessionState.SetString(PREF_PREFAB_PATH, prefabPath);

        // 5. Trigger recompile
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Phase 1 Complete",
            $"Script and SO created for '{miniGameName}'.\n\n" +
            "Wait for Unity to compile, then click 'Finalize'.",
            "OK");
    }

    // ═════════════════════════════════════════════════════
    //  PHASE 2: Create prefab, link SO, add to bank
    // ═════════════════════════════════════════════════════

    private void ExecutePhase2(string className, string soPath, string prefabPath, System.Type miniGameType)
    {
        // 1. Create prefab with component
        GameObject go = new GameObject($"{className}MiniGame");
        Component mgComponent = go.AddComponent(miniGameType);

        // Link the MiniGameSO descriptor
        MiniGameSO so = AssetDatabase.LoadAssetAtPath<MiniGameSO>(soPath);
        if (so != null && mgComponent != null)
        {
            SerializedObject serializedComp = new SerializedObject(mgComponent);
            SerializedProperty descProp = serializedComp.FindProperty("descriptor");
            if (descProp != null)
            {
                descProp.objectReferenceValue = so;
                serializedComp.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        DestroyImmediate(go);
        Debug.Log($"[MiniGameEditorTool] Created prefab: {prefabPath}");

        // 2. Link prefab to SO
        if (so != null && prefab != null)
        {
            so.prefab_MiniGame = prefab;
            EditorUtility.SetDirty(so);
        }

        // 3. Add SO to MiniGameBank
        MiniGameBankSO bank = AssetDatabase.LoadAssetAtPath<MiniGameBankSO>(BANK_PATH);
        if (bank != null && so != null)
        {
            if (!bank.GameBank.Contains(so))
            {
                bank.GameBank.Add(so);
                EditorUtility.SetDirty(bank);
                Debug.Log($"[MiniGameEditorTool] Added to MiniGameBank");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Refresh cached bank
        cachedBank = AssetDatabase.LoadAssetAtPath<MiniGameBankSO>(BANK_PATH);

        ClearPendingState();

        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);

        EditorUtility.DisplayDialog(
            "MiniGame Created!",
            $"'{className}MiniGame' is ready.\n\n" +
            $"  Script: {SCRIPTS_ROOT}/{className}_MiniGame/{className}MiniGame.cs\n" +
            $"  SO: {soPath}\n" +
            $"  Prefab: {prefabPath}\n" +
            $"  Added to MiniGameBank",
            "OK");
    }

    // ═════════════════════════════════════════════════════
    //  SCRIPT GENERATION
    // ═════════════════════════════════════════════════════

    private string GenerateScript(string className, string familyInterface)
    {
        string fullClassName = $"{className}MiniGame";
        string familyDecl = string.IsNullOrEmpty(familyInterface) ? "" : $", {familyInterface}";

        return
$@"using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class {fullClassName} : MiniGame{familyDecl}
{{
    [Header(""{fullClassName}"")]
    // Add your game-specific fields here

    public override void Init()
    {{
        base.Init();
    }}

    public override void Reset()
    {{
        base.Reset();
        // Setup / reset game state for a new round
    }}

    public override void Play()
    {{
        base.Play();
        // Called when the minigame starts
    }}

    public override void Stop()
    {{
        base.Stop();
        // Cleanup when the minigame ends
    }}

    public override void Win()
    {{
        base.Win();
    }}

    public override void Lose()
    {{
        base.Lose();
    }}

    public override bool SuccessCheck()
    {{
        // Return true when the player has met the win condition
        return false;
    }}
}}
";
    }

    // ═════════════════════════════════════════════════════
    //  HELPERS
    // ═════════════════════════════════════════════════════

    private string SanitizeClassName(string input)
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

    private string GetFamilyInterface(EMiniGameFamily fam)
    {
        switch (fam)
        {
            case EMiniGameFamily.REGULAR: return "IRegularFamily";
            case EMiniGameFamily.DOG: return "IDogFamily";
            case EMiniGameFamily.CHESS: return "IChessFamily";
            case EMiniGameFamily.ARCADE: return "IArcadeFamily";
            case EMiniGameFamily.PHYSICS: return "IPhysicsFamily";
            default: return "";
        }
    }

    private int GetNextAvailableID()
    {
        if (cachedBank == null)
            cachedBank = AssetDatabase.LoadAssetAtPath<MiniGameBankSO>(BANK_PATH);

        if (cachedBank == null || cachedBank.GameBank == null)
            return 0;

        int maxID = 0;
        foreach (var mg in cachedBank.GameBank)
        {
            if (mg != null && mg.ID > maxID)
                maxID = mg.ID;
        }
        return maxID + 1;
    }

    private List<string> ValidateInput(string className)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(miniGameName))
            errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(className))
            return errors;

        string scriptFolder = $"{SCRIPTS_ROOT}/{className}_MiniGame";
        string fullPath = Path.Combine(Application.dataPath, scriptFolder.Substring("Assets/".Length));
        if (Directory.Exists(fullPath))
            errors.Add($"Folder already exists: {scriptFolder}");

        string soPath = $"{PRESETS_ROOT}/MG_{className}.asset";
        if (AssetDatabase.LoadAssetAtPath<MiniGameSO>(soPath) != null)
            errors.Add($"SO already exists: {soPath}");

        string prefabPath = $"{PREFABS_ROOT}/{className}MiniGame.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            errors.Add($"Prefab already exists: {prefabPath}");

        return errors;
    }

    private System.Type FindType(string typeName)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }

    private void ClearPendingState()
    {
        SessionState.SetBool(PREF_PENDING, false);
        SessionState.SetString(PREF_CLASS_NAME, "");
        SessionState.SetString(PREF_SO_PATH, "");
        SessionState.SetString(PREF_PREFAB_PATH, "");
    }
}
