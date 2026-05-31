using UnityEngine;
using UnityEngine.Events;

public class Pot : MonoBehaviour
{
    public const string GrowAnimTriggParm = "grow";
    [Header("Visuals")]
    public SpriteRenderer bodySR;
    public Sprite drySprite;       // dry_flower_pot_0
    public Sprite[] growthSprites; // flower_pot_0..3
    public Animator animator;
    public Sprite happyFlower;     // happy_flower
    public StreamFilled waterFill; // optional; WATERED_POT only

    [Header("Layout (room frac space)")]
    public float xFrac = 0.5f;
    public float depthFrac = 0.9f;
    public float mouthRadiusFrac = 0.06f;
    [HideInInspector] public int roomRow;   // assigned via PerspectiveRoom.AddToRoom

    [Header("Drift (0 = stationary)")]
    public float driftSpeed = 0f;  // frac per second
    public float driftMinXFrac = 0.15f;
    public float driftMaxXFrac = 0.85f;

    public UnityEvent OnFertilized;

    PotFill fill;
    int driftDir = 1;

    public Vector2 LandingFrac => new Vector2(xFrac, depthFrac);
    public float MouthRadiusFrac => mouthRadiusFrac;
    public bool IsFertilized => fill != null && fill.IsFertilized;
    public ProjectileType? NextNeeded => fill?.NextNeeded();

    public void Configure(PotFill iFill)
    {
        fill = iFill;
        if (waterFill != null) waterFill.Flush();
        RefreshBody();
    }

    public bool TryFertilize(ProjectileType t)
    {
        if (fill == null || !fill.TryAdd(t)) return false;

        if (t == ProjectileType.Water && waterFill != null)
            waterFill.AddLiquid(waterFill.maxFill);

        RefreshBody();
        if (fill.IsFertilized) Bloom();
        return true;
    }

    void RefreshBody()
    {
        if (bodySR == null) return;
        bool waterPending = NextNeeded == ProjectileType.Water;
        if (drySprite != null && waterPending)
            bodySR.sprite = drySprite;
        else if (growthSprites != null && growthSprites.Length > 0)
            bodySR.sprite = growthSprites[0];
    }

    void Bloom()
    {
        if (animator != null)
        {
            animator.SetTrigger(GrowAnimTriggParm);
        }
        OnFertilized?.Invoke();
    }

    void Update()
    {
        if (driftSpeed <= 0f) return;
        xFrac += driftDir * driftSpeed * Time.deltaTime;
        if (xFrac >= driftMaxXFrac) { xFrac = driftMaxXFrac; driftDir = -1; }
        else if (xFrac <= driftMinXFrac) { xFrac = driftMinXFrac; driftDir = 1; }
    }
}
