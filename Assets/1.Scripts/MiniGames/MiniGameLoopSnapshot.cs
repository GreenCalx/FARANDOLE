using UnityEngine;
using System.Linq;
public class MiniGameLoopSnapshot
{
    public LoopRank completionRank;
    public int comboMultiplier;
    public int succeededStages;
    public bool IsPerfect = false;
    public bool IsFailed = false;
    public float SavedTime = 0f;
    public int depth;
    public int LoopScore
    {
        get
        {
            return (((int)(completionRank)) + 1) * succeededStages * comboMultiplier;
        }
    }
    public MiniGameLoopSnapshot(MiniGameLoop iOriginator)
    {
        completionRank = iOriginator.rank;
        succeededStages = iOriginator.GetSuccessStates().Where(e => (e == MiniGameSuccessState.PASSED)).ToList().Count;
        IsPerfect = iOriginator.IsLoopPerfect();
        IsFailed = !iOriginator.IsLoopPassed();
        comboMultiplier = iOriginator.combo;
        SavedTime = iOriginator.TotalSavedTime; 
        depth = iOriginator.depth;
    }


}
