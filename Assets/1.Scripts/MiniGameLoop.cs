using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum LoopRank
{
    Z = 0,
    I = 1,
    II = 2,
    III = 3,
    S = 4,
    M = 5
}

public class MiniGameLoop : IEnumerator<MiniGame>
{
    public MiniGame Current { get { return miniGame; } }
    object IEnumerator.Current { get { return Current; } }
    MiniGame miniGame;
    public List<MiniGame> inst_miniGames;
    public int index = 0;
    public LoopRank rank;
    public int depth;
    public bool IsRankUpdateRequested {
        get { return rankUpdateRequest; }
    }
    bool rankUpdateRequest = false;
    public MiniGameLoop(MiniGameManager iMGM, List<GameObject> iPrefabs)
    {
        inst_miniGames = new List<MiniGame>();
        foreach (GameObject prefab in iPrefabs)
        {
            GameObject new_mg = GOBuilder.Create(prefab).Build();
            MiniGame as_mg = new_mg.GetComponent<MiniGame>();
            if (as_mg == null)
                return;
            as_mg.MGM = iMGM;
            as_mg.PC = iMGM.PC;
            as_mg.PG = iMGM.PG;
            inst_miniGames.Add(as_mg);
            new_mg.SetActive(false);
        }
        rank = LoopRank.Z;
        depth = 0;
        Reset();
    }
    public bool MoveNext()
    {
        if (++index >= inst_miniGames.Count)
            return false;
        miniGame = inst_miniGames[index];
        return true;
    }
    public void Reset()
    {
        foreach (MiniGame mg in inst_miniGames)
        {
            mg.successState = MiniGameSuccessState.PENDING;
        }
        index = 0;
        miniGame = inst_miniGames[index];
    }

    public MiniGame At(int i)
    {
        return inst_miniGames[i];
    }
    public bool IsLoopPassed()
    {
        int n_passed = 0;
        foreach (MiniGame mg in inst_miniGames)
        {
            if (mg.successState == MiniGameSuccessState.PASSED)
                n_passed++;
        }
        return n_passed >= GameData.GetSettings.loopPassThreshold; ;
    }

    public bool IsLoopPerfect()
    {
        foreach (MiniGame mg in inst_miniGames)
        {
            if (mg.successState != MiniGameSuccessState.PASSED)
            {
                return false;
            }
        }
        return true;
    }

    public MiniGameSuccessState[] GetSuccessStates()
    {
        MiniGameSuccessState[] states = new MiniGameSuccessState[inst_miniGames.Count];
        for (int i = 0; i < inst_miniGames.Count; i++)
        {
            states[i] = inst_miniGames[i].successState;
        }

        return states;
    }
    void IDisposable.Dispose()
    {

    }

    public void RankUpdate()
    {
        rankUpdateRequest = false;
    
        switch (rank)
        {
            case LoopRank.Z:
                RankUp();
                rankUpdateRequest = true;
                break;
            case LoopRank.I:
                if (!IsLoopPassed())
                    return;
                else if (IsLoopPerfect())
                {
                    RankUp();
                    rankUpdateRequest = true;
                }

                break;
            case LoopRank.II:
                if (!IsLoopPassed())
                {
                    RankDown();
                    rankUpdateRequest = true;
                }
                else if (IsLoopPerfect())
                {
                    RankUp();
                    rankUpdateRequest = true;
                }
                break;
            case LoopRank.III:
                if (!IsLoopPassed())
                {
                    RankDown();
                    rankUpdateRequest = true;
                }
                // super loop check
                break;
            case LoopRank.S:
                // master loop check
                break;
            default:
                Debug.LogWarning("tryRankUp:: Unkown loop rank : " + (int)rank);
                break;
        }
    }

    public void RankUp()
    {
        int vrank = (int)rank;
        int l = Enum.GetNames(typeof(LoopRank)).Length;
        if (vrank >= l-1)
            return;
        rank = (LoopRank)(vrank + 1);
    }

    public void RankDown()
    {
        int vrank = (int)rank;
        if (vrank>1)
        {
            rank = (LoopRank)(vrank - 1);
        }
    }

    public string GetRankStr()
    {
        return rank.ToString();
    }
}
