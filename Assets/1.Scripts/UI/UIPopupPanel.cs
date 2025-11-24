using UnityEngine;
using UnityEngine.Events;

public class UIPopupPanel : UIPanel
{
    public GameObject prefab_SignInPopup;
    public GameObject prefab_PremiumPopup;
    UIPopup m_ActivePopup;
    public bool PopupActive => m_ActivePopup != null;
    [Header("Internals")]
    public UnityEvent NavigateBackCB = new UnityEvent();

    void Start()
    {
        m_PanelAnimation.AfterHideCB.AddListener(()=>KillPopup());
    }

    void SetupActivePopup()
    {
        if (m_ActivePopup==null)
            return;
        m_ActivePopup.gameObject.SetActive(true);
        m_ActivePopup.OnPopUpValidation.AddListener( () => NavigateBackCB?.Invoke());
    }

    public void SignInPopup()
    {
        KillPopup();
        m_ActivePopup  = GOBuilder.Create(prefab_SignInPopup)
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .BuildAs<UIPopup>();
        
        SetupActivePopup();
    }

    public void PremiumPopup()
    {
        KillPopup();
        m_ActivePopup  = GOBuilder.Create(prefab_PremiumPopup)
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .BuildAs<UIPopup>();
        SetupActivePopup();
    }

    public void KillPopup()
    {
        if (!PopupActive)
            return;
        m_ActivePopup.gameObject.SetActive(false);
        //GameObject.Destroy(m_ActivePopup);
        m_ActivePopup = null;
    }

}
