using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

public class UILoopCompleteAnimation : MonoBehaviour
{
    public const string LoopPassedTrigger = "LoopPassed";
    public const string LoopHideTrigger = "Hide";
    public const string LoopPassedAnimStateName = "OnLoopPassed";
    public const string LoopHideAnimStateName = "OnLoopHide";
    public List<Image> lightImages;
    public Animator animator;
    [Header("LoopPassed Text")]
    public Color passedTextColor;
    public Color failedTextColor;
    public TextMeshProUGUI loopPassedTxt;
    public int RolloverCharacterSpread = 10;
    public int FadeSpeed = 20;
    void Start()
    {
        animator = GetComponent<Animator>();

        loopPassedTxt.color = new Color
        (
            loopPassedTxt.color.r,
            loopPassedTxt.color.g,
            loopPassedTxt.color.b,
            0
        );
        loopPassedTxt.ForceMeshUpdate();
    }
    public async Task Animate(Color[] iLightColors)
    {
        for (int i = 0; i < iLightColors.Length; i++)
        {
            if (i >= lightImages.Count)
                break;
            lightImages[i].color = iLightColors[i];
        }
        animator.SetTrigger(LoopPassedTrigger);

        await WaitMainAnimTask(1f); // full anim
        await AnimateTextTask();

        await Task.Delay(GameData.GetSettings.LoopCompleteAfterAnimDisplayTimeMs);
        Hide();
        await WaitHideAnimTask(1f); // half anim
    }

    public void Hide()
    {
        animator.SetTrigger(LoopHideTrigger);
    }

    async Task WaitHideAnimTask(float iCompletionFrac)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(LoopHideAnimStateName))
        { await Task.Yield(); }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac)
        { await Task.Yield(); }        
    }
    async Task WaitMainAnimTask(float iCompletionFrac)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(LoopPassedAnimStateName))
        { await Task.Yield(); }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac)
        { await Task.Yield(); }
    }
    async Task AnimateTextTask()
    {
        // make transparent
        loopPassedTxt.color = new Color
        (
            loopPassedTxt.color.r,
            loopPassedTxt.color.g,
            loopPassedTxt.color.b,
            0
        );
        loopPassedTxt.ForceMeshUpdate();

        // fade in text
        TMP_TextInfo textInfo = loopPassedTxt.textInfo;
        Color32[] newVertexColors;
        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool endReached = false;
        int characterCount = textInfo.characterCount;

        byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);
        while (!endReached)
        {
            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                byte alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex + 0].a + fadeSteps, 0, 255);
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;
                if (alpha == 255)
                {
                    startingCharacterRange++;
                    if (startingCharacterRange == characterCount)
                    {
                        currentCharacter = 0;
                        startingCharacterRange = 0;

                        endReached = true;
                    }
                }
            }
            loopPassedTxt.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            if (currentCharacter + 1 < characterCount)
                currentCharacter++;
            await Task.Delay(25 - FadeSpeed);
        }
        Debug.Log("Text displayed");
    }

}
