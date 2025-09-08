using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Threading;
using static GOBuilder;

public class GameManager : MonoBehaviour
{
    public bool GameStarted = false;
    [Header("Managers")]
    public MiniGameManager MGM;
    public GameSceneManager GSM;
    public LayerManager2D LM2D;
    public PlaygroundManager PG;
    public AnimationManager ANIM;
    public UIGame UI;

    [Header("Extra Refs")]
    public PlayerController PC;
    public PlayerData playerData;
    public GameObject prefab_UIGameOver;
    UIGameOver inst_UIGameOver;
    
    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
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

        UI.Init(this);
        while (!UI.IsReady())
        { yield return null; }
        UI.RefreshLoopLevelText(MGM.MGLoop.GetRankStr());
        UI.loopPresentationAnim.Show(MGM.MGLoop);

        //StartGame();
        UI.launchGameBtn?.clickCallback.AddListener(() => StartGame());
    }

    void InitCallbacks()
    {
        MGM.OnHPLossCB.AddListener(playerData.LoseHP);
        UI.OnBeforeLoopDepth.AddListener(OnLoopDepthUpdate);
        MGM.OnMiniGameComplete.AddListener(OnMiniGameCompletion);
        MGM.OnMiniGameTransitionCB.AddListener(OnMiniGameTransition);
        MGM.ShowPostGameUICB.AddListener(UI.ShowSuccessArea);
    }

    void RemoveCallbacks()
    {
        MGM.OnHPLossCB.RemoveListener(playerData.LoseHP);
        UI.OnBeforeLoopDepth.RemoveListener(OnLoopDepthUpdate);
        MGM.OnMiniGameComplete.RemoveListener(OnMiniGameCompletion);
        MGM.OnMiniGameTransitionCB.RemoveListener(OnMiniGameTransition);
        MGM.ShowPostGameUICB.RemoveListener(UI.ShowSuccessArea);

    }

    void StartGame()
    {
        UI.launchGameBtn?.clickCallback.RemoveListener(() => StartGame());
        UI.launchGameBtn.gameObject.SetActive(false);
        UI.loopPresentationAnim.Hide();

        if (inst_UIGameOver != null)
        {
            Destroy(inst_UIGameOver.gameObject);
            inst_UIGameOver = null;
        }
        InitCallbacks();

        MGM.Play();
        UI.ShowMiniGameMode(true);
        GameStarted = true;
    }

    void StopGame()
    {
        GameStarted = false;
        MGM.Stop();
        RemoveCallbacks();
        UI.ShowMiniGameMode(false);
    }

    void RestartGame()
    {
        // Remove callbacks pointing obsolete methods
        RemoveCallbacks();

        // Reset Player data
        playerData = new PlayerData();

        // Mini Game Reset
        MGM.Init(this);
        MGM.ResetLoop();

        /// playground reset mat
        PG.RefreshMatFromDiff(MGM.MGLoop.rank);
        PG.RefreshMatFromLoopLevel(MGM.MGLoop.depth);
        PG.ResetAnimation();
        PG.FinalClapOpen();

        // UI Reset
        UI.RefreshLoopLevelText(MGM.MGLoop.GetRankStr());
        UI.ResetLoopStage();

        // Start game again
        StartGame();
    }

    public void OnMiniGameCompletion()
    {
        // UI feedback
        UI.RefreshLoopStage(MGM.MGLoop.index, MGM.MGLoop.Current.successState);
    }

    public async void OnMiniGameTransition()
    {
        // nothing ?
    }

    void OnLoopDepthUpdate()
    {
        playerData.loopHistory.AddSnapshot(MGM.MGLoop);

        bool loopSuccess = MGM.MGLoop.IsLoopPassed();

        // Animate according to LoopSuccess
        if (loopSuccess)
        {
            PG.RefreshMatFromDiff(MGM.MGLoop.rank);
            UI.RefreshLoopLevelText(MGM.MGLoop.GetRankStr());
        }

        PG.RefreshMatFromLoopLevel(MGM.MGLoop.depth);
        UI.ResetLoopStage();

        AnimationCurve timeScaleCurve = GameData.Get.gameSettings.timeScaleOverLoopLevel;
        if (MGM.MGLoop.depth > timeScaleCurve.keys[timeScaleCurve.length - 1].time)
            return;
        playerData.timeScale = timeScaleCurve.Evaluate(MGM.MGLoop.depth);
        Time.timeScale = playerData.timeScale;
    }

    void GameOver()
    {
        Time.timeScale = 1f;
        playerData.loopHistory.AddSnapshot(MGM.MGLoop);

        StopGame();

        CancellationTokenSource cts = new CancellationTokenSource();

        inst_UIGameOver = GOBuilder.Create(prefab_UIGameOver).BuildAs<UIGameOver>();
        inst_UIGameOver.TryAgainBtn.onClick.AddListener(() => { cts.Cancel(); RestartGame(); });
        inst_UIGameOver.MenuBtn.onClick.AddListener(() => { cts.Cancel(); ExitToTitle(); });

        bool IsHighScore = PostGameScoreProcessing();

        inst_UIGameOver.scoreDisplayValue.text = playerData.score.ToString();

        inst_UIGameOver.Animate(IsHighScore, PG, cts.Token);
    }

    void RefreshUI()
    {
        UI.miniGameClock.text = Mathf.Ceil(MGM.gameClock.GetRemainingTime()).ToString("#0");
        UI.hpClock.text = playerData.HP.ToString("#0.0");
        //UI.score.text = playerData.score.ToString();
    }

    void Update()
    {
        // if (Input.GetKey(KeyCode.Escape))
        //     Application.Quit();

        if (!GameStarted)
            return;

        // GameLoop
        RefreshUI();

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

        int loopSize = GameData.GetSettings.loopSize;
        byte[] gameIDs = new byte[loopSize];
        for (int i = 0; i < loopSize; i++)
        {
            gameIDs[i] = MGM.MGLoop.At(i).descriptor.ID;
        }
        LoopHighScore lhs = new LoopHighScore(GameData.Get.currentGameMode, gameIDs, score, DateTime.Now);
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
