using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using Cysharp.Threading.Tasks;

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
    [Header("Transition")]
    public Image transitionImage;
    [Header("Audio")]
    public FMODUnity.StudioEventEmitter bgmEmitter;
    public float intensityGainTimeStep = 10f;
    readonly string IntensityBGMParm = "Intensity";
    readonly string ExitMenuParm = "State";
    float elapsedTime;
    readonly string GameScene = "Game";

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

        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (bgmEmitter)
        {
            int intensity = (int)Mathf.Floor(elapsedTime/intensityGainTimeStep);
            bgmEmitter.SetParameter(IntensityBGMParm, intensity);
            //Debug.Log(intensity);
        }
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
        bgmEmitter.SetParameter(ExitMenuParm, 1);
        DelayedLaunch(GAME_MODE.RANDOM_SEED);
    }
    void StartDailySeed()
    {
        DelayedLaunch(GAME_MODE.DAILY_SEED);
    }
    void StartSprint()
    {
        DelayedLaunch(GAME_MODE.SPRINT);
    }

    async UniTaskVoid DelayedLaunch(GAME_MODE iGameMode)
    {
        bgmEmitter.SetParameter(ExitMenuParm, 1);
        await Transition();
        GameData.Get.PickGameMode(iGameMode);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }

    async UniTask Transition()
    {
        float startTime = Time.time;
        float frac = 0f;
        Color c = transitionImage.color;
        while (frac < 1f)
        {
            frac = Mathf.Clamp01((Time.time - startTime) / GameData.GetSettings.titleScreenFadeoutTime);    
            c.a = frac;
            transitionImage.color = c;
            await UniTask.Yield();
        }
    }

    void QuitGame()
    {
        if (Application.isPlaying)
            Application.Quit();
    }
}
