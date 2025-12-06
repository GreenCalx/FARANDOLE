using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
public class SignInService : MonoBehaviour
{
    public Button GoogleBtn;
    public Button OfflineBtn;
    public TextMeshProUGUI connectionText;

    async void Start()
    {
#if UNITY_ANDROID
        GoogleBtn.onClick.AddListener(() => GoogleLogin().Forget());
#else // TODO iOS
        GoogleBtn.onClick.AddListener(() => AnonymousLogin().Forget());
#endif
        OfflineBtn.onClick.AddListener(() => OfflineMode());

        await UGSAuthenticationManager.InitializeAndSignIn();

        SessionData.UserName = AuthenticationService.Instance.PlayerId;
        // connectionText.text =
        //     $"Player ID:\n{AuthenticationService.Instance.PlayerId}\n\n" +
        //     $"Anonymous: {AuthenticationService.Instance.PlayerInfo?.CreatedAt}";

        connectionText.text = "Anonymous login OK";
    }

    //--------------------------------------
    // Anonymous SIGN-IN
    //--------------------------------------
    public async UniTaskVoid AnonymousLogin()
    {
#if UNITY_ANDROID
        

#endif
    }


    //--------------------------------------
    // GOOGLE SIGN-IN
    //--------------------------------------
public async UniTaskVoid GoogleLogin()
{
#if UNITY_ANDROID
    string idToken = await GPGSAuthentication.SignInAndGetIdToken();

    if (string.IsNullOrEmpty(idToken))
    {
        connectionText.text = "Google sign-in failed";
        return;
    }

    await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);

    SessionData.UserName = GPGSAuthentication.GetDisplayName();
    connectionText.text = SessionData.UserName;
#endif
}


    //--------------------------------------
    // OFFLINE
    //--------------------------------------
    void OfflineMode()
    {
        SessionData.IsOffline = true;
        SessionData.IsOnline = false;
        connectionText.text = "Offline mode";
    }
}
