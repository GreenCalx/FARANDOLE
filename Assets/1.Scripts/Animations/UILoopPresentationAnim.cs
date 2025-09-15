using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;

public class UILoopPresentationAnim : MonoBehaviour
{
    readonly string ShowLineStateName = "MiniGamePresentationLineShow";
    public GameObject prefab_MiniGamePresentationLine;
    public RectTransform handle_firstElemSpawn;
    public RectTransform handle_lastElemSpawn;
    public int timeBetweenShowLinesInMs = 200;
    List<UIMiniGamePresentationLine> uiLines;
    public CancellationTokenSource cancellationTokenSource;

    public async UniTask Show(MiniGameLoop iMGLoop)
    {
        uiLines = new List<UIMiniGamePresentationLine>(iMGLoop.inst_miniGames.Count);
        Vector3 first_position = handle_firstElemSpawn.anchoredPosition;
        Vector3 last_position = handle_lastElemSpawn.anchoredPosition;
        float frac = 1f / iMGLoop.inst_miniGames.Count;
        int index = 0;
        foreach (MiniGame mg in iMGLoop.inst_miniGames)
        {
            Vector3 pos = Vector3.Lerp(first_position, last_position, index * frac);
            UIMiniGamePresentationLine newLine = GOBuilder.Create(prefab_MiniGamePresentationLine)
                                                .WithParent(transform)
                                                .WithAnchoredPosition(pos)
                                                .BuildAs<UIMiniGamePresentationLine>();
            newLine.SetFromMiniGameDesc(mg.descriptor);
            uiLines.Add(newLine);

            index++;
        }

        cancellationTokenSource = new CancellationTokenSource();
        await ShowLines(cancellationTokenSource.Token);
    }

    async UniTask ShowLines(CancellationToken ct)
    {
        foreach (UIMiniGamePresentationLine l in uiLines)
        {
            l.Show();
            while (!l.self_animator.GetCurrentAnimatorStateInfo(0).IsName(ShowLineStateName))
            {
                await UniTask.Yield();
                if (ct.IsCancellationRequested)
                {
                    //Debug.Log("Cancelled");
                    return;
                }
            }
            while (l.self_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                await UniTask.Yield();
                if (ct.IsCancellationRequested)
                {
                    //Debug.Log("Cancelled");
                    return;
                }
            }
        }
    }

    public void Hide()
    {
        cancellationTokenSource?.Cancel();
        foreach (UIMiniGamePresentationLine l in uiLines)
        {
            if (!l.IsShown)
                continue;
            l.Hide();
        }
    }
}
