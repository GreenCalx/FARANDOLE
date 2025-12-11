using UnityEngine;

public static class SessionData
{
    static readonly string DEFAULT_USERNAME = "Guest";
    public static bool IsOnline = false;
    public static bool IsOffline = false;
    public static bool IsPremium => IAPManager.Get ? IAPManager.Get.CanAccessPremiumContent()  :false;
    public static string UserName => UGSAuthenticationManager.PlayerName=="" ? 
        DEFAULT_USERNAME : 
        UGSAuthenticationManager.PlayerName;
    public static string UserID => UGSAuthenticationManager.PlayerId;
    public static int MasterPoints = 0;
    public static int DivinePoints = 0;
}