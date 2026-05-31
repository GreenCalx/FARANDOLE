using UnityEngine;
using System;

public class Slingshot : MonoBehaviour, IPositionTracker
{
    [Header("Refs")]
    public SpriteRenderer bodySR;    // static slingshot body sprite (on root)
    public Transform pouch;          // loaded projectile holder; rests at the fork midpoint
    public SpriteRenderer loadedSR;  // shows currently-loaded projectile sprite
    public LineRenderer rubberband;  // pull visual (left fork tip -> pouch -> right fork tip)
    public Transform leftAnchor;     // left fork tip (set manually on the sprite)
    public Transform rightAnchor;    // right fork tip (set manually on the sprite)
    public SpriteRenderer reticleSR; // predicted landing marker

    [Header("Rubberband")]
    public int rubberbandPoints = 24; // segments for a smooth bend

    [Header("Sprites")]
    public Sprite almondSprite;
    public Sprite waterSprite;

    [Header("Projectile prefab")]
    public GameObject prefab_FlyingSeed; // FlyingSeed + SpriteRenderer

    [Header("Tuning")]
    public SlingConfig config = new SlingConfig {
        maxPull = 4f, minDepthFrac = 0.1f, maxLateralPull = 4f, arcHeight = 2f
    };
    public float flightDuration = 1f;
    public float pouchPullScale = 0.5f; // extra scale at full pull (Z- exaggeration)
    public float seedBaseScale = 1f;
    [Range(0.05f, 1f)] public float seedFarScaleRatio = 0.3f; // seed size at the far plane, as a fraction of launch size
    public float reticleFloorAngleOffsetDeg = 0f; // fine-tune on top of the room's computed floor angle

    PerspectiveRoom room;
    LayerManager2D lm2d;
    Func<ProjectileType> nextProjectileProvider;
    Action<Vector2, ProjectileType> onLandedToOrchestrator;
    float reticleAlpha = 1f;
    Vector3 anchor;
    Vector3 pouchRestScale = Vector3.one; // designed loaded-seed scale; pull exaggeration is relative to this
    Vector2 pull;
    bool aiming;
    ProjectileType loaded;

    public Vector2 position => new Vector2(transform.position.x, transform.position.y);

    public void Bind(PerspectiveRoom iRoom, Func<ProjectileType> iNext,
                     Action<Vector2, ProjectileType> iOnLanded, float iReticleAlpha,
                     LayerManager2D iLM2D)
    {
        room = iRoom;
        lm2d = iLM2D;
        nextProjectileProvider = iNext;
        onLandedToOrchestrator = iOnLanded;
        reticleAlpha = Mathf.Clamp01(iReticleAlpha);

        // Scale the slingshot like a near-plane object.
        transform.localScale = Vector3.one * room.ScaleAt(0f);

        // Reticle sits at a far landing point; detach it from the scaled slingshot so its
        // world position/scale are independent of the body, and tilt it onto the floor plane.
        if (reticleSR != null)
        {
            reticleSR.transform.SetParent(transform.parent, true);
            // Tilt derived from the room's floor perspective angle (+ optional manual offset).
            reticleSR.transform.rotation = Quaternion.Euler(room.GetFloorAngleDeg() + reticleFloorAngleOffsetDeg, 0f, 0f);
        }

        // All slingshot parts live in the foreground layer (always in front of the room/pots).
        if (lm2d != null)
        {
            if (bodySR != null) lm2d.PlaceForground(bodySR);
            if (rubberband != null) lm2d.PlaceForground(rubberband);
            if (loadedSR != null) lm2d.PlaceForground(loadedSR);
            if (reticleSR != null) lm2d.PlaceForground(reticleSR);
        }

        // Rest pouch position = midpoint between the two fork-tip anchors.
        anchor = (leftAnchor != null && rightAnchor != null)
            ? (leftAnchor.position + rightAnchor.position) * 0.5f
            : (pouch != null ? pouch.position : transform.position);
        if (pouch != null) { pouch.position = anchor; pouchRestScale = pouch.localScale; }

        Reload();
        if (rubberband != null) rubberband.enabled = true; // band is always strung on the fork
        DrawRubberband(anchor);
        SetReticleVisible(false);
    }

