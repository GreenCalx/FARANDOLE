using UnityEngine;

public static class SessionData
{
    public static bool IsOnline = false;
    public static bool IsOffline = false;
    public static bool IsPremium => IAPManager.Get ? IAPManager.Get.CheckPremiumPurchased()  :false;
    public static string UserName = "Guest";
    public static int MasterPoints = 0;
    public static int DivinePoints = 0;
}