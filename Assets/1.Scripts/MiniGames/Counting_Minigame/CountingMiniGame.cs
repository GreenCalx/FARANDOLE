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

    private float spawnMargin;
    public int[] numberCounterObjects;
    private CounterObject[] counterObjects;
    private int counter = 1;
    private Vector2[] counterObjectsPositions;

    public override void Init()
    {
        //Reset();
    }

    public override void Reset()
    {
        int n_spawns = numberCounterObjects[MGM.miniGamesDifficulty - 1];
        counterObjects          = new CounterObject[n_spawns];
        counterObjectsPositions = new Vector2[n_spawns];
        counter = 1;
        for (int i = 0; i < n_spawns; i++)
        {
            counterObjects[i] = GOBuilder.Create(prefabs_dice)
            .WithName("counter" + i)
            .WithParent(transform)
            .WithPosition(counterObjectsPositions[i])
            .Build().GetComponent<CounterObject>();
            counterObjects[i].Setup(i);
            //MGM.LM2D.PlaceObject(counterObjects[i].sr);
            PC.AddTapTracker(counterObjects[i]);
            counterObjects[i].tapCB.AddListener(diceSelected);
        }
        spawnMargin = (counterObjects[0].sr.bounds.max.x - counterObjects[0].sr.bounds.min.x)/2;
        PositionAtRandomNoOverlap();
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
        Reset();
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
            counterObjects[count - 1].Selected();
            counter++;
            if (counter > numberCounterObjects[MGM.miniGamesDifficulty - 1])
            {
                Win();
            }
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

    public void PositionAtRandomNoOverlap()
    {
        bool hasOverlap;
        int safetyCounter = 0;

        for (int i = 0; i < counterObjects.Length; i++)
        {
            do
            {
                hasOverlap = false;
                counterObjects[i].gameObject.transform.position = new Vector3(
                    UnityEngine.Random.Range(PG.bounds.min.x + spawnMargin, PG.bounds.max.x - spawnMargin),
                    UnityEngine.Random.Range(PG.bounds.min.y + spawnMargin, PG.bounds.max.y - spawnMargin),
                    0
                );

                for (int j = 0; j < i; j++)
                {
                    if ((counterObjects[i].transform.position - counterObjects[j].transform.position).magnitude < spawnMargin)
                    {
                        hasOverlap = true;
                    }
                }

                safetyCounter++;

            } while (hasOverlap && safetyCounter < 5); // 50 tentatives max
        }
    }

}