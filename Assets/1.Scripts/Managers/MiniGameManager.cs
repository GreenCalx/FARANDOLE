using UnityEngine;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class MiniGameManager : MonoBehaviour, IManager
{
    [Header("Debug MiniGame")]
    public GameObject MiniGameToTest = null;
    [Header("MGM Set")]
    public List<GameObject> prefab_miniGames;
    [Header("Internals")]
    //public List<MiniGame> miniGames; // > TODO : Make 'MGLoop' 
    public MiniGameLoop MGLoop;
    public GameClock gameClock;
    public int miniGamesDifficulty;
    
    public UnityEvent<float> OnHPLossCB;
    public UnityEvent<int> OnScoreGainCB;
    public UnityEvent OnLoopComplete;
    public UnityEvent OnMiniGameComplete;
    public UnityEvent OnMiniGameTransitionCB;
    public UnityEvent<bool, float> ShowPostGameUICB;
    public LayerManager2D LM2D;
    public PlayerController PC;
    public PlaygroundManager PG;
    public PlayerData PData;
    public UIGame UI;

    #region IManager
    public void Init(GameManager iGameManager)
    {
        gameClock = new GameClock();
        miniGamesDifficulty = 1;

        OnHPLossCB = new UnityEvent<float>();
        PC = iGameManager.PC;
        PG = iGameManager.PG;
        LM2D = iGameManager.LM2D;
        PData = iGameManager.playerData;
        UI = iGameManager.UI;
    }
    
    public bool IsReady()
    {
        return true;
    }
    #endregion

    public void LoadLoop()
    {
        if (MiniGameToTest != null)
        {
            prefab_miniGames.Clear();
            prefab_miniGames.Add(MiniGameToTest);
        }
        else
        { // Random seed
            prefab_miniGames = GameData.GetMGBank.GetRandom(5);
        }
        MGLoop = new MiniGameLoop(this, prefab_miniGames);
    }

    public void Play()
    {
        MGLoop.Current.gameObject.SetActive(true);
        MGLoop.Current.IsInPostGame = false;
        MGLoop.Current.Init();
        MGLoop.Current.successState = MiniGameSuccessState.PENDING;

        ShowPostGameUICB.Invoke(false, GameData.Get.gameSettings.MiniGameTime - gameClock.GetElapsedTime());
        UI.RefreshLoopStage(MGLoop.index, MGLoop.Current.successState);

        MGLoop.Current.Play();
        gameClock.Reset();
    }

    public void Stop()
    {
        MGLoop.Current.Stop();
        MGLoop.Current.gameObject.SetActive(false);
        PC.ClearAllTrackers();
        LM2D.ClearLayers();
    }

    public void WinMiniGame()
    {
        if (MGLoop.Current.IsInPostGame)
        {
            Debug.LogWarning("WinMiniGame fired multiple times by current Minigame. Not good !!");
            return;
        }
        gameClock.Freeze(true);
        MGLoop.Current.IsInPostGame = true;

        float miniGameDuration = GameData.Get.gameSettings.MiniGameTime - gameClock.GetElapsedTime();
        MGLoop.Current.successState = (gameClock.GetElapsedTime() > GameData.Get.gameSettings.MiniGameTime) ?
            MiniGameSuccessState.FAILED : MiniGameSuccessState.PASSED;

        ShowPostGameUICB.Invoke(true, miniGameDuration);
        OnMiniGameComplete.Invoke();
        //UI.RefreshLoopStage(MGLoop.index, MGLoop.Current.successState);

        OnScoreGainCB.Invoke(1);

        DelayedNext();
    }

    async void DelayedNext()
    {
        await Task.Delay(GameData.GetSettings.PostMiniGameLatchInMs);

        Next();
    }

    async void Next()
    {
        Stop();
        
        if (!MGLoop.MoveNext())
        {
            OnLoopComplete.Invoke();
            MGLoop.Reset();
        }

        OnMiniGameTransitionCB.Invoke();
        await Task.Delay(GameData.GetSettings.PreMiniGameLatchInMs);

        Play();
    }

    public void RaiseDifficulty()
    {
        if (miniGamesDifficulty >= GameData.GetSettings.MaxMiniGameDifficulty)
            return;
        miniGamesDifficulty++;
    }

    public LoopHighScore GetLoopHighScore()
    {
        int loopSize = GameData.GetSettings.loopSize;
        byte[] gameIDs = new byte[loopSize];
        for (int i = 0; i < loopSize; i++)
        {
            gameIDs[i] = MGLoop.At(i).ID;
        }
        // TODO : Fetch time from server to ensure that
        // the datetime is right as the current impl depends on 
        // the device time ( which can be modified )
        return new LoopHighScore(GameData.Get.currentGameMode, gameIDs, PData.score, DateTime.Now);
    }

    void Update()
    {
        if (MGLoop.Current.IsInPostGame)
            return;

        gameClock.Tick();
            
        if (gameClock.MiniGameTimeExpired())
        {
            // Lose hp
            OnHPLossCB.Invoke(Time.deltaTime);
        }
    }

}
