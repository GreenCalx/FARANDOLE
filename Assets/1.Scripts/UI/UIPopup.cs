using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIPopup : MonoBehaviour
{
    public UnityEvent OnPopUpValidation = new UnityEvent();
    public List<UIButton> ValidatingButtons;

    public void Setup()
    {
        foreach(UIButton b in ValidatingButtons)
            b.clickCallback.AddListener( () => OnPopUpValidation?.Invoke() );
    }

    void OnDisable()
    {
        GameObject.Destroy(gameObject);
    }
}
