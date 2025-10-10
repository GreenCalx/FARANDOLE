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

    // TODO : bad security maybe, just for quick testing atm.
    bool signedIn = false;
    bool authentificationProcessed = false;
    bool retry = false;
    bool kill = false;

    public void Start()
    {
        signedIn = false;
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
        signedIn = true;
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
        while (!signedIn && !kill)
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
            //Debug.Log("signed in !");
            string userName = "foo";
            connectionText.text = "Signed in as " + userName;

            signedIn = true;
            //retry = false;
        }
        else
        {
            connectionText.text = "Failed to sign in";
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            signedIn = false;
            //retry = true;
        }
        authentificationProcessed = true;
    }
    
}
