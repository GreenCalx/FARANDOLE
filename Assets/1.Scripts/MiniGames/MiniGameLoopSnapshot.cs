using UnityEngine;
using System.Linq;
public class MiniGameLoopSnapshot
{
    public LoopRank completionRank;
    public int comboMultiplier;
    public int succeededStages;
    public int LoopScore
    {
        get
        {
            return (int)(completionRank + 1) * succeededStages;
        }
    }
    public MiniGameLoopSnapshot(MiniGameLoop iOriginator)
    {
        completionRank = iOriginator.rank;
        succeededStages = iOriginator.GetSuccessStates().Where(e => (e == MiniGameSuccessState.PASSED)).ToList().Count;
        comboMultiplier = iOriginator.combo;
    }


}
