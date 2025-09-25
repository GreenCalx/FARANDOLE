using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class UIMiniGamePresentationImage : ManagedAnimation
{
    [Header("UIMiniGamePresentationImage")]
    public UICustomSquircle MiniGameThumbnail;
    public Image light;
    public MiniGameSO selfDesc;
    readonly string showLightStateName = "MiniGameImageShowLight";
    readonly string hideLightStateName = "MiniGameImageHideLight";
    readonly string showLightParam = "showlight";
    public void SetFromMiniGameDesc(MiniGameSO iMGDesc)
    {
        selfDesc = iMGDesc;
        MiniGameThumbnail.sprite = iMGDesc.thumbNailImg;
    }

    public void UpdateLightColor(Color iColor)
    {
        light.color = iColor;
    }

    public void ShowLight(bool iShow)
    {
        animator.SetBool(showLightParam, iShow);
    }


}
