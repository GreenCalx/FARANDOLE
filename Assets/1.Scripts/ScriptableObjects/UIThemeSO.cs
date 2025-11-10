using UnityEngine;

[CreateAssetMenu(fileName = "UIThemeSO", menuName = "Scriptable Objects/UIThemeSO")]
public class UIThemeSO : ScriptableObject
{
    [Header("Menu Button")]
    public Color normalBtnColor;
    public Color pressedBtnColor;
    [Header("Menu Main Button")]
    public Color normalMainBtnColor;
    public Color pressedMainBtnColor;

    [Header("Action Button")]
    public Color normalActionBtnColor;
    public Color pressedActionBtnColor;
    [Header("Thumbnail Button")]
    public Color normalThumbnailColor;
    public Color pressedThumbnailColor;
    public Color thumbnailSuccessLightColor;
    public Color thumbnailFailLightColor;
    [Header("Clock")]
    public Color PositiveTimeColor;
    public Color LastSecondTimeColor;
    public Color NegativeTimeColor;
    public Color FrozenTimeColor;
    [Header("Color")]
    public Color PowerBarFailSection    = Color.red;
    public Color PowerBarPassedSection  = Color.green;
    public Color PowerBarPerfectSection  = Color.yellow;
}
