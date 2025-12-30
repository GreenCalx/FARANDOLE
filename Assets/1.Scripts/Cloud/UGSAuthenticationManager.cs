using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public static class UGSAuthenticationManager
{
    public static string PlayerId => AuthenticationService.Instance.PlayerId;
    public static string PlayerName => AuthenticationService.Instance.PlayerName;

    public static bool IsSignedInUGS()
     {
        try{
            return AuthenticationService.Instance.IsSignedIn;
        } catch (ServicesInitializationException exc)
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
            Debug.Log("[UGS] Anonymous login OK: " + PlayerName);
        }
    #if UNITY_ANDROID
        await LinkGoogleAsync();
    #endif
        
    }

    public static async Task LinkGoogleAsync()
    {
#if UNITY_ANDROID
        string idToken = await GPGSAuthentication.SignInAndGetIdToken();

        if (string.IsNullOrEmpty(idToken))
            return;

        await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);
        Debug.Log("[UGS] Google linked");
#endif
    }
}
