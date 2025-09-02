using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool GameStarted = false;
    [Header("Managers")]
    public MiniGameManager MGM;
    public GameSceneManager GSM;
    public LayerManager2D LM2D;
    public PlaygroundManager PG;

    [Header("Extra Refs")]
    public PlayerController PC;
    public UIGame UI;
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

        UI.Init();
        UI.RefreshLoopLevelText(MGM.MGLoop.GetRankStr());
        UI.loopPresentationAnim.Show(MGM.MGLoop);

        //StartGame();
        UI.launchGameBtn?.clickCallback.AddListener(() => StartGame());
    }

    void InitCallbacks()
    {
        MGM.OnHPLossCB.AddListener(playerData.LoseHP);
        MGM.OnScoreGainCB.AddListener(playerData.AddScore);
        MGM.OnLoopComplete.AddListener(OnLoopCompletion);
        MGM.OnMiniGameComplete.AddListener(OnMiniGameCompletion);
        MGM.OnMiniGameTransitionCB.AddListener(OnMiniGameTransition);
        MGM.ShowPostGameUICB.AddListener(UI.ShowSuccessArea);
    }

    void RemoveCallbacks()
    {
        MGM.OnHPLossCB.RemoveListener(playerData.LoseHP);
        MGM.OnScoreGainCB.RemoveListener(playerData.AddScore);
        MGM.OnLoopComplete.RemoveListener(OnLoopCompletion);
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
        PG.RefreshMatFromLoopLevel(playerData.loopLevel);
        PG.ResetAnimation();

        // UI Reset
        UI.RefreshLoopLevelText(MGM.MGLoop.GetRankStr());

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

    void OnLoopCompletion()
    {
        playerData.loopLevel++;

        bool loopSuccess = MGM.MGLoop.IsLoopPassed();

        // Animate according to LoopSuccess
        if (loopSuccess)
        {
            PG.RefreshMatFromDiff(MGM.MGLoop.rank);
            UI.RefreshLoopLevelText(MGM.MGLoop.GetRankStr());
        }
        
        PG.RefreshMatFromLoopLevel(playerData.loopLevel);
        UI.ResetLoopStage();

        AnimationCurve timeScaleCurve = GameData.Get.gameSettings.timeScaleOverLoopLevel;
        if (playerData.loopLevel > timeScaleCurve.keys[timeScaleCurve.length - 1].time)
            return;
        playerData.timeScale = timeScaleCurve.Evaluate(playerData.loopLevel);
        Time.timeScale = playerData.timeScale;
    }

    void GameOver()
    {
        Time.timeScale = 1f;

        StopGame();
        inst_UIGameOver = Instantiate(prefab_UIGameOver).GetComponent<UIGameOver>();
        inst_UIGameOver.TryAgainBtn.onClick.AddListener(() => RestartGame());
        inst_UIGameOver.MenuBtn.onClick.AddListener(() => ExitToTitle());

        PostGameScoreProcessing();
    }

    void RefreshUI()
    {
        UI.miniGameClock.text = MGM.gameClock.GetRemainingTime().ToString("#0.0");
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

    public void PostGameScoreProcessing()
    {
        LoopHighScore lhs = MGM.GetLoopHighScore();
        LoopHighScore replacedHS = null;
        if (UserData.IsNewHighScore(lhs, out replacedHS))
        {
            if (replacedHS != null)
                UserData.RemoveHighScore(replacedHS);
            UserData.AddHighScore(lhs);
            // <!> load all high score beforehand to avoid overwriting prev data
            UserData.SaveHighScores();

            inst_UIGameOver.newHighScoreDisplayValue.text = playerData.score.ToString();
            inst_UIGameOver.scoreDisplayHandle.gameObject.SetActive(false);
            inst_UIGameOver.newHighScoreDisplayHandle.gameObject.SetActive(true);
        }
        else
        {
            inst_UIGameOver.scoreDisplayValue.text = playerData.score.ToString();
            inst_UIGameOver.scoreDisplayHandle.gameObject.SetActive(true);
            inst_UIGameOver.newHighScoreDisplayHandle.gameObject.SetActive(false);
        }
    }
}
