using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using Unity.VisualScripting;
public class RainDropsMiniGame : MiniGame, IArcadeFamily
{
    [Header("RainDropsMiniGame")]
    public GameObject prefab_walker;
    public GameObject prefab_rainDrop;
    public float[] walkDurations;
    public List<SplineContainer> linesDiff;
    private SplineContainer pathLine;
    public SplinePathRenderer SPR;
    Vector3[] positions;
    int positionCount;
    public UIArcadeInputs m_ArcadeInputs;

    public List<float> dropsPerSec;
    float lastDropTime = 0f;
    FireAutoWalker inst_walker;
    int inst_walker_layerPosition = 0;
    float pathCompletionRatio;
    List<AutoWalkKiller> inst_rainDrops;

    public GameObject rightTree;

    public override void Init()
    {
        
    }

    public override void Reset()
    {
        pathCompletionRatio = 0f;

        pathLine = linesDiff[MGM.LoopRank];
        pathLine.gameObject.SetActive(true);

        SpawnFireWalker();

        inst_rainDrops = new List<AutoWalkKiller>();
        inst_walker_layerPosition = MGM.LM2D.PlaceObject(inst_walker.handle_Renderer);

        if (MGM.miniGamesDifficulty >= 3)
            rightTree.SetActive(false);
        else
            rightTree.SetActive(true);
        preCompute();

    }

    public override void Play()
    {
        IsActiveMiniGame = true;
        lastDropTime = Time.time;
    }
    public override void Stop()
    {
        //PC.RemoveTapTracker(inst_walker);
        Destroy(inst_walker.gameObject);
        inst_rainDrops.ForEach(e => GameObject.Destroy(e.gameObject));
        pathLine.gameObject.SetActive(false);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
        inst_walker.OnKilledCB.RemoveAllListeners();
        inst_walker.isOnCandle = true;
        MGM.WinMiniGame();
    }
    public override void Lose()
    {
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        if (GameIsLost)
            return false;
        return Mathf.Approximately(inst_walker.transform.position.x, positions[positionCount-1].x);

    }

    void Update()
    {
        if (!IsActiveMiniGame)
            return;

        if ((Time.time - lastDropTime) >= (dropsPerSec[MGM.miniGamesDifficulty-1]))
        {
            SpawnRainDrop();
            lastDropTime = Time.time;
        }

    }

    void SpawnFireWalker()
    {
        inst_walker = GOBuilder.Create(prefab_walker)
                        .WithName("FireWalker")
                        .WithParent(transform)
                        .WithPosition(Vector3.zero)
                        .BuildAs<FireAutoWalker>();
        inst_walker.splineWalker.selfPathRenderer = SPR;
        inst_walker.splineWalker.SetSpline(pathLine);
        inst_walker.splineWalker.UpdatePosition(pathCompletionRatio);

        inst_walker.OnReachCB.AddListener(() => Win());
        inst_walker.OnKilledCB.AddListener(() => SpawnFireWalker());
        inst_walker.walkDuration = walkDurations[MGM.LoopRank];

        m_ArcadeInputs.Btn1Unbind();
        m_ArcadeInputs.OnBtn1Press(() => { inst_walker.AutoWalk = true; });
        m_ArcadeInputs.OnBtn1Release(() => { inst_walker.AutoWalk = false; });
    }


    void SpawnRainDrop()
    {
        float x = UnityEngine.Random.Range(PG.bounds.min.x, PG.bounds.max.x);
        float y = PG.bounds.max.y;
        Vector3 spawnPos = new Vector3(x, y, 0f);
        AutoWalkKiller newDrop = GOBuilder.Create(prefab_rainDrop)
                                    .WithName("RainDrop " + inst_rainDrops.Count + 1)
                                    .WithParent(transform)
                                    .WithPosition(spawnPos)
                                    .BuildAs<AutoWalkKiller>();
        newDrop.OnDestroyCB.AddListener(() => inst_rainDrops.Remove(newDrop));
        MGM.LM2D.PlaceObject(newDrop.handle_Renderer);
        inst_rainDrops.Add(newDrop);
    }
    
    void preCompute()
    {
        float preSimulatedSeconds = 3f;
        float dropInterval = dropsPerSec[MGM.miniGamesDifficulty - 1];

        int dropsToSpawn = Mathf.FloorToInt(preSimulatedSeconds / dropInterval);
        AutoWalkKiller drop;
        for (int i = 0; i < dropsToSpawn; i++)
        {
            SpawnRainDrop();
            drop = inst_rainDrops[i];
            float tSpawn = i * dropInterval;
            float tFall = tSpawn + drop.delayBeforeEnablingRB2D;
            float elapsedSinceFallStart = preSimulatedSeconds - tFall;

            if (elapsedSinceFallStart < 0)
            {
                drop.delayBeforeEnablingRB2D += elapsedSinceFallStart;
            }
            else
            {
                Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                rb.simulated = true;
                rb.bodyType = RigidbodyType2D.Dynamic;

                float g = rb.gravityScale * Physics2D.gravity.y;
                float fallDistance = 0.5f * Mathf.Abs(g) * Mathf.Pow(elapsedSinceFallStart, 2);
                float fallVelocity = g * elapsedSinceFallStart;

                Vector3 finalPos = Vector3.down * fallDistance;
                drop.transform.position += Vector3.down * fallDistance;
                rb.linearVelocity = new Vector2(0f, fallVelocity);

                RaycastHit2D hit = Physics2D.Raycast(finalPos + Vector3.down * 0.25f, Vector2.up, 4f, LayerMask.GetMask("Stickers"));
                if (hit.collider != null || finalPos.y < PG.bounds.min.y)
                {
                    Destroy(drop.gameObject);
                }
            }
        }
        lastDropTime = Time.time - (preSimulatedSeconds % dropInterval);
    }
}