    // Quadratic Bezier: leftAnchor -> pouchPoint(control) -> rightAnchor, sampled for a smooth bend.
    void DrawRubberband(Vector3 pouchPoint)
    {
        if (rubberband == null || leftAnchor == null || rightAnchor == null) return;
        int n = Mathf.Max(2, rubberbandPoints);
        rubberband.positionCount = n;
        Vector3 p0 = leftAnchor.position, p1 = pouchPoint, p2 = rightAnchor.position;
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / (n - 1);
            float u = 1f - t;
            rubberband.SetPosition(i, u * u * p0 + 2f * u * t * p1 + t * t * p2);
        }
    }

    public void Reload()
    {
        loaded = nextProjectileProvider != null ? nextProjectileProvider() : ProjectileType.Almond;
        if (loadedSR != null)
            loadedSR.sprite = loaded == ProjectileType.Water ? waterSprite : almondSprite;
    }

    public void OnStartTracking(Vector2 iVec2)
    {
        aiming = true;
        pull = Vector2.zero;
        UpdateAim(iVec2);
    }

    public void OnPositionChanged(Vector2 iVec2)
    {
        if (!aiming) return;
        UpdateAim(iVec2);
    }

    public void OnStopTracking(Vector2 iVec2)
    {
        if (!aiming) return;
        UpdateAim(iVec2);
        Fire();
    }

    void UpdateAim(Vector2 thumb)
    {
        pull = new Vector2(thumb.x - anchor.x, thumb.y - anchor.y);
        if (pull.magnitude > config.maxPull) pull = pull.normalized * config.maxPull;

        // Pouch (and loaded projectile) follows the pull; band bends through it.
        Vector3 pouchPoint = anchor + new Vector3(pull.x, pull.y, 0f);
        if (pouch != null) pouch.position = pouchPoint;
        DrawRubberband(pouchPoint);

        float power = config.maxPull <= 0f ? 0f : pull.magnitude / config.maxPull;
        if (pouch != null)
            pouch.localScale = pouchRestScale * (1f + pouchPullScale * power); // relative Z- exaggeration

        LaunchParams lp = SeedArc.FromPull(pull, config);
        if (reticleSR != null && room != null && reticleAlpha > 0.001f)
        {
            SetReticleVisible(true);
            Vector3 rp = room.GroundPointAt(lp.lateralFrac, lp.depthFrac);
            rp.z = reticleSR.transform.position.z; // keep foreground z
            reticleSR.transform.position = rp;
            reticleSR.transform.localScale = Vector3.one * room.ScaleAt(lp.depthFrac);
            var c = reticleSR.color; c.a = reticleAlpha; reticleSR.color = c;
        }
        else SetReticleVisible(false);
    }

    void Fire()
    {
        aiming = false;
        SetReticleVisible(false);

        LaunchParams lp = SeedArc.FromPull(pull, config);

        // Capture the loaded seed's actual on-screen size *before* the pouch is reset below, so the
        // flying seed spawns at exactly that size (no size pop at release). Falls back to the tuning
        // value scaled to the near plane if the loaded sprite is missing.
        float launchScale = loadedSR != null
            ? loadedSR.transform.lossyScale.x
            : seedBaseScale * room.ScaleAt(0f);

        GameObject go = Instantiate(prefab_FlyingSeed, anchor, Quaternion.identity, transform.parent);
        var seed = go.GetComponent<FlyingSeed>();
        var seedSR = go.GetComponent<SpriteRenderer>();
        if (seedSR != null) seedSR.sprite = loaded == ProjectileType.Water ? waterSprite : almondSprite;
        seed.OnLanded += (frac, type) => onLandedToOrchestrator?.Invoke(frac, type);
        seed.Launch(room, lp, config, flightDuration, launchScale, seedFarScaleRatio, loaded);
        if (lm2d != null && seedSR != null)
        {
            lm2d.PlaceForground(seedSR); // always in front of the room/pots
            // Keep the rubberband drawing over the projectile (the band wraps in front of the seed).
            if (rubberband != null) rubberband.sortingOrder = seedSR.sortingOrder + 1;
        }

        // Snap pouch back to rest and relax the band.
        if (pouch != null) { pouch.position = anchor; pouch.localScale = pouchRestScale; }
        DrawRubberband(anchor);

        Reload(); // auto-load next-needed projectile
    }

    void SetReticleVisible(bool v) { if (reticleSR != null) reticleSR.enabled = v; }
}
