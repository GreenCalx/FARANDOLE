using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
public struct UIPanelLink
{
    public UIPanel source;
    public UIPanel destination;
    public UIPanelLink(UIPanel iSource, UIPanel iDestination)
    {
        source = iSource;
        destination = iDestination;
    }
}

public class UIPanel : MonoBehaviour
{
    public UIPanelAnimation m_PanelAnimation;
    public CanvasGroup m_CanvasGroup;
    public List<UIPanel> m_Destinations;
    public List<UIPanelLink> GetLinks()
    {
        List<UIPanelLink> links = new List<UIPanelLink>(m_Destinations.Count);
        foreach(UIPanel p in m_Destinations)
        {
            links.Add( new UIPanelLink(this,p) );
        }
        return links;
    }
    public void Disable()
    {
        m_CanvasGroup.alpha = 0f;
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;
    }

    public virtual void OnNavEnter(CancellationToken iCT)
    {
        m_PanelAnimation.DefaultShow(iCT);
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.blocksRaycasts = true;
    }

    public virtual void OnNavExit(CancellationToken iCT)
    {
        m_PanelAnimation.DefaultHide(iCT);
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;
    }
}
