using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMiniGamePresentationLine : MonoBehaviour
{
    public UICustomSquircle MiniGameThumbnail;
    public TextMeshProUGUI MiniGameName;
    public TextMeshProUGUI MiniGameDesc;
    public Animator self_animator;
    public readonly string showAnimParm = "show";
    public bool IsShown =  false;
    public void SetFromMiniGameDesc(MiniGameSO iMGDesc)
    {
        MiniGameThumbnail.sprite = iMGDesc.thumbNailImg;
        MiniGameName.text = iMGDesc.name;
        MiniGameDesc.text = iMGDesc.goal;
    }

    public void Show()
    {
        self_animator.SetBool(showAnimParm, true);
        IsShown = true;
    }

    public void Hide()
    {
        self_animator.SetBool(showAnimParm, false);
        IsShown = false;
    }
}
