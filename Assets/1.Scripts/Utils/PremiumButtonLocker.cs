using UnityEngine;
using UnityEngine.UI;

public class PremiumButtonLocker : MonoBehaviour
{
    public Transform m_HandleIsLockedFX;
    public Button m_HandleLockedButton;
    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (IAPManager.Get.CanAccessPremiumContent())
        {
            m_HandleIsLockedFX.gameObject.SetActive(false);
            m_HandleLockedButton.enabled = true;
            m_HandleLockedButton.interactable = true;
            
            return;
        }
        m_HandleIsLockedFX.gameObject.SetActive(true);
        m_HandleLockedButton.enabled = false;
        m_HandleLockedButton.interactable = false;
    }
}
