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
        switch (tag.tag)
        {
            case EUITag.MainBtn:
                ui_img.color = pressed ? GameData.GetUITheme.pressedMainBtnColor : GameData.GetUITheme.normalMainBtnColor;
                break;
            case EUITag.ActionBtn:
                ui_img.color = pressed ? GameData.GetUITheme.pressedActionBtnColor : GameData.GetUITheme.normalActionBtnColor;
                break;
            default:
                ui_img.color = pressed ? GameData.GetUITheme.pressedBtnColor : GameData.GetUITheme.normalBtnColor;
                break;
        }
        ui_img.SetVerticesDirty();
    }

    void UpdateSquircleImage()
    {
        switch (tag.tag)
        {
            case EUITag.MainBtn:
                squircle_img.color = pressed ? GameData.GetUITheme.pressedMainBtnColor : GameData.GetUITheme.normalMainBtnColor;
                break;
            case EUITag.ActionBtn:
                squircle_img.color = pressed ? GameData.GetUITheme.pressedActionBtnColor : GameData.GetUITheme.normalActionBtnColor;
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
        btn_animator.SetTrigger(animationTriggers.normalTrigger);
        DoStateTransition(SelectionState.Normal, true);
        clickCancelled = true;
        pressed = false;
        UpdateImage();
    }
}
