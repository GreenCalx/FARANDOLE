using UnityEngine;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class SignInService : MonoBehaviour
{
    public UnityEvent OnSignIn;

    // TODO : bad security maybe, just for quick testing atm.
    bool signedIn = false;
    bool retry = false;
    bool kill = false;

    public void Start()
    {
        signedIn = false;
        retry = true;
        //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        WaitSignIn();
    }

    void Destroy()
    {
        kill = true;
    }

    public void OfflineMode()
    {
        signedIn = true;
    }

    async void WaitSignIn()
    {
        PlayGamesPlatform.Activate();
        await SignIn();
        OnSignIn.Invoke();
    }
    async Task SignIn()
    {
        while (!signedIn && !kill)
        {
            if (retry)
            {
                PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
            }
            await Task.Delay(500); // half a sec wait time inbetween attempts
        }
        retry = false;
        return;
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            Debug.Log("signed in !");

            signedIn = true;
            retry = false;
        }
        else
        {
            Debug.Log("Failed to sign in");
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            signedIn = false;
            retry = true;
        }
    }
    

}
