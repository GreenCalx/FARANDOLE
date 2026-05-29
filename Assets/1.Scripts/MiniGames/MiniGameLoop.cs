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
    public MiniGame Current { get { return socket.inst_miniGame; } }
    object IEnumerator.Current { get { return Current; } }
    public MiniGameLoopSocket socket;
    public List<MiniGameLoopSocket> sockets;
    public int index = 0;
    public LoopRank rank;
    public int depth;
    int m_Combo;
    public int combo
    {
        set { m_Combo = Mathf.Clamp(value, 0, GameData.GetSettings.MaxComboPoints); }
        get { return m_Combo; }
    }
    public bool IsFullCombo => combo >= GameData.GetSettings.MaxComboPoints;
    public int previousCombo;
    public bool IsFinalLoop => depth >= GameData.GetSettings.MaxLoopDepth;
    public int count => sockets!=null ? sockets.Count : 0;
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
            foreach (MiniGameLoopSocket s in sockets)
            {
                if ((s.inst_miniGame.successState==MiniGameSuccessState.NONE)||(s.inst_miniGame.successState==MiniGameSuccessState.PENDING))
                    continue;
                retval += GameData.GetSettings.MiniGameTime - s.inst_miniGame.CompletionTime;
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
        sockets = new List<MiniGameLoopSocket>();

    }

    public void Init(MiniGameManager iMGM, List<GameObject> iPrefabs)
    {
        foreach (GameObject prefab in iPrefabs)
        {
            GameObject new_mg = GOBuilder.Create(prefab).Build();
            MiniGame as_mg = new_mg.GetComponent<MiniGame>();
            if (as_mg == null)
            {
                Debug.LogError($"MiniGameLoop.Init: prefab '{prefab.name}' has no MiniGame component. Skipping.");
                UnityEngine.Object.Destroy(new_mg);
                continue;
            }
            as_mg.MGM = iMGM;
            as_mg.PC = iMGM.PC;
            as_mg.PG = iMGM.PG;
            sockets.Add(new MiniGameLoopSocket(as_mg));
            as_mg.Init();
            new_mg.SetActive(false);
        }

        //Start();
    }

    public bool MoveNext()
    {
        if (++index >= sockets.Count)
            return false;

        socket = sockets[index];
        return true;
    }

    public void Start()
    {
        ResetAll();
        PlayCurrent();
    }

    public void PlayCurrent()
    {
        socket.Run();
    }
    public void StopCurrent()
    {
        socket.Stop();
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
        foreach (MiniGameLoopSocket socket in sockets)
        {
            socket.Reset();
        }
        index = 0;
        if (sockets.Count == 0)
        {
            Debug.LogError("MiniGameLoop.Reset: sockets list is empty.");
            return;
        }
        socket = sockets[index];
        rankUpdateRequest = false;
        rankChanged = false;
    }

    public MiniGame At(int i)
    {
        return sockets[i].inst_miniGame;
    }
    
    public bool IsLoopPassed()
    {
        int n_passed = 0;
        foreach (MiniGameLoopSocket socket in sockets)
        {
            if (socket.IsValidated())
                n_passed++;
        }
        if (sockets.Count == 1)
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
        foreach (MiniGameLoopSocket socket in sockets)
        {
            if (!socket.IsValidated())
            {
                return false;
            }
        }
        return true;
    }

    public MiniGameSuccessState GetSuccessState(MiniGameSO iMGDesc)
    {
        foreach (MiniGameLoopSocket socket in sockets)
        {
            if (socket.inst_miniGame.descriptor == iMGDesc)
            {
                return socket.inst_miniGame.successState;
            }
        }
        return MiniGameSuccessState.NONE;
    }

    public MiniGameSuccessState[] GetSuccessStates()
    {
        MiniGameSuccessState[] states = new MiniGameSuccessState[sockets.Count];
        for (int i = 0; i < sockets.Count; i++)
        {
            states[i] = sockets[i].inst_miniGame.successState;
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
                    if (IsFullCombo)
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

    public void RefreshMutations()
    {
        List<Artefact> artefacts = GameManager.Get.playerData.bag;
        Debug.Log($"RefreshMutations: Applying {artefacts.Count} artefacts to {sockets.Count} mini-games");
        foreach (Artefact art in artefacts)
        {
            foreach(MiniGameLoopSocket socket in sockets)
            {
                foreach(IMiniGameMod mod in art.mods)
                {
                    mod.Apply(socket);
                }
            }
        }
    }
}
