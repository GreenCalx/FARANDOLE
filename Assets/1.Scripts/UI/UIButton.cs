using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : Button, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
    public Animator btn_animator;
    public UnityEvent clickCallback;
    bool clickCancelled = false;

    void Start()
    {
        btn_animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        btn_animator.SetTrigger(animationTriggers.pressedTrigger);
        DoStateTransition(SelectionState.Pressed, true);
        clickCancelled = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!clickCancelled)
            clickCallback.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        btn_animator.SetTrigger(animationTriggers.normalTrigger);
        DoStateTransition(SelectionState.Normal, true);
        clickCancelled = true;
    }
}
