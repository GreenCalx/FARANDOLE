using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
    public List<Artefact> bag;
    public PlayerData()
    {
        HP = GameData.Get.gameSettings.PlayerHP;
        timeScale = 1f;
        loopHistory = new MiniGameLoopHistory();
        bag = new List<Artefact>();
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

    public void GainArtefact(ArtefactSO iGainedArtefact)
    {
        if (bag.Count > 0)
            if (bag.Where(e => (e.data == iGainedArtefact)).ToList().Count > 0)
                return;
        bag.Add(new Artefact(iGainedArtefact));
    }

    public void RemoveArtefact(ArtefactSO iLostArtefact)
    {
        if (bag == null || bag.Count == 0)
            return;

        var artefact = bag.FirstOrDefault(a => a.data == iLostArtefact);
        if (artefact == null)
            return;

        bag.Remove(artefact);
    }
}
