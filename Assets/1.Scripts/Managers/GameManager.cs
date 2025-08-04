using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        MGM.Init(this);
        MGM.WarmUpMiniGames();
        while (!MGM.IsReady())
        { yield return null; }

        GSM.Init(this);
        while (!GSM.IsReady())
        { yield return null; }

        UI.Init();

        StartGame();
    }

    void InitCallbacks()
    {
        MGM.OnHPLossCB.AddListener(playerData.LoseHP);
        MGM.OnScoreGainCB.AddListener(playerData.AddScore);
        MGM.OnLoopComplete.AddListener(LevelUp);
    }

    void RemoveCallbacks()
    {
        MGM.OnHPLossCB.RemoveListener(playerData.LoseHP);
        MGM.OnScoreGainCB.RemoveListener(playerData.AddScore);
        MGM.OnLoopComplete.RemoveListener(LevelUp);
    }

    void StartGame()
    {
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

        /// playground reset mat
        PG.RefreshMatFromDiff(MGM.miniGamesDifficulty);
        PG.RefreshMatFromLoopLevel(playerData.loopLevel);

        // Start game again
        StartGame();
    }

    void LevelUp()
    {
        if (++playerData.loopLevel % GameData.Get.gameSettings.miniGameLevelUpThreshold == 0)
        {
            MGM.RaiseDifficulty();
            PG.RefreshMatFromDiff(MGM.miniGamesDifficulty);
        }
        PG.RefreshMatFromLoopLevel(playerData.loopLevel);

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
        inst_UIGameOver.scoreLbl.text = playerData.score.ToString();
        inst_UIGameOver.TryAgainBtn.onClick.AddListener(() => RestartGame());
    }

    void RefreshUI()
    {
        UI.miniGameClock.text    = MGM.GetRemainingTime().ToString("#0.0");
        UI.hpClock.text          = playerData.HP.ToString("#0.0");
        UI.score.text            = playerData.score.ToString();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        if (!GameStarted)
            return;

        // GameLoop
        RefreshUI();

        if (playerData.HP <= 0)
        {
            GameOver();
        }
    }
}
