using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class UIMiniGamePresentationImage : ManagedAnimation, ITapTracker
{
    [Header("UIMiniGamePresentationImage")]
    public bool infoBubbleEnabled = true;
    public UICustomSquircle MiniGameThumbnail;
    public UICustomSquircle light;
    public UICustomSquircle BG;
    public UICustomSquircle MASK;
    public Sprite defaultSprite;
    [Header("InfoBubble")]
    public UIButton infoBubbleBtn;
    public RectTransform h_InfoBubbleAnchor;
    public TextMeshProUGUI h_InfoBubbleText;
    public RectTransform h_InfoBubbleTags;
    public GameObject prefab_InfoBubbleTag;
    List<TextMeshProUGUI> inst_infoBubbleTags;
    [Header("RankMedal")]
    public RectTransform h_RankMedalAnchor;
    public Image h_RankMedalImage;
    public List<TextMeshProUGUI> h_rankTexts;
    [Header("Internals")]
    public MiniGameSuccessState MGSuccessState;
    public bool stopPropagation => false;
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

        inst_infoBubbleTags = new List<TextMeshProUGUI>(iMGDesc.tags.Count);
        foreach (EMiniGameTags tag in iMGDesc.tags)
        {
            TextMeshProUGUI newTagText = GOBuilder.Create(prefab_InfoBubbleTag)
                                        .WithParent(h_InfoBubbleTags)
                                        .BuildAs<TextMeshProUGUI>();
            newTagText.text = "[" + tag.ToString() + "]";
            inst_infoBubbleTags.Add(newTagText);
        }
    }

    public void DisableButton()
    { 
        infoBubbleBtn.interactable = false;
        infoBubbleBtn.enabled = false;
    }

    public void HideInfoBubble()
    {
        bubbleShown = false;
        h_InfoBubbleAnchor.gameObject.SetActive(false);
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

    public void ShowRank(LoopRank iRank)
    {
        h_RankMedalAnchor.gameObject.SetActive(true);
        h_RankMedalImage.sprite = GameData.GetSettings.RankSettings.GetImageFromRank(iRank);
        if (h_rankTexts!=null)
        {
            foreach( TextMeshProUGUI tmp in h_rankTexts)
            {
                tmp.text = iRank.ToString();
            }
        }
    }
    public void HideRank()
    {
      h_RankMedalAnchor.gameObject.SetActive(false);
    }

    public bool OnTap(Vector2 iPos)
    {
        if (bubbleShown)
        {
            HideInfoBubble();
            return true;
        }
        return false;
    }
    public int GetDisplayPriority(){ return 0; }

}
