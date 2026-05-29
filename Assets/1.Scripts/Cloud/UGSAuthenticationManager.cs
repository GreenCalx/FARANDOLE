using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public static class UGSAuthenticationManager
{
    public static string PlayerId
    {
        get
        {
            try { return AuthenticationService.Instance.PlayerId; }
            catch (ServicesInitializationException) { return string.Empty; }
        }
    }

    public static string PlayerName
    {
        get
        {
            try { return AuthenticationService.Instance.PlayerName ?? string.Empty; }
            catch (ServicesInitializationException) { return string.Empty; }
        }
    }

    public static bool IsSignedInUGS()
    {
        try
        {
            return AuthenticationService.Instance.IsSignedIn;
        }
        catch (ServicesInitializationException)
        {
            return false;
        }
    }

    public static async Task InitializeAndSignIn()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("[UGS] Anonymous login OK");
        }
#if UNITY_ANDROID
        await LinkGoogleAsync();
#endif
    }

    // Links the current UGS account to Google Play Games (GPGS v2) and syncs the
    // GPGS display name to UGS PlayerName so leaderboards and profile card agree.
    public static async Task LinkGoogleAsync()
    {
#if UNITY_ANDROID
        string authCode = await GPGSAuthentication.SignInAndGetAuthCode();

        if (string.IsNullOrEmpty(authCode))
            return;

        try
        {
            await AuthenticationService.Instance.LinkWithGooglePlayGamesAsync(authCode);
            Debug.Log("[UGS] Google Play Games linked");
        }
        catch (AuthenticationException e) when (e.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.Log("[UGS] Account already linked to Google Play Games.");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[UGS] Google Play Games link failed: {e.Message}");
            return;
        }

        // Sync the GPGS display name → UGS PlayerName so both leaderboard and
        // profile card show the player's real Google identity.
        string displayName = GPGSAuthentication.GetDisplayName();
        if (!string.IsNullOrEmpty(displayName))
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(displayName);
                Debug.Log($"[UGS] PlayerName updated to: {displayName}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UGS] Failed to update PlayerName: {e.Message}");
            }
        }
#endif
    }
}
