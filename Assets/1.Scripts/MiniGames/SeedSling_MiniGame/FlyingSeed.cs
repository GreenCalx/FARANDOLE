using UnityEngine;
using System;

public class FlyingSeed : MonoBehaviour
{
    public SpriteRenderer SR;

    PerspectiveRoom room;
    LaunchParams lp;
    SlingConfig cfg;
    float duration;
    float launchScale;   // world scale of the seed at release (matches the pouch seed -> no pop)
    float farScaleRatio; // fraction of launchScale the seed reaches at the far plane (sense of distance)
    ProjectileType type;
    float t;
    bool flying;

    public event Action<Vector2, ProjectileType> OnLanded;

    // iLaunchScale: the loaded seed's actual world scale at the moment of fire. The seed spawns at
    // this size (seamless hand-off from the slingshot pouch) then shrinks down toward iFarScaleRatio
    // of it as it travels into the room, selling the sense of distance. The shrink tracks the seed's
    // current ground depth, so a short throw barely shrinks and a deep throw shrinks the most.
    public void Launch(PerspectiveRoom iRoom, LaunchParams iLp, SlingConfig iCfg,
                       float flightDuration, float iLaunchScale, float iFarScaleRatio, ProjectileType iType)
    {
        room = iRoom; lp = iLp; cfg = iCfg; duration = Mathf.Max(0.01f, flightDuration);
        launchScale = iLaunchScale; farScaleRatio = Mathf.Clamp01(iFarScaleRatio);
        type = iType; t = 0f; flying = true;
        Apply(0f);
    }

    void Update()
    {
        if (!flying) return;
        t += Time.deltaTime / duration;
        if (t >= 1f) { t = 1f; Apply(t); Land(); return; }
        Apply(t);
    }

    void Apply(float ct)
    {
        ArcSample s = SeedArc.Sample(lp, cfg, ct);
        Vector3 ground = room.GroundPointAt(s.groundFrac.x, s.groundFrac.y);
        Vector3 pos = ground + Vector3.up * s.height;
        pos.z = transform.position.z; // keep the LM2D foreground z assigned after spawn
        transform.position = pos;
        // Depth-driven shrink: full launch size at the near plane (groundFrac.y == 0, so t==0 keeps
        // the exact launch size -> no pop), easing down to farScaleRatio of it at the far plane.
        float depthShrink = Mathf.Lerp(1f, farScaleRatio, Mathf.Clamp01(s.groundFrac.y));
        transform.localScale = Vector3.one * launchScale * depthShrink;
    }

    void Land()
    {
        flying = false;
        OnLanded?.Invoke(new Vector2(lp.lateralFrac, lp.depthFrac), type);
        Destroy(gameObject);
    }
}
