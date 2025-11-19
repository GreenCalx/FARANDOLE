using UnityEngine;

public class UIPopupPanel : UIPanel
{
    public GameObject prefab_SignInPopup;
    public GameObject prefab_PremiumPopup;

    GameObject m_ActivePopup;
    GameObject m_SignInPopupInst;
    GameObject m_PremiumPopupInst;

    public void SignInPopup()
    {
        if (m_ActivePopup!=null)
        {
            GameObject.Destroy(m_ActivePopup);
        }
        m_ActivePopup  = GOBuilder.Create(prefab_SignInPopup)
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .Build();    
        m_ActivePopup.SetActive(true);
    }

    public void PremiumPopup()
    {
        if (m_ActivePopup!=null)
        {
            GameObject.Destroy(m_ActivePopup);
        }
        m_ActivePopup  = GOBuilder.Create(m_PremiumPopupInst)
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .Build();
        m_ActivePopup.SetActive(true);
    }
}
