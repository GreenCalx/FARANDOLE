using UnityEngine;

public class GameData : MonoBehaviour
{
    public GameSettingsSO gameSettings;
    private static GameData instance = null;
    public static GameData Get => instance;
    public static GameSettingsSO GetSettings => instance.gameSettings;

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
}
