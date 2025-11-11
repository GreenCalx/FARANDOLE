using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Threading.Tasks;
public class RainDropsMiniGame : MiniGame
{
    [Header("RainDropsMiniGame")]
    public GameObject prefab_walker;
    public GameObject prefab_rainDrop;
    public Transform handle_startLine;
    public Transform handle_endLine;

    public UIArcadeInputs m_ArcadeInputs;

    public float timeBeforeRainDrops = 0.5f;
    public float dropsPerSec = 1f;
    public Material pathLRMat;
    [Range(0.03f,0.1f)]
    public float LRWidth = 0.03f;
    float lastDropTime = 0f;
    AutoWalker inst_walker;
    int inst_walker_layerPosition = 0;
    List<AutoWalkKiller> inst_rainDrops;

    LineRenderer pathLR;

    public override void Init()
    {
        
    }

    public override void Reset()
    {
        pathLR = GOBuilder.Create()
                    .WithName("PathLineRenderer")
                    .WithParent(transform)
                    .WithPosition(handle_startLine.position)
                    .WithLineRenderer(pathLRMat)
                    .BuildAs<LineRenderer>();
        pathLR.startWidth = LRWidth;
        pathLR.endWidth = LRWidth;
        pathLR.positionCount = 2;
        Vector3[] positions = {
                                handle_startLine.position,
                                handle_endLine.position
                                };
        pathLR.SetPositions(positions);
        MGM.LM2D.PlaceObject(pathLR);

        SpawnFireWalker();

        inst_rainDrops = new List<AutoWalkKiller>();
        inst_walker_layerPosition = MGM.LM2D.PlaceObject(inst_walker.handle_Renderer);
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
        Destroy(pathLR.gameObject);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
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
        return Mathf.Approximately(inst_walker.transform.position.x, handle_endLine.position.x);
    }

    void Update()
    {
        if (!IsActiveMiniGame)
            return;

        if ((Time.time - lastDropTime) >= (dropsPerSec / MGM.miniGamesDifficulty))
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
                        .WithPosition(handle_startLine.position)
                        .BuildAs<AutoWalker>();
        inst_walker.from = handle_startLine.position;
        inst_walker.to = handle_endLine.position;
        inst_walker.OnReachCB.AddListener(Win);
        inst_walker.OnKilledCB.AddListener(() => SpawnFireWalker());

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
}
