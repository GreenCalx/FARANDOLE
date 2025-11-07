using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.UI.Extensions;
using static DrawUtils;

public class UILoopPresentationAnim : ManagedAnimation
{
    [Header("UILoopPresentationAnim")]
    public GameObject prefab_LoopStartImage;
    public GameObject prefab_MiniGamePresentationImage;
    public RectTransform handle_LoopShow;
    public UILineRenderer handle_LR;
    public UIRankMedalAnim rankMedalAnimation;
    public int timeBetweenShowLinesInMs = 200;
    List<UIMiniGamePresentationImage> uiImages;

    public float radius = 50f;
    public int LR_resolution = 2;
    bool init = false;
    [Header("Internals")]
    UILoopStartImage inst_loopStartImage;

    public void Init(MiniGameLoop iMGLoop)
    {
        uiImages = new List<UIMiniGamePresentationImage>(iMGLoop.inst_miniGames.Count);

        int index = 0;
        float angle_step = Mathf.PI * 2f / (GameData.GetSettings.loopSize + 1);
        float angle = 0f;
        float angle_offset = -77f;
        Vector3 pos = Vector3.zero;

        // Start Image
        pos = new Vector3(
                radius * Mathf.Cos(angle_offset + angle),
                radius * Mathf.Sin(angle_offset + angle),
                0f);
        inst_loopStartImage = GOBuilder.Create(prefab_LoopStartImage)
                                .WithParent(handle_LoopShow)
                                .WithAnchoredPosition(pos)
                                .BuildAs<UILoopStartImage>();

        // Mini Game Images
        index = 1;
        foreach (MiniGame mg in iMGLoop.inst_miniGames)
        {
            angle = (index * angle_step);
            pos = new Vector3(
                radius * Mathf.Cos(angle_offset - angle),
                radius * Mathf.Sin(angle_offset - angle),
                0f);

            UIMiniGamePresentationImage newImg = GOBuilder.Create(prefab_MiniGamePresentationImage)
                                                .WithParent(handle_LoopShow.transform)
                                                .WithAnchoredPosition(pos)
                                                .BuildAs<UIMiniGamePresentationImage>();
            newImg.SetFromMiniGameDesc(mg.descriptor);
            uiImages.Add(newImg);
            index++;
        }

        // rank medals
        rankMedalAnimation.UpdateCurrentRank(iMGLoop);

        DrawUtils.DrawUICircle(handle_LR, radius);
        UpdateLights(iMGLoop);

        init = true;
    }

    public override void Cancel()
    {
        m_Animator.SetBool(DefaultShowAnimParm, false);
        IsShown = false;
        m_Animator.SetTrigger(animTriggerCancel);
        cancellationTokenSource?.Cancel();
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            img.Cancel();
        }
        rankMedalAnimation.Cancel();
        inst_loopStartImage.Cancel();
    }

    public async UniTask Show(MiniGameLoop iMGLoop)
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        await Show(iMGLoop, cancellationTokenSource.Token);
    }

    public async UniTask Show(MiniGameLoop iMGLoop, CancellationToken iCT)
    {
        if (!init)
        { return; }

        UpdateLights(iMGLoop);

        List<UniTask> l = new List<UniTask>();
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            l.Add(img.DefaultShow(iCT));
        }

        await UniTask.WhenAll(
            DefaultShow(iCT),
            UniTask.WhenAll(l),
            inst_loopStartImage.DefaultShow(iCT),
            rankMedalAnimation.DefaultShow(iCT));
        IsShown = true;
    }

    public async UniTask Hide()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        await Hide(cancellationTokenSource.Token);
    }

    public async UniTask Hide(CancellationToken iCT)
    {
        if (!init)
        { return; }

        List<UniTask> l = new List<UniTask>();
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            // if (!img.IsShown)
            //     continue;
            l.Add(img.DefaultHide(iCT));
        }

        await UniTask.WhenAll(
            UniTask.WhenAll(l),
            rankMedalAnimation.DefaultHide(iCT),
            inst_loopStartImage.DefaultHide(iCT),
            DefaultHide(iCT)
            );
        IsShown = false;
    }

    public void UpdateLights(MiniGameLoop iMGLoop)
    {
        if (!init)
        { return; }

        Color light_color = new Color(0f, 0f, 0f, 0f);
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            switch (iMGLoop.GetSuccessState(img.selfDesc))
            {
                case MiniGameSuccessState.PASSED:
                    light_color = GameData.GetUITheme.thumbnailSuccessLightColor;
                    break;
                case MiniGameSuccessState.FAILED:
                    light_color = GameData.GetUITheme.thumbnailFailLightColor;
                    break;
                default:
                    light_color = new Color(1f, 1f, 1f, 0f);
                    break;
            }
            light_color.a = 0f;
            img.UpdateLightColor(light_color);
        }
    }

    public async UniTask ShowLights(MiniGameLoop iMGLoop, bool iState, CancellationToken iCT)
    {
        if (!init)
        { return; }

        float delay = 1f / uiImages.Count;
        int delay_step_in_ms = (int)(delay * GameData.GetSettings.ShowLightTotalTimeInMs);
        delay_step_in_ms = Mathf.Min(delay_step_in_ms, GameData.GetSettings.MaxLightUpMiniGameThumbnailTime);

        if (delay_step_in_ms <= 0)
        {
            foreach (UIMiniGamePresentationImage img in uiImages)
            {
                img.ShowLight(iState);
            }
        }

        Queue<Func<UniTask>> q = new Queue<Func<UniTask>>();
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            q.Enqueue(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    img.ShowLight(iState);
                }
            );
        }

        while (q.Count > 0)
        {
            UniTask.Run(q.Dequeue());
            if (!iCT.IsCancellationRequested)
                await UniTask.Delay(delay_step_in_ms);
        }
        //await WaitAnimState(showLightStateName, 1f, iCT);
    }
    
    public void DisableThumbnailButtons()
    {
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            img.DisableButton();
        }
    }
}
