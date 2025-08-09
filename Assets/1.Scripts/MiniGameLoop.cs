using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniGameLoop : IEnumerator<MiniGame>
{
    public MiniGame Current { get { return miniGame; } }
    object IEnumerator.Current { get { return Current; } }
    MiniGame miniGame;
    List<MiniGame> inst_miniGames;
    public int index = 0;
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
        index = 0;
        miniGame = inst_miniGames[index];
    }

    public MiniGame At(int i)
    {
        return inst_miniGames[i];
    }
    void IDisposable.Dispose()
    {

    }
}
