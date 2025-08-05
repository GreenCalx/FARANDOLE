using UnityEngine;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class MiniGameManager : MonoBehaviour, IManager
{
    [Header("MGM Set")]
    public List<GameObject> prefab_miniGames;
    [Header("Internals")]
    public List<MiniGame> miniGames;
    public GameClock gameClock;
    public int miniGamesDifficulty;
    int currIndex = -1;
    public UnityEvent<float> OnHPLossCB;
    public UnityEvent<int> OnScoreGainCB;
    public UnityEvent OnLoopComplete;
    public UnityEvent<bool> ShowPostGameUICB;
    public LayerManager2D LM2D;
    float gameStartTime;
    PlayerController PC;
    PlaygroundManager PG;


    #region IManager
    public void Init(GameManager iGameManager)
    {
        gameClock = new GameClock();
        miniGamesDifficulty = 1;
        currIndex = 0;
        OnHPLossCB = new UnityEvent<float>();
        PC = iGameManager.PC;
        PG = iGameManager.PG;
        LM2D = iGameManager.LM2D;
    }
    
    public bool IsReady()
    {
        return true;
    }
    #endregion

    public void WarmUpMiniGames()
    {
        miniGames = new List<MiniGame>();
        foreach (GameObject prefab in prefab_miniGames)
        {
            GameObject new_mg = Instantiate(prefab);
            MiniGame as_mg = new_mg.GetComponent<MiniGame>();
            if (as_mg == null)
                return;
            as_mg.MGM = this;
            as_mg.PC = PC;
            as_mg.PG = PG;
            miniGames.Add(as_mg);
            new_mg.SetActive(false);
        }
    }

    public void Play()
    {
        miniGames[currIndex].gameObject.SetActive(true);
        miniGames[currIndex].IsInPostGame = false;
        miniGames[currIndex].Init();
        ShowPostGameUICB.Invoke(false);

        miniGames[currIndex].Play();
        gameClock.Reset();
    }

    public void Stop()
    {
        miniGames[currIndex].Stop();
        miniGames[currIndex].gameObject.SetActive(false);
        PC.ClearAllTrackers();
        LM2D.ClearLayers();
    }

    public void WinMiniGame()
    {
        if (miniGames[currIndex].IsInPostGame)
        {
            Debug.LogWarning("WinMiniGame fired multiple times by current Minigame. Not good !!");
            return;
        }
        gameClock.Freeze(true);
        miniGames[currIndex].IsInPostGame = true;
        ShowPostGameUICB.Invoke(true);
        OnScoreGainCB.Invoke(1);
        DelayedNext();
    }

    async void DelayedNext()
    {
        await Task.Delay(GameData.GetSettings.PostMiniGameLatchInMs);
        Next();
    }

    void Next()
    {
        if (currIndex >= 0)
        {
            Stop();
        }
        if (++currIndex >= miniGames.Count)
        {
            OnLoopComplete.Invoke();
            currIndex = 0;
        }
        Play();
    }

    public void RaiseDifficulty()
    {
        if (miniGamesDifficulty >= GameData.GetSettings.MaxMiniGameDifficulty)
            return;
        miniGamesDifficulty++;
    }

    void Update()
    {
        if (miniGames[currIndex].IsInPostGame)
            return;

        gameClock.Tick();
            
        if (gameClock.MiniGameTimeExpired())
        {
            // Lose hp
            OnHPLossCB.Invoke(Time.deltaTime);
        }
    }

}
