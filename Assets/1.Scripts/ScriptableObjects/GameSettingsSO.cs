using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "Scriptable Objects/GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Player Tweaks")]
    public float PlayerHP = 10f;

    [Header("GameLoop Tweaks")]
    public int loopSize = 5;

    [Tooltip("Inclusive")]
    public float percentOfLoopSizeToPass = 0.5f;
    public int ComboRequirementForSuper = 2;
    public int ComboRequirementForMaster = 5;
    public int loopPassThreshold
    {
        get { return (int)Mathf.Floor((float)loopSize / 2f); }
    }
    public float MiniGameTime = 5f;
    public float MaxTimeScale = 3f;
    public AnimationCurve timeScaleOverLoopLevel;


    [Header("UI Tweaks")]
    public float titleScreenFadeoutTime = 1f;
    public float GameUIScreenProportion = 0.05f;
    public float GameFieldScreenProportion = 0.8f;
    public Color LoopPefectColor = Color.yellow;
    public Color LoopPassedColor = Color.green;
    public Color LoopFailedColor = Color.red;
    public LoopRankSO RankSettings;

    [Header("GameFlow Tweaks")]
    public int PostMiniGameLatchInMs = 500;
    public int PreMiniGameLatchInMs = 500;
    public float PlayGroundColorLerpTimeSec = 1f;
    public float BGMRankLerpTimeSec = 1f;
    public float GameClockScaleAnimDuration = 0.25f;
    public float ShowLightTotalTimeInMs = 2000f;
    public int MaxLightUpMiniGameThumbnailTime = 250;

}
