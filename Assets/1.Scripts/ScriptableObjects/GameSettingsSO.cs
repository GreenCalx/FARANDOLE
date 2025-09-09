using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "Scriptable Objects/GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Player Tweaks")]
    public float PlayerHP = 10f;

    [Header("GameLoop Tweaks")]
    public int loopSize = 5;
    public float MiniGameTime = 5f;
    public int miniGameLevelUpThreshold = 2;
    public float MaxTimeScale = 3f;
    public AnimationCurve timeScaleOverLoopLevel;

    [Header("UI Tweaks")]
    public float titleScreenFadeoutTime = 1f;
    public float GameUIScreenProportion = 0.05f;
    public float GameFieldScreenProportion = 0.8f;

    [Header("GameFlow Tweaks")]
    public int PostMiniGameLatchInMs = 500;
    public int PreMiniGameLatchInMs = 500;
    public int LoopCompleteAfterAnimDisplayTimeMs = 1000;
    public int LoopCompleteShowDepthAnimDisplayTimeMs = 500;
    public float PlayGroundColorLerpTimeSec = 1f;

}
