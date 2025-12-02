using UnityEngine;
using UnityEngine.Events;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public class SignInService : MonoBehaviour
{
    public Button SignInBtn;
    public Button OfflineBtn;
    public TextMeshProUGUI connectionText;
    public UnityEvent OnSignIn;

    bool kill = false;

    void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = true;

        SignInBtn.onClick.AddListener(() => TryGoogleSignIn().Forget());
        OfflineBtn.onClick.AddListener(() => OfflineMode());

        // Auto start full sign-in flow
        FullSignInFlow().Forget();
    }

    void OnDestroy()
    {
        kill = true;
        SignInBtn.onClick.RemoveAllListeners();
        OfflineBtn.onClick.RemoveAllListeners();
    }

    //------------------------------------
    // SIGN IN MAIN
    //------------------------------------
    async UniTaskVoid FullSignInFlow()
    {
        // Firebase base identity (NO internet required)
        await FirebaseManager.WaitReady();
        await FirebaseManager.SignInAnonymous();
        await FirebaseManager.CreateUserIfMissing();

        // log profile metadata
        #if UNITY_ANDROID
        // Try auto GPGS login (non-blocking)
        await TryGoogleSignIn();
        #endif

        OnSignIn?.Invoke();
    }

    //------------------------------------
    // OFFLINE MODE
    //------------------------------------
    void OfflineMode()
    {
        connectionText.text = "Offline mode";
        SessionData.IsOnline = false;
        SessionData.IsOffline = true;
        OnSignIn?.Invoke();
    }

    //------------------------------------
    // GOOGLE SIGN-IN (PROFILE ONLY)
    //------------------------------------
    public async UniTask TryGoogleSignIn()
    {
        if (kill)
            return;

        var tcs = new UniTaskCompletionSource<bool>();

        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            tcs.TrySetResult(status == SignInStatus.Success);
        });

        bool success = await tcs.Task;

        if (!success)
        {
            connectionText.text = "Failed to connect to Google Play Games";
            SessionData.IsOnline = false;
            return;
        }

        // SUCCESS → extract profile
        var user = Social.localUser;
        if (user != null && user.authenticated)
        {
            SessionData.IsOnline = true;
            SessionData.UserName = user.userName;

            string googleId   = user.id;
            string googleName = user.userName;
            string avatarUrl  = user.image != null ? user.image.ToString() : "";

            connectionText.text = "Connected as " + googleName;

            // Store profile in Firestore
            await FirebaseManager.SaveGoogleProfile(googleId, googleName, avatarUrl);
        }
    }
}
