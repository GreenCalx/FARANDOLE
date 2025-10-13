using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UITextFaderAnimation : MonoBehaviour
{
    // int : current character
    // byte : alpha value
    List<Dictionary<int, byte>> bakedAnim;
    public TextMeshProUGUI targetTxt;
    public int RolloverCharacterSpread = 10;
    public int FadeSpeed = 20;
    [Range(0f, 1f)]
    public float textFadeInFrac = 0f;

    bool baked = false;
    bool animate = false;

    public Dictionary<int, byte> GetKeyFrameAtFrac(float iFrac)
    {
        float frac = Mathf.Clamp01(iFrac);
        return bakedAnim[(int)Math.Floor(frac * (bakedAnim.Count - 1))];
    }

    // Text Fader from invisible to fully visible
    public void Bake()
    {
        TMP_TextInfo textInfo = targetTxt.textInfo;
        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool endReached = false;
        int characterCount = textInfo.characterCount;

        bakedAnim = new List<Dictionary<int, byte>>();
        Dictionary<int, byte> keyframe = new Dictionary<int, byte>(characterCount - 1);
        for (int i = 0; i <= characterCount; i++)
        {
            keyframe.Add(i, 0);
        }
        bakedAnim.Add(new Dictionary<int, byte>(keyframe));

        byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);

        byte alpha = 0;
        while (!endReached)
        {
            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                alpha = (byte)Mathf.Clamp(bakedAnim[bakedAnim.Count - 1][i] + fadeSteps, 0, 255);
                keyframe[i] = alpha;
                bakedAnim.Add(new Dictionary<int, byte>(keyframe));

                if (alpha == 255)
                {
                    startingCharacterRange++;
                    if (startingCharacterRange == characterCount)
                    {
                        currentCharacter = 0;
                        startingCharacterRange = 0;

                        endReached = true;
                        break;
                    }
                }
            }
            bakedAnim.Add(new Dictionary<int, byte>(keyframe));
            if (currentCharacter + 1 < characterCount)
                currentCharacter++;
        }

        baked = true;
    }

    public void Animate(CancellationToken iAnimStopper, CancellationToken iCT)
    {
        if (!baked)
        {
            Debug.LogWarning("UITextFaderAnimation not baked.");
            animate = false;
            return;
        }
        CancellationTokenSource cts = new CancellationTokenSource();
        AnimateTextTask(iAnimStopper, iCT);
    }

    public async UniTask AnimateTextTask(CancellationToken iAnimStopper, CancellationToken iCT)
    {
        targetTxt.color = new Color
        (
            targetTxt.color.r,
            targetTxt.color.g,
            targetTxt.color.b,
            0
        );

        TMP_TextInfo textInfo = targetTxt.textInfo;
        Color32[] newVertexColors;
        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool endReached = false;
        int characterCount = textInfo.characterCount;

        byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);
        while (!iAnimStopper.IsCancellationRequested)
        {
            if (iCT.IsCancellationRequested)
                return;
            Dictionary<int, byte> keyframe = GetKeyFrameAtFrac(textFadeInFrac);
            for (int i = 0; i < characterCount + 1; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                byte alpha = keyframe[i];
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
            if (targetTxt == null)
                return;
            targetTxt.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            if (currentCharacter + 1 < characterCount)
                currentCharacter++;
            await UniTask.Delay(FadeSpeed);
        }
    }

}
