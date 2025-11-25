using UnityEngine;
using UnityEngine.Events;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using TMPro;

public class SignInService : MonoBehaviour
{
    public Button SignInBtn;
    public Button OfflineBtn;
    public TextMeshProUGUI connectionText;
    public UnityEvent OnSignIn;

    bool authentificationProcessed = false;
    bool userAuthentificationProcessed = false;
    bool retry = false;
    bool kill = false;

    PlayGamesLocalUser localUser;

    public void Start()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Destroy(gameObject);
        }

        SessionData.IsOnline = false;
        retry = true;

        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = true;

        authentificationProcessed = false;
        SignInBtn?.onClick.AddListener(() => TrySignIn());
        OfflineBtn?.onClick.AddListener(() => OfflineMode());

        WaitSignIn();
    }

    void OnDestroy()
    {
        SignInBtn?.onClick.RemoveListener(() => TrySignIn());
        OfflineBtn?.onClick.RemoveListener(() => OfflineMode());

        kill = true;
    }

    public void OfflineMode()
    {
        SessionData.IsOnline = true;
        IAPManager.Get.DebugPremiumUnlock();
    }

    public void TrySignIn()
    {
        authentificationProcessed = false;
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    async UniTaskVoid WaitSignIn()
    {
        await SignIn();
        OnSignIn.Invoke();
    }
    async UniTask SignIn()
    {
        while (!SessionData.IsOnline && !kill)
        {
            //     if (retry)
            //     {
            //         PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
            //     }
            //     await Task.Delay(500); // half a sec wait time inbetween attempts

            await Task.Yield();
        }
        // retry = false;
        return;
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            
            // TODO
            // When google cloud connection is available, fetch user name and
            // sign in used through ProcessUserAuthentication
            
            //SessionData.LocalUser = PlayGamesPlatform.Instance.localUser;
            //localUser.Authenticate(ProcessUserAuthentication);
            SessionData.IsOnline = true;
        }
        else
        {
            connectionText.text = "Failed to connect to google play services";
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            SessionData.IsOnline = false;
            //retry = true;
        }
        authentificationProcessed = true;
    }

    internal void ProcessUserAuthentication(bool status)
    {
        if (status)
        {
            SessionData.UserName = PlayGamesPlatform.Instance.localUser.userName;
            connectionText.text = "Signed in as " + SessionData.UserName;
            SessionData.IsOnline = true;
        }
        else
        {
            connectionText.text = "Failed to connect to authenticate user";
            SessionData.IsOnline = false;
        }
        userAuthentificationProcessed = true;
    }
    
}
