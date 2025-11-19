using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class UITitle : UINavigator
{
    [Header("UITitle")]
    public UIPopupPanel handle_PopUpPanel;
    public UIPanel handle_GameModePanel;
    public UIPanel handle_UIHighScores;
    public UIPanel handle_UISettings;
    public UIPanel handle_UISingles;
    [Header("Title base buttons")]
    public UIButton playBtn;
    public UIButton highScoresBtn;
    public UIButton settingsBtn;
    public UIButton backBtn;
    public UIButton quitBtn;
    [Header("Game Modes Buttons")]
    public UIButton m_MutationMode;
    public UIButton m_SinglesMode;
    public UIButton m_DailySeedMode;
    public UIButton m_CustomMode;
    [Header("Launch Modes Button")]
    public UIButton m_LaunchSinglesMode;
    public UIMiniGameSelectionGrid m_LaunchSinglesGame;

    [Header("Transition")]
    public Image transitionImage;
    [Header("Audio")]
    public FMODUnity.StudioEventEmitter bgmEmitter;
    public float intensityGainTimeStep = 10f;
    readonly string IntensityBGMParm = "Intensity";
    readonly string ExitMenuParm = "State";
    float elapsedTime;
    [Header("Animations")]
    public TitleAnim titleAnimations;
    //
    readonly string GameScene = "Game";

    void Start()
    {
        base.Setup(m_Home);

        playBtn?.clickCallback.AddListener(() => base.NavigateTo(handle_GameModePanel));
        highScoresBtn?.clickCallback.AddListener(() => base.NavigateTo(handle_UIHighScores));
        settingsBtn?.clickCallback.AddListener(() => base.NavigateTo(handle_UISettings));
        m_SinglesMode?.clickCallback.AddListener(() => base.NavigateTo(handle_UISingles));

        m_MutationMode?.clickCallback.AddListener(() => StartMutationMode());
        m_DailySeedMode?.clickCallback.AddListener(() => StartDailySeed());
        m_CustomMode?.clickCallback.AddListener(() => StartCustom());
        m_LaunchSinglesMode?.clickCallback.AddListener(() => StartSingles(m_LaunchSinglesGame.selectedGame.ID));

        backBtn?.clickCallback.AddListener(() => base.OnBack());
        quitBtn?.clickCallback.AddListener(() => QuitGame());

        elapsedTime = 0f;
        handle_PopUpPanel.SignInPopup();
        NavigateTo(handle_PopUpPanel);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (bgmEmitter)
        {
            int intensity = (int)Mathf.Floor(elapsedTime/intensityGainTimeStep);
            bgmEmitter.SetParameter(IntensityBGMParm, intensity);
        }
    }

    void StartMutationMode()
    {
        DelayedLaunch(GAME_MODE.MUTATION);
    }
    void StartDailySeed()
    {
        DelayedLaunch(GAME_MODE.DAILY_SEED);
    }
    void StartCustom()
    {
        DelayedLaunch(GAME_MODE.CUSTOM);
    }

    void StartSingles(int iMiniGameIndex)
    {
        GameData.Get.MiniGameSeeds = new List<int>(1);
        GameData.Get.MiniGameSeeds.Add(iMiniGameIndex);

        DelayedLaunch(GAME_MODE.SINGLES);
    }

    async UniTaskVoid DelayedLaunch(GAME_MODE iGameMode)
    {
        bgmEmitter.SetParameter(ExitMenuParm, 1);
        titleAnimations.ExitAnim();
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
