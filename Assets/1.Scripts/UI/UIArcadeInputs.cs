using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UIArcadeInputs : MonoBehaviour
{
    public Color32 highlightTextColor;
    public UIArcadeButton button1;
    public TextMeshProUGUI text1;
    Color32 text1_normalColor;

    void OnEnable()
    {
        text1_normalColor = text1.color;
    }

    void OnDisable()
    {
        text1.color = text1_normalColor;
    }

    public void Btn1Unbind()
    {
        button1.m_PressCB.RemoveAllListeners();
        button1.m_OnReleaseCB.RemoveAllListeners();
    }
    public void OnBtn1Press(UnityAction iCB)
    {
        button1.m_PressCB.AddListener(iCB);
        button1.m_PressCB.AddListener(() => { text1.color = highlightTextColor; text1.UpdateVertexData(); });
    }
    public void OnBtn1Release(UnityAction iCB)
    {
        button1.m_OnReleaseCB.AddListener(iCB);
        button1.m_OnReleaseCB.AddListener(() => { text1.color = text1_normalColor; text1.UpdateVertexData(); });
    }
}
