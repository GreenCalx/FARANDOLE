using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using static Utils;

public class UICompletionBar : MonoBehaviour
{
    public Image FillImage;
    public RectTransform layoutHandle;
    public Slider progressBar;

    public void Setup(float initValue = 0f)
    {
        progressBar.value = initValue;
    }
}
