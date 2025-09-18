using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class Counting_MiniGame : MiniGame
{
    [Header("CountingMiniGame")]
    public GameObject prefabs_dice;
    public GameObject prefab_poolBall;

    public float spawnMargin;
    public int[] numberCounterObjects;
    private CounterObject[] counterObjects;
    private int counter = 1;

    public override void Init()
    {
        int n_spawns = numberCounterObjects[MGM.miniGamesDifficulty - 1];
        counterObjects = new CounterObject[n_spawns];
        counter = 1;
        for (int i = 0; i < n_spawns; i++)
        {
            counterObjects[i] = GOBuilder.Create(prefabs_dice)
            .WithName("counter" + i)
            .WithParent(transform)
            .WithRandomPositionInBoundsNoOverlap(PG.bounds, 0.5f)
            .Build().GetComponent<CounterObject>();
            counterObjects[i].Setup(i);
            //MGM.LM2D.PlaceObject(counterObjects[i].sr);
            PC.AddTapTracker(counterObjects[i]);
            counterObjects[i].tapCB.AddListener(diceSelected);
        }
    }

    public override void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public override void Stop()
    {
        destroyObjects();
        IsActiveMiniGame = false;
        IsInPostGame = false;
    }
    public override void Win()
    {
        MGM.WinMiniGame();
    }

    public override void Lose()
    {
        destroyObjects();
        Init();
        counter = 1;
    }
    public override bool SuccessCheck()
    {
        return false;
    }


    private void diceSelected(int count)
    {
        if (count == counter)
        {
            counterObjects[count-1].Selected();
            counter++;
            if (counter > numberCounterObjects[MGM.miniGamesDifficulty - 1])
            {
                Win();
            }
        }
        else
        {
            Lose();
        }
    }

    private void destroyObjects()
    {
        foreach (var counterObj in counterObjects)
        {
            PC.RemoveTapTracker(counterObj);
            Destroy(counterObj.gameObject);
        }
    }


}