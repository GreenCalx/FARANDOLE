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
    public UIVisualToggle UIVisualToggle;

    public float timeBeforeRainDrops = 0.5f;
    public float dropsPerSec = 1f;
    public Material pathLRMat;
    [Range(0.03f,0.1f)]
    public float LRWidth = 0.03f;
    float lastDropTime = 0f;
    AutoWalker inst_walker;
    List<AutoWalkDelayer> inst_rainDrops;

    LineRenderer pathLR;

    public override void Init()
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


        inst_walker = GOBuilder.Create(prefab_walker)
                        .WithName("AutoWalker")
                        .WithParent(transform)
                        .WithPosition(handle_startLine.position)
                        .BuildAs<AutoWalker>();
        inst_walker.from = handle_startLine.position;
        inst_walker.to = handle_endLine.position;
        inst_walker.OnReachCB.AddListener(() => { if (SuccessCheck()) { Win(); } });
        inst_walker.OnAutoWalkToggleCB.AddListener((b) => UIVisualToggle.Toggle(b));
        inst_walker.OnPreDelayedyCB.AddListener(() => UIVisualToggle.freeze = true);
        inst_walker.OnPostDelayedCB.AddListener(() => UIVisualToggle.freeze = false);

        PC.AddTapTracker(inst_walker);

        UIVisualToggle.Toggle(inst_walker.AutoWalk);
        UIVisualToggle.freeze = false;

        inst_rainDrops = new List<AutoWalkDelayer>();

        MGM.LM2D.PlaceObject(pathLR);
        MGM.LM2D.PlaceObject(inst_walker.handle_Renderer);
        
    }
    public override void Play()
    {
        IsActiveMiniGame = true;
        lastDropTime = Time.time;
    }
    public override void Stop()
    {
        PC.RemoveTapTracker(inst_walker);
        Destroy(inst_walker.gameObject);
        inst_rainDrops.ForEach(e => GameObject.Destroy(e.gameObject));
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
        MGM.WinMiniGame();
    }
    public override void Lose()
    {

    }
    public override bool SuccessCheck()
    {
        return Mathf.Approximately(inst_walker.transform.position.x, handle_endLine.position.x);
    }

    void Update()
    {
        if (!IsActiveMiniGame)
            return;

        if ((Time.time - lastDropTime) >= (dropsPerSec/MGM.miniGamesDifficulty))
        {
            SpawnRainDrop();
            lastDropTime = Time.time;
        }

    }

    void SpawnRainDrop()
    {
        float x = UnityEngine.Random.Range(PG.bounds.min.x, PG.bounds.max.x);
        float y = PG.bounds.max.y;
        Vector3 spawnPos = new Vector3(x, y, 0f);
        AutoWalkDelayer newDrop = GOBuilder.Create(prefab_rainDrop)
                                    .WithName("RainDrop " + inst_rainDrops.Count + 1)
                                    .WithParent(transform)
                                    .WithPosition(spawnPos)
                                    .BuildAs<AutoWalkDelayer>();
        newDrop.OnDestroyCB.AddListener(() => inst_rainDrops.Remove(newDrop));
        MGM.LM2D.PlaceObject(newDrop.handle_Renderer);
        inst_rainDrops.Add(newDrop);
    }
}
