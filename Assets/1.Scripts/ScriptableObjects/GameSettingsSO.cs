using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "Scriptable Objects/GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
    [Header("GameLoop Tweaks")]
    public int MaxMiniGameDifficulty = 3;
    public int miniGameLevelUpThreshold = 2;
    public float MaxTimeScale = 3f;
    public AnimationCurve timeScaleOverLoopLevel;

    [Header("UI Tweaks")]
    public float GameUIScreenProportion = 0.05f;
    public float GameFieldScreenProportion = 0.8f;

}
