using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UIArcadeInputs : MonoBehaviour
{
    [Header("ButtonA")]
    public bool activateButton1 = true;
    public Transform h_Button1;
    public Color32 highlightTextColor;
    public UIArcadeButton button1;
    public TextMeshProUGUI text1;
    Color32 text1_normalColor;
    [Header("Joystick")]
    public bool activateXYController = false;
    public Transform h_xyController;
    public XYController xyController;

    void OnEnable()
    {
        h_Button1.gameObject.SetActive(TryEnableBtn1());
        h_xyController.gameObject.SetActive(TryEnableXY());
    }
    void OnDisable()
    {
        TryDisableBtn1();
        TryDisableXY();
    }

    #region Button1
    bool TryEnableBtn1()
    {
        if (!activateButton1)
        {
            return false;
        }
        text1_normalColor = text1.color;
        return true;
    }
    void TryDisableBtn1()
    {
        if (!activateButton1)
            return;
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
    #endregion

    #region XYController
    bool TryEnableXY()
    {
        if (!activateXYController)
        {
            return false;
        }
        return true;
    }
    void TryDisableXY()
    {
        if (!activateXYController)
            return;
    }

    public void XYBind( UnityAction<Vector2> iCB)
    {
        xyController.PositionChangedCB.AddListener(iCB);
    }
    public void XYUnbind()
    {
        xyController.PositionChangedCB.RemoveAllListeners();
    }
    #endregion
}
