using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class SheepMiniGame : MiniGame
{
    [Header("SheepMiniGame")]
    public GameObject prefab_sheep;
    public GameObject prefab_fence;
    private Sheep[] inst_sheeps;
    private Fence inst_fence;

    public int[] sheepsCounts;
    int sheepCount;
    public override void Init()
    {
       // Reset();
    }

    public override void Reset()
    {
        sheepCount = sheepsCounts[MGM.miniGamesDifficulty - 1];
        inst_fence = GOBuilder.Create(prefab_fence)
                    .WithParent(transform)
                    .BuildAs<Fence>();

        inst_fence.totalSheepsNumber = sheepCount;

        inst_sheeps = GOSpawner.SpawnAtRandom<Sheep>(transform, prefab_sheep, PG.bounds, sheepCount, "sheep", inst_fence.GetComponent<SpriteRenderer>().bounds);
        foreach (Sheep sheep in inst_sheeps)
        {
            sheep.droppedEvent.AddListener(() => { if (SuccessCheck()) { Win(); } });
            if (sheep.PGClamper != null)
            {
                sheep.PGClamper.PG = MGM.PG;
                sheep.PGClamper.OnClamp.AddListener(() => sheep.OnStopTracking(sheep.transform.position));
            }
                
            sheep.SetOutTargetZone();
            PC.AddPositionTracker(sheep);
        }
    }

    public override void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public override void Stop()
    {
        Clean();
        IsActiveMiniGame = false;
        IsInPostGame = false;
    }
    public override void Win()
    {
        MGM.WinMiniGame();
    }
    public override void Lose()
    {
        IsInPostGame = false;
    }
    public override bool SuccessCheck()
    {
        return inst_fence.isFull;
    }

    private void Clean()
    {
        for (int i = 0; i < sheepCount; i++)
        {
            if (inst_sheeps[i] != null)
            {
                PC.RemovePositionTracker(inst_sheeps[i]);
                Destroy(inst_sheeps[i].gameObject);
            }
            if (inst_fence != null)
                Destroy(inst_fence.gameObject);
        }
    }
    
}