using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class UIMiniGamePresentationImage : ManagedAnimation
{
    [Header("UIMiniGamePresentationImage")]
    public UICustomSquircle MiniGameThumbnail;
    public Image light;
    public UICustomSquircle BG;
    [Header("Internals")]
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
        //BG.color = iColor;
    }

    public void ShowLight(bool iShow)
    {
        animator.SetBool(showLightParam, iShow);
    }

    public override async UniTask DefaultHide(CancellationToken iCT)
    {
        animator.SetBool(DefaultShowAnimParm, false);
        await UniTask.WhenAny(
            WaitAnimState(DefaultHideStateName, 1f, iCT),
            WaitAnimState(hideLightStateName, 1f, iCT)
        );
        
        IsShown = false;
    }

}
