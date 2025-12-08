using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
public class SignInService : MonoBehaviour
{
    public Button SignInBtn;
    public Button OfflineBtn;
    public TextMeshProUGUI connectionText;

    async void Start()
    {
#if UNITY_ANDROID
        SignInBtn.onClick.AddListener(() => GoogleLogin().Forget());
#else // TODO iOS
        SignInBtn.onClick.AddListener(() => AnonymousLogin().Forget());
#endif
        OfflineBtn.onClick.AddListener(() => OfflineMode());

        await UGSAuthenticationManager.InitializeAndSignIn();

        connectionText.text = "Anonymous login OK";
    }

    //--------------------------------------
    // Anonymous SIGN-IN
    //--------------------------------------
    public async UniTaskVoid AnonymousLogin()
    {
#if UNITY_ANDROID
        
#endif
        connectionText.text = "Guest Session OK";
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

    connectionText.text = "Google Play Sign in OK";
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
