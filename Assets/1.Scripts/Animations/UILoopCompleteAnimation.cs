using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UILoopCompleteAnimation : MonoBehaviour
{
    public const string LoopPassedTrigger = "LoopPassed";
    public const string LoopPassedAnimStateName = "OnLoopPassed";
    public List<Image> lightImages;
    public Animator animator;
    [Header("LoopPassed Text")]
    public Color passedTextColor;
    public Color failedTextColor;
    public TextMeshProUGUI loopPassedTxt;
    public int RolloverCharacterSpread = 10;
    public float FadeSpeed = 20.0f;
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
    public void Animate(Color[] iLightColors)
    {
        for (int i = 0; i < iLightColors.Length; i++)
        {
            if (i >= lightImages.Count)
                break;
            lightImages[i].color = iLightColors[i];
        }
        animator.SetTrigger(LoopPassedTrigger);

        StartCoroutine(AnimateTextCo());
    }

    IEnumerator AnimateTextCo()
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

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(LoopPassedAnimStateName));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        // fade in text
        TMP_TextInfo textInfo = loopPassedTxt.textInfo;
        Color32[] newVertexColors;
        int currentCharacter = 0;
        int startingCharacterRange  = currentCharacter;
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
                        loopPassedTxt.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                        //yield return new WaitForSeconds(1.0f);
                        loopPassedTxt.ForceMeshUpdate();
                        //yield return new WaitForSeconds(1.0f);
                        currentCharacter = 0;
                        startingCharacterRange = 0;

                        endReached = true;
                    }
                }
            }
            loopPassedTxt.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            if (currentCharacter + 1 < characterCount)
                currentCharacter++;
            yield return new WaitForSeconds(0.25f - FadeSpeed * 0.01f);
        }

    }

}
