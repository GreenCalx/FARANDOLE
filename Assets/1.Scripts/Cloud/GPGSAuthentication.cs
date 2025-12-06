#if UNITY_ANDROID
using System.Threading.Tasks;
using UnityEngine;
using GooglePlayGames;

public static class GPGSAuthentication
{
    public static Task<string> SignInAndGetIdToken()
    {
        var tcs = new TaskCompletionSource<string>();

        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate(success =>
        {
            if (!success)
            {
                Debug.LogError("[GPGS] Authentication failed");
                tcs.SetResult(null);
                return;
            }

            string idToken = PlayGamesPlatform.Instance.GetUserId();

            if (string.IsNullOrEmpty(idToken))
            {
                Debug.LogError("[GPGS] ID Token is empty");
                tcs.SetResult(null);
                return;
            }

            Debug.Log("[GPGS] ID Token received");
            tcs.SetResult(idToken);
        });

        return tcs.Task;
    }

    public static string GetDisplayName()
    {
        return Social.localUser.userName;
    }
}
#endif
