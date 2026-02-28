using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class UIMiniGamePresentationImage : UISelectableImage, ITapTracker
{
    [Header("UIMiniGamePresentationImage")]
    public bool infoBubbleEnabled = true;
    public UICustomSquircle MiniGameThumbnail;
    public Sprite defaultSprite;
    [Header("Mutations")]
    public UIMutationIndicators h_MutationIndicators;
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
    [Header("Selection")]
    public UIButton selectButton;
    [Header("Internals")]
    public MiniGameSuccessState MGSuccessState;
    public bool stopPropagation => false;
    public bool Successed => MGSuccessState == MiniGameSuccessState.PASSED;
    public MiniGameSO selfDesc;
    bool bubbleShown = false;

    protected override void Awake()
    {
        base.Awake();
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
        }
    }

    private void OnSelectButtonClicked()
    {
        Select();
    }

    public override void Select()
    {
        base.Select();
        UpdateLightColor(GameData.GetUITheme.thumbnailSuccessLightColor);
    }

    public void SetFromMiniGameDesc(MiniGameSO iMGDesc)
    {
        selfDesc = iMGDesc;
        if (iMGDesc.thumbNailImg != null)
            MiniGameThumbnail.sprite = iMGDesc.thumbNailImg;
        else
            MiniGameThumbnail.sprite = defaultSprite;

        infoBubbleBtn.onClick.AddListener(() => { bubbleShown = !bubbleShown; h_InfoBubbleAnchor.gameObject.SetActive(bubbleShown); });
        h_InfoBubbleText.text = selfDesc.goal;

        // family
        inst_infoBubbleTags = new List<TextMeshProUGUI>();
        TextMeshProUGUI newTagText = GOBuilder.Create(prefab_InfoBubbleTag)
                                    .WithParent(h_InfoBubbleTags)
                                    .BuildAs<TextMeshProUGUI>();
        newTagText.text = "[" + iMGDesc.family.ToString() + "]";
        inst_infoBubbleTags.Add(newTagText);

    }

    public void RefreshMutations(MiniGame iMiniGame)
    {
        if (!iMiniGame.HaveMutations)
        {
            h_MutationIndicators.gameObject.SetActive(false);
            return;   
        }
        h_MutationIndicators.gameObject.SetActive(true);
        h_MutationIndicators.Refresh(iMiniGame);
    }

    public void DisableButton()
    {
        infoBubbleBtn.interactable = false;
        infoBubbleBtn.enabled = false;
    }

    public void EnableSelection()
    {
        if (selectButton != null)
        {
            selectButton.interactable = true;
            selectButton.enabled = true;
        }

        // Redirect infoBubbleBtn to selection instead of tooltip
        if (infoBubbleBtn != null)
        {
            infoBubbleBtn.onClick.RemoveAllListeners();
            infoBubbleBtn.onClick.AddListener(OnSelectButtonClicked);
        }
        HideInfoBubble();
    }

    public void DisableSelection()
    {
        if (selectButton != null)
        {
            selectButton.interactable = false;
            selectButton.enabled = false;
        }

        // Restore infoBubbleBtn to tooltip behavior
        if (infoBubbleBtn != null && selfDesc != null)
        {
            infoBubbleBtn.onClick.RemoveAllListeners();
            infoBubbleBtn.onClick.AddListener(() => {
                bubbleShown = !bubbleShown;
                h_InfoBubbleAnchor.gameObject.SetActive(bubbleShown);
            });
        }
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
        UpdateLightColor(c);
    }

    public void UpdateMGState(MiniGameSuccessState iMGState)
    {
        MGSuccessState = iMGState;
    }

    public override async UniTask DefaultHide(CancellationToken iCT)
    {
        h_InfoBubbleAnchor.gameObject.SetActive(false);
        await base.DefaultHide(iCT);
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
