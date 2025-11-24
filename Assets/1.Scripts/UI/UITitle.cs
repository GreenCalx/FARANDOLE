using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class UITitle : UINavigator
{
    [Header("UITitle")]
    public UIPopupPanel handle_PopUpPanel;
    public UIPanel handle_GameModePanel;
    public UIPanel handle_UIHighScores;
    public UIPanel handle_UISettings;
    public UIPanel handle_UISingles;
    public UIPlayerProfileCard handle_profilePanel;
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
    public UILoadingTransition h_UISceneTransition;
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
        elapsedTime = 0f;
        
        playBtn?.clickCallback.AddListener(() => base.NavigateTo(handle_GameModePanel));
        highScoresBtn?.clickCallback.AddListener(() => base.NavigateTo(handle_UIHighScores));
        settingsBtn?.clickCallback.AddListener(() => base.NavigateTo(handle_UISettings));
        m_SinglesMode?.clickCallback.AddListener(() => base.NavigateTo(handle_UISingles));
        handle_profilePanel.m_ProfileCardButton?.clickCallback.AddListener(()=>
        {
            if (handle_PopUpPanel.PopupActive)
                return;
            handle_PopUpPanel.PremiumPopup();
            base.NavigateTo(handle_PopUpPanel);
        });

        m_MutationMode?.clickCallback.AddListener(() => StartMutationMode());
        m_DailySeedMode?.clickCallback.AddListener(() => StartDailySeed());
        m_CustomMode?.clickCallback.AddListener(() => StartCustom());
        m_LaunchSinglesMode?.onClick.AddListener(() => StartSingles(m_LaunchSinglesGame.selectedGame.ID));

        
        handle_PopUpPanel.NavigateBackCB.AddListener(()=> base.OnBack());

        backBtn?.clickCallback.AddListener(() => base.OnBack());
        quitBtn?.clickCallback.AddListener(() => QuitGame());

        // Start with sign in pop up active
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
        GameData.Get.NewGameSeed();
        GameData.Get.AddGameSeed(iMiniGameIndex);
        DelayedLaunch(GAME_MODE.SINGLES);
    }

    async UniTaskVoid DelayedLaunch(GAME_MODE iGameMode)
    {
        bgmEmitter.SetParameter(ExitMenuParm, 1);
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        await UniTask.WhenAll(
            h_UISceneTransition.FadeIn(GameData.GetSettings.titleScreenFadeoutTime),
            titleAnimations.ExitAnim()
        );
        //titleAnimations.CancelAnimation();
        GameData.Get.PickGameMode(iGameMode);
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }

    void QuitGame()
    {
        if (Application.isPlaying)
            Application.Quit();
    }
}
