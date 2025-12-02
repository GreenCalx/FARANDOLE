using UnityEngine;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Cysharp.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance = null;
    public static FirebaseManager Instance => instance;

    public static FirebaseAuth Auth;
    public static FirebaseFirestore DB;
    public static bool IsReady = false;

    async void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        IsReady = false;
        instance = this;
        DontDestroyOnLoad(gameObject);

        var result = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (result == DependencyStatus.Available)
        {
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseFirestore.DefaultInstance;
            IsReady = true;
        }
        else
        {
            Debug.LogError("Firebase dependency error: " + result);
        }
    }

    public static async UniTask WaitReady()
    {
        while (!IsReady)
        {
            await UniTask.Yield();
        }
        return;
    }

    // Anonymous login (always works)
    public static async UniTask SignInAnonymous()
    {
        if (Auth.CurrentUser != null)
            return;

        await Auth.SignInAnonymouslyAsync();
    }

    // Creates user document in Firestore if new
    public static async UniTask CreateUserIfMissing()
    {
        string uid = Auth.CurrentUser.UserId;
        var doc = DB.Collection("users").Document(uid);
        var snap = await doc.GetSnapshotAsync();

        if (!snap.Exists)
        {
            await doc.SetAsync(new
            {
                createdAt = Timestamp.GetCurrentTimestamp(),
                username = "Player_" + UnityEngine.Random.Range(1000, 9999),
                masterPoints = 0,
                divinePoints = 0,
                platform = Application.platform.ToString(),
                googleId = "",
                googleName = "",
                googleAvatar = ""
            });

            Debug.Log("[Firestore] User created.");
        }
        else
        {
            Debug.Log("[Firestore] User already exists.");
        }
    }

    // Updates GPGS profile info on Firestore
    public static async UniTask SaveGoogleProfile(string googleId, string name, string avatarUrl)
    {
        string uid = Auth.CurrentUser.UserId;
        DocumentReference doc = DB.Collection("users").Document(uid);

        var data = new Dictionary<string, object>
        {
            { "googleId", googleId },
            { "googleName", name },
            { "googleAvatar", avatarUrl }
        };

        await doc.UpdateAsync(data);

        Debug.Log("[Firestore] Synced Google profile.");
    }

    public static async UniTask AddMasterPoints(int amount)
    {
        string uid = Auth.CurrentUser.UserId;
        var doc = DB.Collection("users").Document(uid);

        await doc.UpdateAsync("masterPoints", FieldValue.Increment(amount));
    }

    public static async UniTask AddDivinePoints(int amount)
    {
        string uid = Auth.CurrentUser.UserId;
        var doc = DB.Collection("users").Document(uid);

        await doc.UpdateAsync("divinePoints", FieldValue.Increment(amount));
    }
}
