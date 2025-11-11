using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using Unity.VisualScripting;
public class RainDropsMiniGame : MiniGame
{
    [Header("RainDropsMiniGame")]
    public GameObject prefab_walker;
    public GameObject prefab_rainDrop;

    public List<LineRenderer> linesDiff;
    private LineRenderer pathLine;
    Vector3[] positions;
    int positionCount;
    public UIArcadeInputs m_ArcadeInputs;

    public List<float> dropsPerSec;
    float lastDropTime = 0f;
    FireAutoWalker inst_walker;
    int inst_walker_layerPosition = 0;
    List<AutoWalkKiller> inst_rainDrops;

    public GameObject rightTree;

    public override void Init()
    {
        
    }

    public override void Reset()
    {
        pathLine = linesDiff[MGM.miniGamesDifficulty - 1];
        pathLine.gameObject.SetActive(true);
        positionCount = pathLine.positionCount;
        positions = new Vector3[positionCount];
        pathLine.GetPositions(positions);
        MGM.LM2D.PlaceObject(pathLine);

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

    }
    public override bool SuccessCheck()
    {
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
                        .WithPosition(positions[0])
                        .BuildAs<FireAutoWalker>();
        inst_walker.from = positions[0];
        inst_walker.to = positions[1];
        float[] distances = new float[positionCount - 1];
        float totalDistance = 0f;
        inst_walker.walkDurations = new float[positionCount - 1];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = Vector3.Distance(positions[i], positions[i + 1]);
            totalDistance += distances[i];
        }
        float frac;
        for(int i = 0; i < distances.Length; i++)
        {
            frac = distances[i] / totalDistance;
            inst_walker.walkDurations[i] = inst_walker.walkDuration * frac;
        }

        inst_walker.nextPositionIndex = 1;
        inst_walker.OnReachCB.AddListener(ReachPos);
        inst_walker.OnKilledCB.AddListener(() => SpawnFireWalker());
        inst_walker.walkDuration = inst_walker.walkDurations[0];

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

    void ReachPos()
    {
        if (inst_walker.nextPositionIndex == positionCount - 1)
        {
            Win();
        }
        else
        {
            inst_walker.from = positions[inst_walker.nextPositionIndex];
            inst_walker.nextPositionIndex += 1;
            inst_walker.to = positions[inst_walker.nextPositionIndex];
            inst_walker.ResetWalker();
        }
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
