using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILoopCompleteAnimation : MonoBehaviour
{
    public const string LoopPassedTrigger = "LoopPassed";
    public List<Image> lightImages;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
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
    }

}
