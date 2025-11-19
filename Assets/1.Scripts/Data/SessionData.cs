using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public static class SessionData
{
    public static bool IsOnline = false;
    public static bool IsPremium => IAPManager.Get ? IAPManager.Get.CheckPremiumPurchased()  :false;
    public static string UserName = "Guest";
    public static PlayGamesLocalUser LocalUser;
}