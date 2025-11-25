using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;

public class UINavigator : MonoBehaviour
{
    [Header("UINavigator")]
    public bool AutoSubscribersInHierarchy = true;
    public List<UIPanel> m_SubscribedElements;
    public UINavigationPanel m_NavigationPanel;
    public UIPanel m_Home;
    [Header("Internals")]
    public Stack<UIPanel> panels;
    public UIPanel activeCanvas => panels?.Peek();
    public CancellationTokenSource m_CTS;
    public void Setup(UIPanel iHome)
    {
        if (AutoSubscribersInHierarchy)
        {
            m_SubscribedElements = new List<UIPanel>();
            foreach (Transform child in transform)
            {
                UIPanel p = child.GetComponent<UIPanel>()    ;
                if (p==null)
                    continue;
                m_SubscribedElements.Add(p);
            }
        }

        m_Home = iHome;
        foreach(UIPanel p in m_SubscribedElements)
        {
            p.gameObject.SetActive(true);

            p.m_PanelAnimation.AfterHideCB.AddListener(()=>p.DisableCanvas());
            p.m_PanelAnimation.AfterHideCB.AddListener(()=>p.DisableChilds());

            p.m_PanelAnimation.PreShowCB.AddListener(()=>p.EnableChilds());
            p.m_PanelAnimation.AfterShowCB.AddListener(()=>p.EnableCanvas());

            if (p==m_Home)
                p.EnableAll();
            else
                p.DisableAll();
        }

        panels = new Stack<UIPanel>();
        panels.Push(m_Home);

        m_CTS = new CancellationTokenSource();
        
        m_NavigationPanel.Setup(this);
        
        //NavigateTo(m_Home);
        m_Home.OnNavEnter(m_CTS.Token);
    }

    public void NavigateTo(UIPanel iDestination)
    {
        panels.Peek().OnNavExit(m_CTS.Token);
        panels.Push(iDestination);
        panels.Peek().OnNavEnter(m_CTS.Token);

        m_NavigationPanel.Refresh();
    }

    public void OnBack()
    {
        if (panels.Count <= 1)
            return;

        panels.Pop().OnNavExit(m_CTS.Token);
        panels.Peek().OnNavEnter(m_CTS.Token);

        m_NavigationPanel.Refresh();
    }

}
