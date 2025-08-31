using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UITitle : MonoBehaviour
{
    public RectTransform handle_titleBtns;
    public RectTransform handle_gameModesBtns;
    public RectTransform handle_UIHighScores;
    public RectTransform handle_UISettings;
    public RectTransform handle_backBtn;
    public RectTransform handle_quitBtn;
    [Header("Title base buttons")]
    public UIButton playBtn;
    public UIButton highScoresBtn;
    public UIButton settingsBtn;
    public UIButton backBtn;
    public UIButton quitBtn;
    [Header("Game Modes Buttons")]
    public UIButton randomSeedBtn;
    public UIButton dailySeedBtn;
    public UIButton sprintBtn;
    [Header("High Scores UI")]

    [Header("Others")]
    public string GameScene = "Game";

    public void DisableAll()
    {
        handle_titleBtns.gameObject.SetActive(false);
        handle_gameModesBtns.gameObject.SetActive(false);
        handle_UIHighScores.gameObject.SetActive(false);
        handle_backBtn.gameObject.SetActive(false);

        handle_quitBtn.gameObject.SetActive(true);
    }

    public void EnableHome()
    {
        handle_titleBtns.gameObject.SetActive(true);
        handle_gameModesBtns.gameObject.SetActive(false);
        handle_UIHighScores.gameObject.SetActive(false);
        handle_backBtn.gameObject.SetActive(false);

        handle_quitBtn.gameObject.SetActive(true);
    }

    void Start()
    {
        playBtn?.clickCallback.AddListener(() => ShowGameModes());
        highScoresBtn?.clickCallback.AddListener(() => ShowHighScores());
        settingsBtn?.clickCallback.AddListener(() => ShowSettings());

        randomSeedBtn?.clickCallback.AddListener(() => StartRandomSeed());
        dailySeedBtn?.clickCallback.AddListener(() => StartDailySeed());
        sprintBtn?.clickCallback.AddListener(() => StartSprint());

        backBtn?.clickCallback.AddListener(() => BackToTitle());
        quitBtn?.clickCallback.AddListener(() => QuitGame());

        DisableAll();
    }

    void ShowGameModes()
    {
        handle_titleBtns.gameObject.SetActive(false);
        handle_gameModesBtns.gameObject.SetActive(true);
        handle_backBtn.gameObject.SetActive(true);
        handle_quitBtn.gameObject.SetActive(false);
    }
    void ShowHighScores()
    {
        handle_UIHighScores.gameObject.SetActive(true);
        handle_backBtn.gameObject.SetActive(true);
        handle_quitBtn.gameObject.SetActive(false);
        handle_titleBtns.gameObject.SetActive(false);
    }
    void ShowSettings()
    {
        handle_backBtn.gameObject.SetActive(true);
        handle_UISettings.gameObject.SetActive(true);
        handle_quitBtn.gameObject.SetActive(false);
        handle_titleBtns.gameObject.SetActive(false);
    }

    void BackToTitle()
    {
        handle_titleBtns.gameObject.SetActive(true);
        handle_UIHighScores.gameObject.SetActive(false);
        handle_gameModesBtns.gameObject.SetActive(false);
        handle_backBtn.gameObject.SetActive(false);
        handle_UISettings.gameObject.SetActive(false);

        handle_quitBtn.gameObject.SetActive(true);
    }
    void StartRandomSeed()
    {
        GameData.Get.PickGameMode(GAME_MODE.RANDOM_SEED);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }
    void StartDailySeed()
    {
        GameData.Get.PickGameMode(GAME_MODE.DAILY_SEED);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }
    void StartSprint()
    {
        GameData.Get.PickGameMode(GAME_MODE.SPRINT);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }

    void QuitGame()
    {
        if (Application.isPlaying)
            Application.Quit();
    }
}
