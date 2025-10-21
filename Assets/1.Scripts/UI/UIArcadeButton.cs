using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
public class UIArcadeButton : Button, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
    public Image m_Image;
    public UnityEvent m_PressCB;
    public UnityEvent m_OnReleaseCB;
    bool wasPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (wasPressed)
            return;

        DoStateTransition(SelectionState.Pressed, true);
        m_PressCB.Invoke();
        wasPressed = true;
    }

    // IPointerUpHandler
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!wasPressed)
            return;
        m_OnReleaseCB?.Invoke();
        DoStateTransition(SelectionState.Normal, true);
        wasPressed = false;
    }

    // IPointerExitHandler
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!wasPressed)
            return;
        m_OnReleaseCB?.Invoke();
        DoStateTransition(SelectionState.Normal, true);
        wasPressed = false;
    }
}
