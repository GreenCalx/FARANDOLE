using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using static Utils;
public class GameData : MonoBehaviour
{
    public List<int> m_MiniGameSeeds;
    public List<int> MiniGameSeeds => m_MiniGameSeeds;
    public bool HasSeeds => m_MiniGameSeeds!=null && m_MiniGameSeeds.Count > 0;
    public GAME_MODE currentGameMode;
    public GameModesSO gameModes;
    public GameSettingsSO gameSettings;
    public MiniGameBankSO gameBank;
    public UIThemeSO UITheme;
    public ArtefactCollectionSO artefactCollection;
    public AccessibilitySettingsSO accessibilitySettingsSO;
    public GlobalSettingsSO globalSettingsSO;
    private static GameData instance = null;
    public static GameData Get => instance;
    public static GameSettingsSO GetSettings => instance.gameSettings;
    public static MiniGameBankSO GetMGBank => instance.gameBank;
    public static UIThemeSO GetUITheme => instance.UITheme;
    public static ArtefactCollectionSO GetArtefactCollection => instance.artefactCollection;
    public static AccessibilitySettingsSO GetAccessibilitySettings => instance.accessibilitySettingsSO;
    public static GlobalSettingsSO GetGlobalSettings => instance.globalSettingsSO;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            #if UNITY_EDITOR
            // Apply editor launch config to existing instance
            instance.ApplyEditorLaunchConfig();
            #endif
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            #if UNITY_EDITOR
            ApplyEditorLaunchConfig();
            #endif

            ApplyGlobalSettings();
            LoadUserData();
        }
    }

    #if UNITY_EDITOR
    private void ApplyEditorLaunchConfig()
    {
        const string KEY_ACTIVE = "EditorLaunch_Active";
        const string KEY_MODE = "EditorLaunch_Mode";
        const string KEY_SINGLES_INDEX = "EditorLaunch_SinglesIndex";

        if (PlayerPrefs.GetInt(KEY_ACTIVE, 0) != 1)
            return;

        GAME_MODE mode = (GAME_MODE)PlayerPrefs.GetInt(KEY_MODE, 0);
        int singlesIndex = PlayerPrefs.GetInt(KEY_SINGLES_INDEX, -1);

        Debug.Log($"[GameData] Editor launch config: {mode}" +
                  (mode == GAME_MODE.SINGLES ? $" (MiniGame: {singlesIndex})" : ""));

        switch (mode)
        {
            case GAME_MODE.RANDOM:
                NewGameSeed();
                PickGameMode(GAME_MODE.RANDOM);
                break;
            case GAME_MODE.DAILY_SEED:
                PickGameMode(GAME_MODE.DAILY_SEED);
                SetToDailySeed();
                break;
            case GAME_MODE.MUTATION:
                NewGameSeed();
                PickGameMode(GAME_MODE.MUTATION);
                break;
            case GAME_MODE.SINGLES:
                PickGameMode(GAME_MODE.SINGLES);
                NewGameSeed();
                if (singlesIndex >= 0)
                    AddGameSeed(singlesIndex);
                break;
        }

        // Clear launch data
        PlayerPrefs.DeleteKey(KEY_ACTIVE);
        PlayerPrefs.DeleteKey(KEY_MODE);
        PlayerPrefs.DeleteKey(KEY_SINGLES_INDEX);
        PlayerPrefs.Save();
    }
    #endif

    void ApplyGlobalSettings()
    {
        Application.targetFrameRate = globalSettingsSO.targetFrameRate;
    }

    void LoadUserData()
    {
        UserData.Init();
        if (!UserData.LoadHighScores())
        {
            Debug.Log("No save file found.");
        }
        else
        {
            Debug.Log("Save file loaded.");
        }
    }

    public void PickGameMode(GAME_MODE iMode)
    {
        currentGameMode = iMode;
        gameSettings = gameModes.GetSettings(iMode);
    }

    public void AddGameSeed(int iSeed)
    {
        if (m_MiniGameSeeds == null)
            m_MiniGameSeeds = new List<int>();
        m_MiniGameSeeds.Add(iSeed);
    }

    public void NewGameSeed()
    {
        if (m_MiniGameSeeds == null)
            m_MiniGameSeeds = new List<int>();
        else
            m_MiniGameSeeds.Clear();
    }
    
    public void SetToDailySeed()
    {
        NewGameSeed();

        // Generate a stable seed across platforms.
        int rawseed = Utils.GetTodaySeed();

         // load RNG to shuffle mini games
        System.Random rng = new System.Random(rawseed);
        List<int> order = Enumerable.Range(0, GetMGBank.Size).ToList();
        for (int i = order.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            int temp = order[i];
            order[i] = order[j];
            order[j] = temp;
        }

        // Fill seeds
        for (int i=0; i < gameSettings.loopSize; i++)
        {
            AddGameSeed(order[i]);
        }
    }
}
 