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
    [Header("Title base buttons")]
    public Button playBtn;
    public Button highScoresBtn;
    public Button settingsBtn;
    public Button backBtn;
    [Header("Game Modes Buttons")]
    public Button randomSeedBtn;
    public Button dailySeedBtn;
    public Button sprintBtn;
    [Header("High Scores UI")]
    
    [Header("Others")]
    public string GameScene = "Game";

    public void DisableAll()
    {
        handle_titleBtns.gameObject.SetActive(false);
        handle_gameModesBtns.gameObject.SetActive(false);
        handle_UIHighScores.gameObject.SetActive(false);
        handle_backBtn.gameObject.SetActive(false);
    }

    public void EnableHome()
    {
        handle_titleBtns.gameObject.SetActive(true);
        handle_gameModesBtns.gameObject.SetActive(false);
        handle_UIHighScores.gameObject.SetActive(false);
        handle_backBtn.gameObject.SetActive(false);
    }

    void Start()
    {
        playBtn?.onClick.AddListener(() => ShowGameModes());
        highScoresBtn?.onClick.AddListener(() => ShowHighScores());
        settingsBtn?.onClick.AddListener(() => ShowSettings());

        randomSeedBtn?.onClick.AddListener(() => StartRandomSeed());
        dailySeedBtn?.onClick.AddListener(() => StartDailySeed());
        sprintBtn?.onClick.AddListener(() => StartSprint());

        backBtn?.onClick.AddListener(() => BackToTitle());

        DisableAll();
    }

    void ShowGameModes()
    {
        handle_titleBtns.gameObject.SetActive(false);
        handle_gameModesBtns.gameObject.SetActive(true);
        handle_backBtn.gameObject.SetActive(true);
    }
    void ShowHighScores()
    {
        handle_UIHighScores.gameObject.SetActive(true);
        handle_backBtn.gameObject.SetActive(true);

    }
    void ShowSettings()
    {
        handle_backBtn.gameObject.SetActive(true);
        handle_UISettings.gameObject.SetActive(true);
    }

    void BackToTitle()
    {
        handle_titleBtns.gameObject.SetActive(true);
        handle_UIHighScores.gameObject.SetActive(false);
        handle_gameModesBtns.gameObject.SetActive(false);
        handle_backBtn.gameObject.SetActive(false);
        handle_UISettings.gameObject.SetActive(false);
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
}
