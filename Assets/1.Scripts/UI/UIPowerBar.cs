using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using static Utils;
public class UIPowerBar : MonoBehaviour
{
    // public GameObject prefab_PowerBarSection;
    public RectTransform layoutHandle;
    // public List<Image> inst_PowerBarSections;
    public Slider progressBar;
    public float Current
    {
        get { return m_TargetValue; }
        set { m_TargetValue = value; }
    }
    float m_TargetValue;
    bool m_IsTrackingProgress;

    float elapsedLerp = 0f;
    public float LerpTime = 0.1f;
    MiniGameLoop m_MGLoop;

    UILoopPresentationAnim Source;
    public void Setup(MiniGameLoop iMGLoop, UILoopPresentationAnim iSource)
    {
        m_MGLoop = iMGLoop;
        progressBar.maxValue = iMGLoop.count;
        progressBar.value = 0f;
        m_TargetValue = 0f;
        Source = iSource;
        TrackProgress();
    }

    void OnDisable()
    {
        StopProgress();
    }

    void TrackProgress()
    {
        if (Source == null)
            return;
        m_IsTrackingProgress = true;
        ProgressTracker();
    }
    public void StopProgress()
    {
        m_IsTrackingProgress = false;
    }

    void Update()
    {
        if (elapsedLerp < LerpTime) { elapsedLerp += Time.deltaTime; }
    }
    
    async UniTaskVoid ProgressTracker()
    {
        while (m_IsTrackingProgress)
        {
            m_TargetValue = Source.DisplayedSuccessLights;
            if (progressBar.value != m_TargetValue)
            {
                progressBar.value = Lerp(progressBar.value, m_TargetValue, elapsedLerp / LerpTime);
            } else
            {
                elapsedLerp = 0f;
            }
            await UniTask.Yield();
        }
    }
}
