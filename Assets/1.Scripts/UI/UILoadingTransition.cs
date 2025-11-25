using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(CanvasGroup))]
public class UILoadingTransition : MonoBehaviour
{
    public Image blackscreen;
    public Image animatedImage;
    [Header("Internals")]
    public CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public async UniTask Transition(bool iToVisible, float iDuration)
    {
        float startTime = Time.time;
        float frac = 0f;
        while (frac < 1f)
        {
            frac = Mathf.Clamp01((Time.time - startTime) / iDuration);    
            canvasGroup.alpha = iToVisible ? frac : 1f - frac;
            await UniTask.Yield();
        }
    }

    public async UniTask FadeIn(float iDuration)
    {
        await Transition(true, iDuration);
    }

    public async UniTask FadeOut(float iDuration)
    {
        await Transition(false, iDuration);
    }

}
