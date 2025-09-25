using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
public class UIRankMedalAnim : ManagedAnimation
{
    readonly string RankUpState = "RankUp";
    readonly string RankDownState = "RankDown";
    readonly string RankUpTrigger = "RankUp";
    readonly string RankDownTrigger = "RankDown";
    public TextMeshProUGUI currentRankText;
    public TextMeshProUGUI newRankText;
    public Image currentRankImage;
    public Image newRankImage;
    public async UniTask RankUp(CancellationToken iCT)
    {
        animator.SetTrigger(RankUpTrigger);
        await WaitAnimState(RankUpState, 1f, iCT);
    }

    public async UniTask RankDown(CancellationToken iCT)
    {
        animator.SetTrigger(RankDownTrigger);
        await WaitAnimState(RankDownState, 1f, iCT);
    }

    public void UpdateCurrentRank(MiniGameLoop iMGLoop)
    {
        currentRankText.text = iMGLoop.GetRankStr();
        currentRankImage.sprite = GameData.GetSettings.RankSettings.GetImageFromRank(iMGLoop.rank);
    }

    public void UpdateNewRank(MiniGameLoop iMGLoop)
    {
        newRankText.text = iMGLoop.GetRankStr();
        newRankImage.sprite = GameData.GetSettings.RankSettings.GetImageFromRank(iMGLoop.rank);
    }
}
