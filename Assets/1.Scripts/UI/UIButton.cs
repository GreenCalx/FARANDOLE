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
    Image self_img;
    public UnityEvent OnPressed;
    public UICustomSquircle image;
    UITag tag;
    bool pressed = false;

    void Start()
    {
        self_img = GetComponent<Image>();
        btn_animator = GetComponent<Animator>();
        image = GetComponent<UICustomSquircle>();
        tag = GetComponent<UITag>();
        pressed = false;
        UpdateImage();
    }

    // void Update()
    // {
    //     UpdateImage();
    // }

    // IMaterialModifier
    public void UpdateImage()
    {
        if (image==null)
            image = GetComponent<UICustomSquircle>();
            
        switch (tag.tag)
        {
            case EUITag.MainBtn:
                image.color = pressed ? GameData.GetUITheme.pressedMainBtnColor : GameData.GetUITheme.normalMainBtnColor;
                break;
            case EUITag.ActionBtn:
                image.color = pressed ? GameData.GetUITheme.pressedActionBtnColor : GameData.GetUITheme.normalActionBtnColor;
                break;
            default:
                image.color = pressed ? GameData.GetUITheme.pressedBtnColor : GameData.GetUITheme.normalBtnColor;
                break;
        }
        
        image.SetVerticesDirty();
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
