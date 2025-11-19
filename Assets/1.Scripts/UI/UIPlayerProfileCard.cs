using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
public class UIPlayerProfileCard : MonoBehaviour
{
    public TextMeshProUGUI m_UserNameText;
    public RectTransform m_HandlePremiumAcc;
    public RectTransform m_HandleNotPremiumAcc;
    public UIButton m_ProfileCardButton;
    PlayGamesLocalUser localUser;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // Set premium features
        bool isPremium = SessionData.IsPremium;
        Debug.Log("Refresh premium = " + isPremium);
        m_HandlePremiumAcc.gameObject.SetActive(isPremium);
        m_HandleNotPremiumAcc.gameObject.SetActive(!isPremium);

        //m_ProfileCardButton.gameObject.SetActive(true);
        m_ProfileCardButton?.onClick.RemoveAllListeners();
        if (!isPremium)
            m_ProfileCardButton?.onClick.AddListener(() => OpenStore());
        else
            m_ProfileCardButton?.onClick.AddListener(() => OpenProfileEdit());

        // update profile
        if (PlayGamesPlatform.Instance.IsAuthenticated())
            m_UserNameText.text = PlayGamesPlatform.Instance.GetUserDisplayName();
        else
            m_UserNameText.text = "Guest";
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
