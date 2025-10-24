using UnityEngine;

[CreateAssetMenu(fileName = "GlobalSettingsSO", menuName = "Scriptable Objects/GlobalSettingsSO")]
public class GlobalSettingsSO : ScriptableObject
{
    [Header("Graphics")]
    public int targetFrameRate = 60;
    public int fallbackTargetFrameRate = 30;
    [Header("Sound")]
    public bool MuteMusic = false;
}
