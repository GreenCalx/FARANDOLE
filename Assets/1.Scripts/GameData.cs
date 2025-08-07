using UnityEngine;

public class GameData : MonoBehaviour
{
    public GAME_MODE currentGameMode;
    public GameModesSO gameModes;
    public GameSettingsSO gameSettings;
    public MiniGameBankSO gameBank;
    private static GameData instance = null;
    public static GameData Get => instance;
    public static GameSettingsSO GetSettings => instance.gameSettings;
    public static MiniGameBankSO GetMGBank => instance.gameBank;

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
        }
    }

    public void PickGameMode(GAME_MODE iMode)
    {
        currentGameMode = iMode;
        gameSettings = gameModes.GetSettings(iMode);
    }
}
