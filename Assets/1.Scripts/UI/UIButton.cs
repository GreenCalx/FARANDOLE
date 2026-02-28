using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UITag))]
[Serializable]
public class UIButton : Button, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
    public Animator btn_animator;
    public UnityEvent clickCallback;

    bool clickCancelled = false;
    Image ui_img;
    public UnityEvent OnPressed;
    UICustomSquircle squircle_img;
    UITag tag;
    bool pressed = false;

    void Awake()
    {
       // clickCallback = new UnityEvent();
    }
    void Start()
    {
        ui_img = GetComponent<Image>();
        btn_animator = GetComponent<Animator>();
        squircle_img = GetComponent<UICustomSquircle>();
        tag = GetComponent<UITag>();
        pressed = false;
        //UpdateImage();
    }

    void OnEnable()
    {
        UpdateImage();
    }

    // IMaterialModifier
    public void UpdateImage()
    {
        if (squircle_img != null)
            UpdateSquircleImage();
        if (ui_img != null)
            UpdateUIImage();
    }

    void UpdateUIImage()
    {
        if (GameData.Get == null)
            return; // weird but happens..
        switch (tag.tag)
            {
                case EUITag.MainBtn:
                    ui_img.color = pressed ? GameData.GetUITheme.pressedMainBtnColor : GameData.GetUITheme.normalMainBtnColor;
                    break;
                case EUITag.ActionBtn:
                    ui_img.color = pressed ? GameData.GetUITheme.pressedActionBtnColor : GameData.GetUITheme.normalActionBtnColor;
                    break;
                case EUITag.ThumbnailBtn:
                    ui_img.color = pressed ? GameData.GetUITheme.pressedThumbnailColor : GameData.GetUITheme.normalThumbnailColor;
                    break;
                default:
                    ui_img.color = pressed ? GameData.GetUITheme.pressedBtnColor : GameData.GetUITheme.normalBtnColor;
                    break;
            }
        ui_img.SetVerticesDirty();
    }

    void UpdateSquircleImage()
    {
        if (GameData.Get == null)
            return; // weird but happens..

        if (!interactable)
        {
            squircle_img.color = GameData.GetUITheme.disabledBtnColor;
            squircle_img.SetVerticesDirty();
            return;
        }
        switch (tag.tag)
        {
            case EUITag.MainBtn:
                squircle_img.color = pressed ? GameData.GetUITheme.pressedMainBtnColor : GameData.GetUITheme.normalMainBtnColor;
                break;
            case EUITag.ActionBtn:
                squircle_img.color = pressed ? GameData.GetUITheme.pressedActionBtnColor : GameData.GetUITheme.normalActionBtnColor;
                break;
            case EUITag.ThumbnailBtn:
                squircle_img.color = pressed ?  GameData.GetUITheme.pressedThumbnailColor : GameData.GetUITheme.normalThumbnailColor ;
                break;
            default:
                squircle_img.color = pressed ? GameData.GetUITheme.pressedBtnColor : GameData.GetUITheme.normalBtnColor;
                break;
        }
        squircle_img.SetVerticesDirty();
    }

    // IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;

        btn_animator.SetTrigger(animationTriggers.pressedTrigger);
        DoStateTransition(SelectionState.Pressed, true);
        clickCancelled = false;
        pressed = true;
        UpdateImage();
    }

    // IPointerUpHandler
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!clickCancelled)
            clickCallback.Invoke();
        pressed = false;
        UpdateImage();
    }

    // IPointerExitHandler
    public void OnPointerExit(PointerEventData eventData)
    {
        if (btn_animator != null && !string.IsNullOrEmpty(animationTriggers.normalTrigger))
            btn_animator.ResetTrigger(animationTriggers.pressedTrigger);
        DoStateTransition(SelectionState.Normal, true);
        clickCancelled = true;
        pressed = false;
        UpdateImage();
    }

}
