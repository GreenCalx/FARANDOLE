using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.UI.Extensions;

public class UILoopPresentationAnim : ManagedAnimation
{
    [Header("UILoopPresentationAnim")]
    public GameObject prefab_MiniGamePresentationImage;
    public RectTransform handle_LoopShow;
    public UILineRenderer handle_LR;
    public UIRankMedalAnim rankMedalAnimation;
    public int timeBetweenShowLinesInMs = 200;
    List<UIMiniGamePresentationImage> uiImages;

    public float radius = 50f;
    public int LR_resolution = 2;
    bool init = false;


    public void Init(MiniGameLoop iMGLoop)
    {
        uiImages = new List<UIMiniGamePresentationImage>(iMGLoop.inst_miniGames.Count);

        int index = 0;
        float angle_step = Mathf.PI * 2f / GameData.GetSettings.loopSize;
        float angle = 0f;
        Vector3 pos = Vector3.zero;

        int lr_index = 0;
        float lr_angle_step = (angle_step > 1f) ? angle_step / LR_resolution : 1f;
        // We're not showing the last joint that makes the full loop
        int n_points = (GameData.GetSettings.loopSize * LR_resolution) - (LR_resolution - 1);
        handle_LR.Points = new Vector2[n_points];
        float lr_angle = 0f;
        Vector3 lr_pos = Vector3.zero;

        foreach (MiniGame mg in iMGLoop.inst_miniGames)
        {
            angle = index * angle_step;
            pos = new Vector3(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle),
                0f);

            UIMiniGamePresentationImage newImg = GOBuilder.Create(prefab_MiniGamePresentationImage)
                                                .WithParent(handle_LoopShow.transform)
                                                .WithAnchoredPosition(pos)
                                                .BuildAs<UIMiniGamePresentationImage>();
            newImg.SetFromMiniGameDesc(mg.descriptor);
            uiImages.Add(newImg);


            lr_index = index * LR_resolution;
            // Add point on self coordinates
            lr_angle = lr_index * lr_angle_step;
            lr_pos = new Vector3(
                    radius * Mathf.Cos(lr_angle),
                    radius * Mathf.Sin(lr_angle),
                    0f);
            handle_LR.Points[lr_index] = lr_pos;
            if (index >= iMGLoop.inst_miniGames.Count - 1)
            {
                // don't finish the loop
                continue;
            }
            // Add resolution points continuing the LR curve
            for (int i = 1; i < LR_resolution; i++)
            {
                lr_angle = (lr_index + i) * lr_angle_step;
                lr_pos = new Vector3(
                    radius * Mathf.Cos(lr_angle),
                    radius * Mathf.Sin(lr_angle),
                    0f);
                handle_LR.Points[lr_index + i] = lr_pos;
            }
            index++;
        }
        //handle_LoopShow.transform.RotateAround(handle_LoopShow.transform.position, Vector3.forward, -45f);
        UpdateLights(iMGLoop);

        init = true;
    }

    public override void Cancel()
    {
        animator.SetBool(DefaultShowAnimParm, false);
        IsShown = false;
        animator.SetTrigger(animTriggerCancel);
        cancellationTokenSource?.Cancel();
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            img.Cancel();
        }
        rankMedalAnimation.Cancel();
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
            rankMedalAnimation.DefaultShow(iCT));
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
            DefaultHide(iCT)
            );
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
                    light_color = GameData.GetSettings.LoopPassedColor;
                    break;
                case MiniGameSuccessState.FAILED:
                    light_color = GameData.GetSettings.LoopFailedColor;
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
        Debug.Log("Show Lights !");
        if (!init)
        { return; }

        float delay = 1f / uiImages.Count;
        int delay_step_in_ms = (int)(delay * 2000f);
        
        Queue<Func<UniTask>> q = new Queue<Func<UniTask>>();
        foreach (UIMiniGamePresentationImage img in uiImages)
        {
            q.Enqueue( async () =>
                {
                    await UniTask.SwitchToMainThread();
                    img.ShowLight(iState);
                }
            );
        }

        while (q.Count > 0)
        {
            UniTask.Run(q.Dequeue());
            await UniTask.Delay(delay_step_in_ms);
        }
        //await WaitAnimState(showLightStateName, 1f, iCT);
    }
}
