using UnityEngine;

public class GameData : MonoBehaviour
{
    public GAME_MODE currentGameMode;
    public GameModesSO gameModes;
    public GameSettingsSO gameSettings;
    public MiniGameBankSO gameBank;
    public UIThemeSO UITheme;
    public AccessibilitySettingsSO accessibilitySettingsSO;
    public GlobalSettingsSO globalSettingsSO;
    private static GameData instance = null;
    public static GameData Get => instance;
    public static GameSettingsSO GetSettings => instance.gameSettings;
    public static MiniGameBankSO GetMGBank => instance.gameBank;
    public static UIThemeSO GetUITheme => instance.UITheme;
    public static AccessibilitySettingsSO GetAccessibilitySettings => instance.accessibilitySettingsSO;
    public static GlobalSettingsSO GetGlobalSettings => instance.globalSettingsSO;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            ApplyGlobalSettings();
            LoadUserData();
        }
    }

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
}
 