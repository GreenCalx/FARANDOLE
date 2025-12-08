using UnityEngine;
using UnityEngine.UI;

using TMPro;
public class UIPlayerProfileCard : MonoBehaviour
{
    public bool Invalidate = false;
    public TextMeshProUGUI m_UserNameText;
    public RectTransform m_HandlePremiumAcc;
    public RectTransform m_HandleNotPremiumAcc;
    public UIButton m_ProfileCardButton;


    void OnEnable()
    {
        Invalidate = true;
    }
    void Update()
    {
        if (Invalidate)
        {
            Refresh();
            Invalidate = false;
        }
    }

    public void Refresh()
    {
        // Set premium features
        bool isPremium = SessionData.IsPremium;

        m_HandlePremiumAcc.gameObject.SetActive(isPremium);
        m_HandleNotPremiumAcc.gameObject.SetActive(!isPremium);

        //m_ProfileCardButton.gameObject.SetActive(true);
        m_ProfileCardButton?.onClick.RemoveAllListeners();
        if (!isPremium)
            m_ProfileCardButton?.onClick.AddListener(() => OpenStore());
        else
            m_ProfileCardButton?.onClick.AddListener(() => OpenProfileEdit());

        // update
        m_UserNameText.text = SessionData.UserName;

        Debug.Log("Profile refreshed");
    }

    public void OpenStore()
    {
        Debug.Log("OpenStore");
        IAPManager.Get.OpenStore();
    }
    
    public void OpenProfileEdit()
    {
        Debug.Log("Profile edit");
    }
}
