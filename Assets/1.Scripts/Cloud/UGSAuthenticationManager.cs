using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class UGSAuthenticationManager : MonoBehaviour
{
    public static string PlayerId => AuthenticationService.Instance.PlayerId;

    async void Awake()
    {
        await InitializeAndSignIn();
    }

    public static async Task InitializeAndSignIn()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("[UGS] Anonymous login OK: " + PlayerId);
        }
    }

    public async Task LinkGoogleAsync()
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
