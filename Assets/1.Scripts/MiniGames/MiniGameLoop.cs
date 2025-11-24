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
    M = 4,
    D = 5
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
    public int combo;
    public int previousCombo;
    public bool IsFinalLoop => depth >= GameData.GetSettings.MaxLoopDepth;
    public int count => inst_miniGames!=null ? inst_miniGames.Count : 0;
    public bool IsRankUpdateRequested {
        get { return rankUpdateRequest; }
    }
    bool rankUpdateRequest = false;

    public bool HasRankChanged
    {
        get { return rankChanged; }
    }

    public float TotalSavedTime
    {
        get
        {
            float retval= 0f;
            foreach (MiniGame mg in inst_miniGames)
            {
                retval += GameData.GetSettings.MiniGameTime - mg.CompletionTime;
            }
            return retval;
        }
    }
    bool rankChanged = false;
    public MiniGameLoop()
    {
        combo = 0;
        previousCombo = 0;
        depth = 0;
        rank = LoopRank.I;
        index = 0;
        rankUpdateRequest = false;
        rankChanged = false;
        inst_miniGames = new List<MiniGame>();
    }

    public void Init(MiniGameManager iMGM, List<GameObject> iPrefabs)
    {
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
            as_mg.Init();
            new_mg.SetActive(false);
        }

        //Start();
    }

    public bool MoveNext()
    {
        if (++index >= inst_miniGames.Count)
            return false;
        miniGame = inst_miniGames[index];
        return true;
    }

    public void Start()
    {
        ResetAll();

        miniGame.gameObject.SetActive(true);
        miniGame.IsInPostGame = false;
        miniGame.successState = MiniGameSuccessState.PENDING;
    }
    public void ResetAll()
    {
        Reset();
        depth = 0;
        rank = LoopRank.I;
        combo = 0;
        previousCombo = 0;
    }
    public void Reset()
    {
        foreach (MiniGame mg in inst_miniGames)
        {
            mg.successState = MiniGameSuccessState.PENDING;
            mg.IsInPostGame = false;
        }
        index = 0;
        miniGame = inst_miniGames[index];
        rankUpdateRequest = false;
        rankChanged = false;
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
        if (inst_miniGames.Count == 1)
        {
            return n_passed == 1;
        }
        else
        {
            return n_passed >= GameData.GetSettings.loopPassThreshold; ;
        }
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

    public MiniGameSuccessState GetSuccessState(MiniGameSO iMGDesc)
    {
        foreach (MiniGame mg in inst_miniGames)
        {
            if (mg.descriptor == iMGDesc)
            {
                return mg.successState;
            }
        }
        return MiniGameSuccessState.NONE;
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

    public void ComboUpdate()
    {
        previousCombo = combo;

        if (IsLoopPerfect())
            combo++;
        else if (!IsLoopPassed())
            combo = 0;
    }

    public void RankUpdate()
    {
        rankUpdateRequest = false;
        rankChanged = false;

        switch (rank)
        {
            case LoopRank.I:
                if (IsLoopPerfect())
                    RankUp();
                else
                    return;
                break;
            case LoopRank.II:
                if (!IsLoopPassed())
                    RankDown();
                else if (IsLoopPerfect())
                    RankUp();
                else
                    return;
                break;
            case LoopRank.III:
                if (!IsLoopPassed())
                    RankDown();
                else if (IsLoopPerfect())
                {
                    // super loop check
                    if (combo >= GameData.GetSettings.ComboRequirementForSuper)
                        RankUp();
                } else
                {
                    return;
                }
                break;
            case LoopRank.S:
                if (!IsLoopPassed())
                    RankDown();
                else if (IsLoopPerfect())
                {
                    // master loop check
                    // TODO : not a simple combo requirement
                    if (combo >= GameData.GetSettings.ComboRequirementForMaster)
                        RankUp();
                } else
                {
                    return;
                }
                break;
            case LoopRank.M:
                if (!IsLoopPassed())
                    RankDown();
                else if (IsLoopPerfect() && (combo>=GameData.GetSettings.MaxLoopDepth))
                    RankUp();
                break;
            default:
                Debug.LogWarning("tryRankUp:: Unkown loop rank : " + (int)rank);
                return;
        }
        rankChanged = true;
    }

    public void RankUp()
    {
        int vrank = (int)rank;
        int l = Enum.GetNames(typeof(LoopRank)).Length;
        if (vrank >= l - 1)
            return;
        rank = (LoopRank)(vrank + 1);
        
        rankUpdateRequest = true;
    }

    public void RankDown()
    {
        int vrank = (int)rank;
        if (vrank > 1)
        {
            rank = (LoopRank)(vrank - 1);
        }
        
        rankUpdateRequest = true;
    }

    public string GetRankStr()
    {
        return rank.ToString();
    }
}
