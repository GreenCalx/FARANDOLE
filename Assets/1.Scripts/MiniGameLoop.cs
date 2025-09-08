using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum LoopRank
{
    I = 0,
    II = 1,
    III = 2,
    S = 3,
    M = 4
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
        rank = LoopRank.I;
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

    public void RankUp()
    {
        if (rank == LoopRank.M)
            return;
        rank = (LoopRank)((int)rank + 1);
    }

    public string GetRankStr()
    {
        return rank.ToString();
    }
}
