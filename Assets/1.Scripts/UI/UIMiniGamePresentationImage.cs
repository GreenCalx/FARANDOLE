using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
public class UIMiniGamePresentationImage : ManagedAnimation
{
    [Header("UIMiniGamePresentationImage")]
    public bool infoBubbleEnabled = true;
    public UICustomSquircle MiniGameThumbnail;
    public UICustomSquircle light;
    public UICustomSquircle BG;
    public UIButton infoBubbleBtn;
    public RectTransform h_InfoBubbleAnchor;
    public TextMeshProUGUI h_InfoBubbleText;
    public Sprite defaultSprite;
    [Header("Internals")]
    public MiniGameSuccessState MGSuccessState;
    public bool Successed => MGSuccessState == MiniGameSuccessState.PASSED;
    public bool LightShown = false;
    public MiniGameSO selfDesc;
    readonly string showLightStateName = "MiniGameImageShowLight";
    readonly string hideLightStateName = "MiniGameImageHideLight";
    readonly string showLightParam = "showlight";
    bool bubbleShown = false;
    public void SetFromMiniGameDesc(MiniGameSO iMGDesc)
    {
        selfDesc = iMGDesc;
        if (iMGDesc.thumbNailImg != null)
            MiniGameThumbnail.sprite = iMGDesc.thumbNailImg;
        else
            MiniGameThumbnail.sprite = defaultSprite;

        infoBubbleBtn.onClick.AddListener(() => { bubbleShown = !bubbleShown; h_InfoBubbleAnchor.gameObject.SetActive(bubbleShown); });
        h_InfoBubbleText.text = selfDesc.goal;
    }

    public void DisableButton()
    { 
        infoBubbleBtn.interactable = false;
        infoBubbleBtn.enabled = false;
    }

    public void UpdateLightColor()
    {
        Color c = new Color(1f, 1f, 1f, 0f);
        switch (MGSuccessState)
        {
            case MiniGameSuccessState.PASSED:
                c = GameData.GetUITheme.thumbnailSuccessLightColor;
                break;
            case MiniGameSuccessState.FAILED:
                c = GameData.GetUITheme.thumbnailFailLightColor;
                break;
            default:
                break;
        }
        c.a = 0f;
        light.color = c;
    }

    public void UpdateMGState(MiniGameSuccessState iMGState)
    {
        MGSuccessState = iMGState;
    }

    public void ShowLight(bool iShow)
    {
        m_Animator.SetBool(showLightParam, iShow);
    }

    public override async UniTask DefaultHide(CancellationToken iCT)
    {
        h_InfoBubbleAnchor.gameObject.SetActive(false);

        m_Animator.SetBool(DefaultShowAnimParm, false);
        m_Animator.SetBool(showLightParam, false);

        await UniTask.WhenAny(
            WaitAnimState(DefaultHideStateName, 1f, iCT),
            WaitAnimState(hideLightStateName, 1f, iCT)
        );
        IsShown = false;
    }


}
