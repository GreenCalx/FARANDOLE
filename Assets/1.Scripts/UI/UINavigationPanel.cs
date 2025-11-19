using UnityEngine;

public class UINavigationPanel : UIPanel
{
    public UIButton backBtn;
    public UIButton quitBtn;
    UINavigator m_ObservedNavigator;
    public void Setup(UINavigator iNavigator)
    {
        m_ObservedNavigator = iNavigator;
        OnNavEnter(m_ObservedNavigator.m_CTS.Token);
    }

    public void Refresh()
    {
        bool isHomePage = m_ObservedNavigator.panels.Count <= 1;
        backBtn.gameObject.SetActive(!isHomePage);
        quitBtn.gameObject.SetActive(isHomePage);
    }
}
