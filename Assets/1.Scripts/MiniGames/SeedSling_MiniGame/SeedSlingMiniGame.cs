using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public enum SeedSlingMode { SIMPLE_POTS, WATERED_POT }

public class SeedSlingMiniGame : MiniGame, IArcadeFamily
{
    [Header("SeedSling Config")]
    public SeedSlingMode mode = SeedSlingMode.SIMPLE_POTS;
    public GameObject prefab_PerspectiveRoom;
    public GameObject prefab_Slingshot;
    public GameObject prefab_SimplePot;
    public GameObject prefab_WateredPot;

    [Header("Difficulty (indexed by miniGamesDifficulty-1)")]
    public int[] potCountPerDifficulty = { 1, 1, 2, 3 };
    public float[] potDepthFracPerDifficulty = { 0.6f, 0.75f, 0.85f, 0.92f };
    public float[] potMouthRadiusPerDifficulty = { 0.10f, 0.08f, 0.07f, 0.05f };
    public float[] driftSpeedPerDifficulty = { 0f, 0f, 0.05f, 0.1f };

    [Header("Reticle fade (indexed by LoopRank); 0 = invisible")]
    public float[] reticleAlphaPerRank = { 1f, 1f, 0.6f, 0.3f, 0f, 0f };

    PerspectiveRoom inst_Room;
    Slingshot inst_Slingshot;
    readonly List<Pot> inst_Pots = new();

    int DiffIndex => Mathf.Clamp(MGM.miniGamesDifficulty - 1, 0, 3);

    public override void Init() { base.Init(); }

    public override void Reset()
    {
        base.Reset();
        CleanupInstances();

        inst_Room = GOBuilder.Create(prefab_PerspectiveRoom).WithParent(transform).BuildAs<PerspectiveRoom>();
        inst_Room.Init(MGM.LM2D);
        inst_Room.Build();

        SpawnPots();
        SpawnSlingshot();

        // Hand depth/scale/sort to LayerManager2D (same as Cowboy/WhackAMole).
        inst_Room.PlaceAllOnLayers();
    }

    void SpawnPots()
    {
        int count = (mode == SeedSlingMode.WATERED_POT) ? 1 : potCountPerDifficulty[DiffIndex];
        float depth = potDepthFracPerDifficulty[DiffIndex];
        float radius = potMouthRadiusPerDifficulty[DiffIndex];
        float drift = (mode == SeedSlingMode.WATERED_POT)
            ? Mathf.Max(0.05f, driftSpeedPerDifficulty[DiffIndex])
            : driftSpeedPerDifficulty[DiffIndex];

        GameObject prefab = (mode == SeedSlingMode.WATERED_POT) ? prefab_WateredPot : prefab_SimplePot;

        int rows = inst_Room.m_Depth;
        int row = Mathf.Clamp(Mathf.RoundToInt(depth * rows), 1, rows); // quantize depth -> room row

        for (int i = 0; i < count; i++)
        {
            float xFrac = count == 1 ? 0.5f : Mathf.Lerp(0.2f, 0.8f, (float)i / (count - 1));
            Pot pot = GOBuilder.Create(prefab).WithName("Pot" + i).WithParent(transform).BuildAs<Pot>();
            pot.roomRow = row;
            pot.xFrac = xFrac;
            pot.depthFrac = (float)row / rows;   // landing math matches the quantized row
            pot.mouthRadiusFrac = radius;
            pot.driftSpeed = drift;
            pot.Configure(mode == SeedSlingMode.WATERED_POT ? PotFill.Watered() : PotFill.Simple());
            pot.OnFertilized.AddListener(OnPotFertilized);

            // Room owns scale/y/z/sort; we only set the lateral x within the row.
            inst_Room.AddToRoom(pot.transform, row, pot.bodySR);
            Vector3 p = pot.transform.position;
            p.x = inst_Room.GetXRowLerp(row, xFrac);
            pot.transform.position = p;

            inst_Pots.Add(pot);
        }
    }

    void SpawnSlingshot()
    {
        inst_Slingshot = GOBuilder.Create(prefab_Slingshot).WithName("Slingshot").WithParent(transform).BuildAs<Slingshot>();
        // Low on screen like the Dart thrower (centered, ~5% up from the bottom); z via foreground.
        inst_Slingshot.transform.position = new Vector3(0f, PG.GetYPosFromHeightFrac(0.05f), 0f);
        float alpha = reticleAlphaPerRank[Mathf.Clamp(MGM.LoopRank, 0, reticleAlphaPerRank.Length - 1)];
        inst_Slingshot.Bind(inst_Room, NextProjectileForSlingshot, OnSeedLanded, alpha, MGM.LM2D);
        PC.AddPositionTracker(inst_Slingshot);
    }

    ProjectileType NextProjectileForSlingshot()
    {
        Pot target = NearestUnfertilizedPot();
        return target?.NextNeeded ?? ProjectileType.Almond;
    }

    Pot NearestUnfertilizedPot()
        => inst_Pots.Where(p => p != null && !p.IsFertilized)
                    .OrderBy(p => p.depthFrac).FirstOrDefault();

    void OnSeedLanded(Vector2 landingFrac, ProjectileType type)
    {
        Pot hit = inst_Pots
            .Where(p => p != null && !p.IsFertilized
                        && p.NextNeeded == type
                        && SeedArc.IsInPot(landingFrac, p.LandingFrac, p.MouthRadiusFrac))
            .OrderBy(p => (landingFrac - p.LandingFrac).magnitude)
            .FirstOrDefault();

        if (hit != null) hit.TryFertilize(type);
    }

    void OnPotFertilized()
    {
        if (SuccessCheck()) Win();
    }

    void Update()
    {
        if (inst_Room == null) return;
        // Only drift updates lateral x within the row; the room/LM2D own y, z, scale & sort.
        foreach (Pot p in inst_Pots)
        {
            if (p == null || p.driftSpeed <= 0f) continue;
            Vector3 pos = p.transform.position;
            pos.x = inst_Room.GetXRowLerp(p.roomRow, p.xFrac);
            p.transform.position = pos;
        }
    }

    public override void Play() { base.Play(); }

    public override void Stop()
    {
        base.Stop();
        if (inst_Slingshot != null) PC.RemovePositionTracker(inst_Slingshot);
        CleanupInstances();
    }

    public override void Win()
    {
        if (inst_Slingshot != null) PC.RemovePositionTracker(inst_Slingshot);
        base.Win(); // calls MGM.WinMiniGame()
    }

    public override void Lose() { base.Lose(); }

    public override bool SuccessCheck()
    {
        if (GameIsLost) return false;
        return inst_Pots.Count > 0 && inst_Pots.All(p => p == null || p.IsFertilized);
    }

    void CleanupInstances()
    {
        foreach (Pot p in inst_Pots) if (p != null) Destroy(p.gameObject);
        inst_Pots.Clear();
        if (inst_Slingshot != null) { Destroy(inst_Slingshot.gameObject); inst_Slingshot = null; }
        if (inst_Room != null) { inst_Room.Clean(); Destroy(inst_Room.gameObject); inst_Room = null; }
    }

    public override async UniTask IntroAnim(CancellationToken token) { return; }
}
