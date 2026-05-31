using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor tool to launch any game mode from any scene.
/// Access via Tools > Game Mode Launcher
/// </summary>
public class GameModeLauncher : EditorWindow
{
    private MiniGameBankSO miniGameBank;
    private int selectedMiniGameIndex = 0;
    private string[] miniGameNames;
    private Vector2 scrollPosition;

    private const string MINIGAME_BANK_PATH = "Assets/5.Presets/MiniGameBank.asset";
    private const string GAME_SCENE_PATH = "Assets/0.Scenes/Game.unity";

    [MenuItem("Tools/Game Mode Launcher %#g")] // Ctrl+Shift+G shortcut
    public static void ShowWindow()
    {
        var window = GetWindow<GameModeLauncher>("Game Launcher");
        window.minSize = new Vector2(280, 350);
    }

    private void OnEnable()
    {
        LoadMiniGameBank();
    }

    private void LoadMiniGameBank()
    {
        miniGameBank = AssetDatabase.LoadAssetAtPath<MiniGameBankSO>(MINIGAME_BANK_PATH);

        if (miniGameBank != null && miniGameBank.GameBank != null)
        {
            miniGameNames = new string[miniGameBank.GameBank.Count];
            for (int i = 0; i < miniGameBank.GameBank.Count; i++)
            {
                var mg = miniGameBank.GameBank[i];
                miniGameNames[i] = mg != null ? $"{i}: {mg.name}" : $"{i}: (null)";
            }
        }
        else
        {
            miniGameNames = null;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Label("Game Mode Launcher", titleStyle);

        EditorGUILayout.Space(15);

        // Main modes section
        EditorGUILayout.LabelField("Quick Launch", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (GUILayout.Button("RANDOM", GUILayout.Height(35)))
        {
            LaunchMode(GAME_MODE.RANDOM);
        }

        EditorGUILayout.Space(3);

        if (GUILayout.Button("DAILY SEED", GUILayout.Height(35)))
        {
            LaunchMode(GAME_MODE.DAILY_SEED);
        }

        EditorGUILayout.Space(3);

        if (GUILayout.Button("MUTATION", GUILayout.Height(35)))
        {
            LaunchMode(GAME_MODE.MUTATION);
        }

        EditorGUILayout.Space(20);

        // Singles mode section
        EditorGUILayout.LabelField("SINGLES Mode", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (miniGameBank == null)
        {
            EditorGUILayout.HelpBox($"MiniGameBank not found at:\n{MINIGAME_BANK_PATH}", MessageType.Error);

            if (GUILayout.Button("Refresh"))
            {
                LoadMiniGameBank();
            }
            return;
        }

        if (miniGameNames == null || miniGameNames.Length == 0)
        {
            EditorGUILayout.HelpBox("MiniGameBank is empty!", MessageType.Warning);
            return;
        }

        // Minigame selection dropdown
        selectedMiniGameIndex = EditorGUILayout.Popup("Select MiniGame", selectedMiniGameIndex, miniGameNames);

        // Show thumbnail preview if available
        if (selectedMiniGameIndex >= 0 && selectedMiniGameIndex < miniGameBank.GameBank.Count)
        {
            var selectedMG = miniGameBank.GameBank[selectedMiniGameIndex];
            if (selectedMG != null)
            {
                EditorGUILayout.Space(5);

                // Show thumbnail centered
                if (selectedMG.thumbNailImg != null)
                {
                    Texture2D tex = AssetPreview.GetAssetPreview(selectedMG.thumbNailImg);
                    if (tex != null)
                    {
                        Rect rect = GUILayoutUtility.GetRect(80, 80, GUILayout.ExpandWidth(true));
                        GUI.DrawTexture(new Rect(rect.x + (rect.width - 80) / 2, rect.y, 80, 80), tex, ScaleMode.ScaleToFit);
                    }
                }

                // Show minigame info
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Goal:", selectedMG.goal ?? "N/A", EditorStyles.wordWrappedLabel);
            }
        }

        EditorGUILayout.Space(10);

        GUI.backgroundColor = new Color(0.5f, 0.8f, 0.5f);
        if (GUILayout.Button("Launch SINGLES", GUILayout.Height(35)))
        {
            LaunchSingles(selectedMiniGameIndex);
        }
        GUI.backgroundColor = Color.white;

        // Prefab buttons
        EditorGUILayout.Space(5);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Open Prefab", GUILayout.Height(25)))
            {
                OpenSelectedPrefab();
            }
            if (GUILayout.Button("Select in Project", GUILayout.Height(25)))
            {
                SelectSelectedPrefab();
            }
        }

        // Footer
        EditorGUILayout.Space(15);
        EditorGUILayout.HelpBox("This tool will load the Game scene and auto-configure the selected game mode.", MessageType.Info);
    }

    private void OpenSelectedPrefab()
    {
        if (miniGameBank == null || selectedMiniGameIndex < 0 || selectedMiniGameIndex >= miniGameBank.GameBank.Count)
            return;

        var selectedMG = miniGameBank.GameBank[selectedMiniGameIndex];
        if (selectedMG == null || selectedMG.prefab_MiniGame == null)
        {
            EditorUtility.DisplayDialog("Error", "Selected minigame has no prefab assigned.", "OK");
            return;
        }

        // Open prefab in Prefab Mode
        string prefabPath = AssetDatabase.GetAssetPath(selectedMG.prefab_MiniGame);
        if (!string.IsNullOrEmpty(prefabPath))
        {
            AssetDatabase.OpenAsset(selectedMG.prefab_MiniGame);
        }
    }

    private void SelectSelectedPrefab()
    {
        if (miniGameBank == null || selectedMiniGameIndex < 0 || selectedMiniGameIndex >= miniGameBank.GameBank.Count)
            return;

        var selectedMG = miniGameBank.GameBank[selectedMiniGameIndex];
        if (selectedMG == null || selectedMG.prefab_MiniGame == null)
        {
            EditorUtility.DisplayDialog("Error", "Selected minigame has no prefab assigned.", "OK");
            return;
        }

        // Select and ping in Project window
        Selection.activeObject = selectedMG.prefab_MiniGame;
        EditorGUIUtility.PingObject(selectedMG.prefab_MiniGame);
    }

    private void LaunchMode(GAME_MODE mode)
    {
        SetLaunchData(mode, -1);
        StartPlayMode();
    }

    private void LaunchSingles(int miniGameIndex)
    {
        // GetFromSeeds() matches on MiniGameSO.ID, not the bank list index — pass the ID.
        int id = miniGameIndex;
        if (miniGameBank != null && miniGameBank.GameBank != null
            && miniGameIndex >= 0 && miniGameIndex < miniGameBank.GameBank.Count
            && miniGameBank.GameBank[miniGameIndex] != null)
        {
            id = miniGameBank.GameBank[miniGameIndex].ID;
        }
        SetLaunchData(GAME_MODE.SINGLES, id);
        StartPlayMode();
    }

    private void SetLaunchData(GAME_MODE mode, int singlesIndex)
    {
        PlayerPrefs.SetInt(EditorLaunchConfig.KEY_ACTIVE, 1);
        PlayerPrefs.SetInt(EditorLaunchConfig.KEY_MODE, (int)mode);
        PlayerPrefs.SetInt(EditorLaunchConfig.KEY_SINGLES_INDEX, singlesIndex);
        PlayerPrefs.Save();
    }

    private void StartPlayMode()
    {
        // Check if Game scene exists
        var gameSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(GAME_SCENE_PATH);
        if (gameSceneAsset == null)
        {
            EditorUtility.DisplayDialog("Error", $"Game scene not found at:\n{GAME_SCENE_PATH}", "OK");
            ClearLaunchData();
            return;
        }

        // Set the Game scene as the play mode start scene
        EditorSceneManager.playModeStartScene = gameSceneAsset;

        // Enter play mode
        EditorApplication.isPlaying = true;
    }

    private void ClearLaunchData()
    {
        PlayerPrefs.DeleteKey(EditorLaunchConfig.KEY_ACTIVE);
        PlayerPrefs.DeleteKey(EditorLaunchConfig.KEY_MODE);
        PlayerPrefs.DeleteKey(EditorLaunchConfig.KEY_SINGLES_INDEX);
        PlayerPrefs.Save();
    }
}

/// <summary>
/// Shared keys for editor launch configuration via PlayerPrefs.
/// Used by both the editor window and the runtime bootstrap.
/// </summary>
public static class EditorLaunchConfig
{
    public const string KEY_ACTIVE = "EditorLaunch_Active";
    public const string KEY_MODE = "EditorLaunch_Mode";
    public const string KEY_SINGLES_INDEX = "EditorLaunch_SinglesIndex";
}
