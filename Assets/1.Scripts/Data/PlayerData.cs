using UnityEngine;

public class PlayerData
{
    public bool FullLoopCompleted = false;
    public float HP;
    public double score {
        get
        {
            return GetLoopScore();
        }
    }
    public float timeScale;
    public MiniGameLoopHistory loopHistory;
    public PlayerData()
    {
        HP = GameData.Get.gameSettings.PlayerHP;
        timeScale = 1f;
        loopHistory = new MiniGameLoopHistory();
    }

    public void LoseHP(float iLostHP)
    {
        HP -= iLostHP;
    }

    public double GetLoopScore()
    {
        double ls = 0;
        foreach (MiniGameLoopSnapshot snap in loopHistory)
        {
            ls += snap.LoopScore;
        }
        return ls;
    }

    public LoopRank GetMaxRank()
    {
        LoopRank maxRank = loopHistory.RankOnFullCompletion;
        foreach (MiniGameLoopSnapshot snap in loopHistory)
        {
            if (snap.completionRank > maxRank)
                maxRank = snap.completionRank;
        }
        return maxRank;
    }
}
