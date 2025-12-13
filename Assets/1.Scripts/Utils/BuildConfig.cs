using UnityEngine;

public static class BuildConfig
{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public const bool AllowPremiumOverride = true;
#else
    public const bool AllowPremiumOverride = false;
#endif
}