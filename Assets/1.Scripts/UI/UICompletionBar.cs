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
    float m_ChunkSize;
    public void Setup(float minValue, float maxValue, float chunkSize =1f, float initValue = 0f)
    {
        progressBar.minValue = minValue;
        progressBar.maxValue = maxValue;
        progressBar.value = initValue;
        m_ChunkSize = chunkSize;
    }

    public void Increment()
    {
        progressBar.value += m_ChunkSize;
    }

    public void Decrement()
    {
        progressBar.value -= m_ChunkSize;
    }
}
