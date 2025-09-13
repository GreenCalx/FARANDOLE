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
}
