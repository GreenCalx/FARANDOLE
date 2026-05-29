#if UNITY_ANDROID
using System.Threading.Tasks;
using UnityEngine;
using GooglePlayGames;

public static class GPGSAuthentication
{
    // Returns a server auth code suitable for UGS LinkWithGooglePlayGamesAsync.
    // GPGS v2 requires RequestServerSideAccess — GetUserId() returns a player ID string, not a token.
    public static Task<string> SignInAndGetAuthCode()
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

            PlayGamesPlatform.Instance.RequestServerSideAccess(
                forceRefreshToken: false,
                callback: authCode =>
                {
                    if (string.IsNullOrEmpty(authCode))
                    {
                        Debug.LogError("[GPGS] Server auth code is empty");
                        tcs.SetResult(null);
                        return;
                    }
                    Debug.Log("[GPGS] Server auth code received");
                    tcs.SetResult(authCode);
                }
            );
        });

        return tcs.Task;
    }

    public static string GetDisplayName()
    {
        return Social.localUser.userName;
    }
}
#endif
