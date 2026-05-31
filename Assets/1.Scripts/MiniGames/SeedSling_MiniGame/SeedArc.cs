using UnityEngine;

[System.Serializable]
public struct SlingConfig
{
    public float maxPull;        // drag magnitude (world units) for full power
    public float minDepthFrac;   // nearest landing depth at zero power
    public float maxLateralPull; // drag.x (world units) mapping to a room edge
    public float arcHeight;      // world-unit peak of the visual hop
}

public struct LaunchParams
{
    public float lateralFrac; // 0..1 landing x across the room width
    public float depthFrac;   // 0..1 landing depth (near..far)
}

public struct ArcSample
{
    public Vector2 groundFrac; // (xFrac, depthFrac) on the ground plane
    public float height;       // world-unit hop above the ground
}

public static class SeedArc
{
    // Launch is opposite the pull: pull down/left -> shot far/right.
    public static LaunchParams FromPull(Vector2 pull, SlingConfig cfg)
    {
        float power = cfg.maxPull <= 0f ? 0f : Mathf.Clamp01(pull.magnitude / cfg.maxPull);
        float depthFrac = Mathf.Lerp(cfg.minDepthFrac, 1f, power);

        float lateral = cfg.maxLateralPull <= 0f
            ? 0f
            : Mathf.Clamp(-pull.x / cfg.maxLateralPull, -1f, 1f);
        float lateralFrac = Mathf.Clamp01(0.5f + lateral * 0.5f);

        return new LaunchParams { lateralFrac = lateralFrac, depthFrac = depthFrac };
    }

    public static ArcSample Sample(LaunchParams lp, SlingConfig cfg, float t)
    {
        t = Mathf.Clamp01(t);
        Vector2 launch = new Vector2(0.5f, 0f);
        Vector2 landing = new Vector2(lp.lateralFrac, lp.depthFrac);
        return new ArcSample
        {
            groundFrac = Vector2.Lerp(launch, landing, t),
            height = cfg.arcHeight * 4f * t * (1f - t)
        };
    }

    public static bool IsInPot(Vector2 landingFrac, Vector2 potFrac, float mouthRadiusFrac)
        => (landingFrac - potFrac).magnitude <= mouthRadiusFrac;
}
