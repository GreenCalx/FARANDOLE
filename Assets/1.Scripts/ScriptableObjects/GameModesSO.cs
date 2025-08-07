using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_MODE
{
    RANDOM_SEED,
    DAILY_SEED,
    SPRINT
};

[CreateAssetMenu(fileName = "GameModesSO", menuName = "Scriptable Objects/GameModesSO")]
public class GameModesSO : ScriptableObject
{
    [Serializable]
    public class GameModeInfo
    {
        public GAME_MODE gameMode;
        public GameSettingsSO settings;
    }
    public GameSettingsSO defaultSettings;
    public List<GameModeInfo> modCollection;

    public GameSettingsSO GetSettings(GAME_MODE iMode)
    {
        foreach (GameModeInfo gmi in modCollection)
        {
            if (gmi.gameMode == iMode)
            {
                return gmi.settings;
            }
        }
        return defaultSettings;
    }
}
