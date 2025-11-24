using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Threading;
using Cysharp.Threading.Tasks;
using static GOBuilder;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Get => instance;

    public bool GameStarted = false;
    [Header("Managers")]
    public MiniGameManager MGM;
    public GameSceneManager GSM;
    public LayerManager2D LM2D;
    public PlaygroundManager PG;
    public AnimationManager ANIM;
    public UIGame UI;
    public AudioManager AUDIO;

    [Header("Extra Refs")]
    public PlayerController PC;
    public PlayerData playerData;
    public GameObject prefab_UIGameOver;
    [Header("Audio Refs")]
    
    UIGameOver inst_UIGameOver;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        // ensure PlayerController is loaded
        // before doing anything
        while (PC == null)
        { yield return null; }

        LM2D.Init(this);
        while (!LM2D.IsReady())
        { yield return null; }

        playerData = new PlayerData();
        PG.Init(this);
        while (!PG.IsReady())
        { yield return null; }
        PG.ForceDoorClose();

        MGM.Init(this);
        MGM.LoadLoop();
        while (!MGM.IsReady())
        { yield return null; }

        GSM.Init(this);
        while (!GSM.IsReady())
        { yield return null; }

        ANIM.Init(this);
        while (!ANIM.IsReady())
        { yield return null; }

        AUDIO.Init(this);
        while (!AUDIO.IsReady())
        { yield return null; }

        UI.Init(this);
        while (!UI.IsReady())
        { yield return null; }
        MGM.UI = UI; // TODO dirty fix

        InitCallbacks();
        UI.PresentLoop();
        UI.launchGameBtn?.clickCallback.AddListener(() => StartGame());
    }

    void InitCallbacks()
    {
        MGM.OnHPLossCB.AddListener(playerData.LoseHP);
        MGM.OnMiniGameComplete.AddListener(OnMiniGameCompletion);
        MGM.OnMiniGameTransitionCB.AddListener(OnMiniGameTransition);

        UI.OnBeforeLoopDepth.AddListener(OnLoopDepthUpdate);
        UI.h_PauseMenu.h_ResumeBtn.clickCallback.AddListener(() => { UI.h_PauseMenu.Toggle(); });
        UI.h_PauseMenu.h_TryAgainBtn.clickCallback.AddListener(() => { UI.h_PauseMenu.Toggle(); RestartGame(); });
        UI.h_PauseMenu.h_ExitBtn.clickCallback.AddListener(() => { UI.h_PauseMenu.Toggle(); ExitToTitle(); });
    }

    void RemoveCallbacks()
    {
        MGM.OnHPLossCB.RemoveListener(playerData.LoseHP);
        MGM.OnMiniGameComplete.RemoveListener(OnMiniGameCompletion);
        MGM.OnMiniGameTransitionCB.RemoveListener(OnMiniGameTransition);

        UI.OnBeforeLoopDepth.RemoveListener(OnLoopDepthUpdate);
        UI.h_PauseMenu.h_ResumeBtn.clickCallback.RemoveListener(() => { UI.h_PauseMenu.Toggle(); });
        UI.h_PauseMenu.h_TryAgainBtn.clickCallback.RemoveListener(() => { UI.h_PauseMenu.Toggle(); RestartGame(); });
        UI.h_PauseMenu.h_ExitBtn.clickCallback.RemoveListener(() => { UI.h_PauseMenu.Toggle(); ExitToTitle(); });
    }

    async UniTaskVoid StartGame()
    {
        UI.launchGameBtn?.clickCallback.RemoveListener(() => StartGame());
        UI.launchGameBtn.gameObject.SetActive(false);
        UI.h_LaunchGameCanvas.gameObject.SetActive(false);
        if ((UI.inst_loopPresentationAnim.IsShown!=null) && UI.inst_loopPresentationAnim.IsShown)
            await UI.HideLoopPresentation();

        if (inst_UIGameOver != null)
        {
            Destroy(inst_UIGameOver.gameObject);
            inst_UIGameOver = null;
        }
        //InitCallbacks();

        await MGM.Launch();
        UI.ShowMiniGameMode(true);
        GameStarted = true;
    }

    void StopGame()
    {
        GameStarted = false;
        PG.StopAnimation();
        PC.Freeze();
        

        RemoveCallbacks();
        UI.ShowMiniGameMode(false);
    }

    void RestartGame()
    {
        // Its quite overkill, but requires no maintenance and is less likely to produce buggy behaviours
        // Also it can be smoothed by a dedicated scene loader manager at some point..
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMiniGameCompletion()
    {

    }

    public async void OnMiniGameTransition()
    {
        // nothing ?
    }

    void OnLoopDepthUpdate()
    {
        // playGround Mat update
        PG.RefreshRendering(MGM.MGLoop.depth, MGM.MGLoop.HasRankChanged, MGM.MGLoop.rank);

        // Global time scale update based on depth
        AnimationCurve timeScaleCurve = GameData.Get.gameSettings.timeScaleOverLoopLevel;
        if (MGM.MGLoop.depth > timeScaleCurve.keys[timeScaleCurve.length - 1].time)
            return;
        playerData.timeScale = timeScaleCurve.Evaluate(MGM.MGLoop.depth);
        Time.timeScale = playerData.timeScale;

        AUDIO.gameBGM.RefreshSpeed(playerData.timeScale);
    }

    public void GameOver()
    {
        Time.timeScale = 1f;

        PC.Freeze();
        PC.Flush();

        playerData.loopHistory.RankOnFullCompletion = MGM.MGLoop.rank;
        MGM.GameOver();

        StopGame();
        UI.ShowMiniGameMode(false);
        
        CancellationTokenSource cts = new CancellationTokenSource();

        inst_UIGameOver = GOBuilder.Create(prefab_UIGameOver).BuildAs<UIGameOver>();
        inst_UIGameOver.Setup(this);
        inst_UIGameOver.TryAgainBtn.clickCallback.AddListener(() => { cts.Cancel(); RestartGame(); });
        inst_UIGameOver.MenuBtn.clickCallback.AddListener(() => { cts.Cancel(); ExitToTitle(); });

        bool IsHighScore = PostGameScoreProcessing();

        inst_UIGameOver.scoreDisplayValue.text = playerData.score.ToString();

        inst_UIGameOver.Animate(IsHighScore, PG, cts.Token);
    }

    public void OnFullLoopCompleted()
    {
        playerData.FullLoopCompleted = true;
        GameOver();
    }


    void Update()
    {
        if (!GameStarted)
            return;

        UI.Refresh();

        if (playerData.HP <= 0)
        {
            GameOver();
        }
    }

    public void ExitToTitle()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public bool PostGameScoreProcessing()
    {
        int score = playerData.GetLoopScore();
        LoopRank maxRank = playerData.GetMaxRank();

        int loopSize = GameData.GetSettings.loopSize;
        byte[] gameIDs = new byte[loopSize];
        for (int i = 0; i < loopSize; i++)
        {
            gameIDs[i] = MGM.MGLoop.At(i).descriptor.ID;
        }
        LoopHighScore lhs = new LoopHighScore(GameData.Get.currentGameMode, gameIDs, score, DateTime.Now);
        lhs.maxRank = maxRank;
        LoopHighScore replacedHS = null;
        if (UserData.IsNewHighScore(lhs, out replacedHS))
        {
            if (replacedHS != null)
                UserData.RemoveHighScore(replacedHS);
            UserData.AddHighScore(lhs);
            // <!> load all high score beforehand to avoid overwriting prev data
            UserData.SaveHighScores();

            // inst_UIGameOver.scoreDisplayHandle.gameObject.SetActive(false);
            // inst_UIGameOver.newHighScoreDisplayHandle.gameObject.SetActive(true);
            return true;
        }
        return false;
        // else
        // {
        //     inst_UIGameOver.scoreDisplayHandle.gameObject.SetActive(true);
        //     inst_UIGameOver.newHighScoreDisplayHandle.gameObject.SetActive(false);
        // }
    }


}
