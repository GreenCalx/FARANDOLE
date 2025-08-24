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
        Debug.Log("The mouse click si down");
        btn_animator.SetTrigger(animationTriggers.pressedTrigger);
        DoStateTransition(SelectionState.Pressed, true);
        clickCancelled = false;
    }

    //Do this when the mouse click on this selectable UI object is released.
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("The mouse click was released");
        if (!clickCancelled)
            clickCallback.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("The cursor exited the selectable UI element.");

        btn_animator.SetTrigger(animationTriggers.normalTrigger);
        DoStateTransition(SelectionState.Normal, true);
        clickCancelled = true;
    }
}
