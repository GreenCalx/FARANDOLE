using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "Scriptable Objects/GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Player Tweaks")]
    public float PlayerHP = 10f;

    [Header("GameLoop Tweaks")]
    public int loopSize = 5;
    public int PostMiniGameLatchInMs = 500;
    public int PreMiniGameLatchInMs = 500;
    public float MiniGameTime = 5f;
    public int MaxMiniGameDifficulty = 3;
    public int miniGameLevelUpThreshold = 2;
    public float MaxTimeScale = 3f;
    public AnimationCurve timeScaleOverLoopLevel;

    [Header("UI Tweaks")]
    public float GameUIScreenProportion = 0.05f;
    public float GameFieldScreenProportion = 0.8f;

}
