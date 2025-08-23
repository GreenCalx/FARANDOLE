using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class UILoopInfo : MonoBehaviour
{
    public RectTransform handle_UILoopInfo;
    public GameObject prefab_loopSingleLight;
    public float radius = 10f;
    public TextMeshProUGUI loopDifficultyTxt;
    public Color passedLightColor;
    public Color pendingLightColor;
    public Color failedLightColor;
    UISingleLoopLight[] loopLights;

    void Start()
    {

    }

    public void Init()
    {
        int loopSize = GameData.GetSettings.loopSize;
        loopLights = new UISingleLoopLight[loopSize];

        // Build lights
        // > place ligthts in circle
        float angle_step = Mathf.PI * 2f / loopSize;
        for (int i = 0; i < loopSize; i++)
        {
            float angle = i * angle_step;
            //angle += 72f;
            Vector3 localPos = new Vector3(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle),
                0f);
            loopLights[i] = GOBuilder.Create(prefab_loopSingleLight)
                            .WithParent(handle_UILoopInfo)
                            .WithLocalPosition(localPos)
                            .BuildAs<UISingleLoopLight>();
            //loopLights[i].TurnOff();
        }
        handle_UILoopInfo.transform.RotateAround(handle_UILoopInfo.transform.position, Vector3.forward, 90f);
    }

    public void UpdateLoopLevelText(string iText)
    {
        if (loopDifficultyTxt == null)
            return;
        loopDifficultyTxt.text = iText;
    }

    public void TurnOnLight(int iIndex, MiniGameSuccessState iState)
    {
        switch (iState)
        {
            case MiniGameSuccessState.PENDING:
                loopLights[iIndex].TurnOn(pendingLightColor);
                break;
            case MiniGameSuccessState.PASSED:
                loopLights[iIndex].TurnOn(passedLightColor);
                break;
            case MiniGameSuccessState.FAILED:
                loopLights[iIndex].TurnOn(failedLightColor);
                break;
            default:
                loopLights[iIndex].TurnOff();
                break;
        }
    }
    public void TurnOffLights()
    {
        for (int i = 0; i < loopLights.Length; i++)
        {
            loopLights[i].TurnOff();
        }
        
    }
}
