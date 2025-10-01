using UnityEngine;
using UnityEngine.Events;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
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
    public int miniGamesDifficulty
    {
        get {
            if (MGLoop==null)
                return 0;
            return (int)MGLoop.rank+1;
            }
    }
    
    public UnityEvent<float> OnHPLossCB;
    public UnityEvent OnLoopComplete;
    public UnityEvent OnMiniGameComplete;
    public UnityEvent OnMiniGameTransitionCB;
    public UnityEvent<float> ShowPostGameUICB;
    public LayerManager2D LM2D;
    public PlayerController PC;
    public PlaygroundManager PG;
    public PlayerData PData;
    public UIGame UI;
    CancellationTokenSource LoopCompleteAnimCTS;

    #region IManager
    public void Init(GameManager iGameManager)
    {
        gameClock = new GameClock();

        OnHPLossCB = new UnityEvent<float>();
        PC = iGameManager.PC;
        PG = iGameManager.PG;
        LM2D = iGameManager.LM2D;
        PData = iGameManager.playerData;
        UI = iGameManager.UI;

        LoadLoop();
    }
    
    public bool IsReady()
    {
        return MGLoop!=null;
    }
    #endregion

    public void LoadLoop()
    {
        if (MiniGameToTest != null)
        {
            Debug.LogWarning("MINI GAME TEST : Be sure to have a loopSize of 1 in the settings");
            prefab_miniGames.Clear();
            prefab_miniGames.Add(MiniGameToTest);
        }
        else
        { // Random seed
            prefab_miniGames = GameData.GetMGBank.GetRandom(GameData.GetSettings.loopSize);
        }
        BuildLoop();
    }

    public void BuildLoop()
    {
        MGLoop = new MiniGameLoop();
        MGLoop.Init(this, prefab_miniGames);
    }

    // public void Restart()
    // {
    //     // gameClock.Reset();
    //     // MGLoop.Start();
    // }
    public async UniTask Launch()
    {
        MGLoop.Start();
        PlayCurrent();
    }

    public async UniTask PlayCurrent()
    {
        MGLoop.Current.gameObject.SetActive(true);
        MGLoop.Current.Reset();
        
        PC.Freeze();
        await PG.OpenPlaygroundAnim();
        PC.UnFreeze();

        //MGLoop.Current.Reset();
        MGLoop.Current.Play();
        gameClock.Reset();
    }

    public void StopCurrent()
    {
        gameClock.Freeze(true);

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

        ShowPostGameUICB.Invoke(miniGameDuration);
        OnMiniGameComplete.Invoke();
        //UI.RefreshLoopStage(MGLoop.index, MGLoop.Current.successState);

        DelayedNext();
    }

    async void DelayedNext()
    {
        await Task.Delay(GameData.GetSettings.PostMiniGameLatchInMs);

        OnMiniGameTransitionCB.Invoke();
        //Stop();

        Next();
    }

    async void Next()
    {
        await PG.ClosePlaygroundAnim();
        StopCurrent();

        if (!MGLoop.MoveNext())
        {
            MGLoop.depth++;
            OnLoopComplete.Invoke();

            // Cancel animation init
            LoopCompleteAnimCTS = new CancellationTokenSource();
            UI.skipAnimBtn.clickCallback.AddListener(() => LoopCompleteAnimCTS.Cancel());

            // Rank update
            UI.inst_loopPresentationAnim.rankMedalAnimation.UpdateCurrentRank(MGLoop);
            MGLoop.RankUpdate();
            if (MGLoop.IsRankUpdateRequested)
            {
                UI.inst_loopPresentationAnim.rankMedalAnimation.UpdateNewRank(MGLoop);
            }
                
            // play animation
            await UI.LoopCompleteAnim(MGLoop, LoopCompleteAnimCTS.Token);

            UI.skipAnimBtn.clickCallback.RemoveListener(() => LoopCompleteAnimCTS.Cancel());
            
            MGLoop.Reset();
        }
        //await PG.OpenPlaygroundAnim();
        PlayCurrent();
    }

    public LoopHighScore GetLoopHighScore()
    {
        int loopSize = GameData.GetSettings.loopSize;
        byte[] gameIDs = new byte[loopSize];
        for (int i = 0; i < loopSize; i++)
        {
            gameIDs[i] = MGLoop.At(i).descriptor.ID;
        }
        // TODO : Fetch time from server to ensure that
        // the datetime is right as the current impl depends on 
        // the device time ( which can be modified )
        return new LoopHighScore(GameData.Get.currentGameMode, gameIDs, PData.score, DateTime.Now);
    }

    void Update()
    {
        UI.RefreshTimeIndicator(gameClock);

        if (gameClock.IsFrozen)
            return;

        if (MGLoop.Current.IsInPostGame)
                return;

        gameClock.Tick();

        if (gameClock.MiniGameTimeExpired())
        {
            // Lose hp
            OnHPLossCB.Invoke(Time.deltaTime);
        }
        // TODO : Fire event upon critical gameclock changes
        // aka make this part of UI a clock listener.
    }

}
