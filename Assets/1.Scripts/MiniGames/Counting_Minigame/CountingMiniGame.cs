using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;

public class Counting_MiniGame : MiniGame, IRegularMod, ISpawnerMod<CounterObject>
{
    [Header("CountingMiniGame")]
    public GameObject prefabs_dice;
    public GameObject prefab_poolBall;
    public GameObject prefab_OnTapParticles;
    private float spawnMargin;
    public int[] numberCounterObjects;
    private CounterObject[] counterObjects;
    private int counter = 1;
    private Vector2[] counterObjectsPositions;
    List<ParticleSystem> inst_OnTapPS;
    public int fantasyNumberThreshold = 3;
    public float randomTiltRangeMin = -25f,
                 randomTiltRangeMax = 25f;
    public override void Init()
    {
        //Reset();

    }

    public override void Reset()
    {
        if ((inst_OnTapPS!=null)&&(inst_OnTapPS.Count>0))
        {
            inst_OnTapPS.ForEach(e =>
            {
                if (e != null)
                    GameObject.Destroy(e.gameObject);
            });
            inst_OnTapPS.Clear();
        } else
        {
            inst_OnTapPS = new List<ParticleSystem>(numberCounterObjects[numberCounterObjects.Length - 1]);
        }
        

        int n_spawns = numberCounterObjects[MGM.miniGamesDifficulty - 1];
        counterObjects          = new CounterObject[n_spawns];
        counterObjectsPositions = new Vector2[n_spawns];
        counter = 1;
        for (int i = n_spawns-1; i >= 0; i--)
        {
            counterObjects[i] = GOBuilder.Create(prefabs_dice)
            .WithName("counter" + i)
            .WithParent(transform)
            .WithPosition(counterObjectsPositions[i])
            .WithZRotation(UnityEngine.Random.Range(randomTiltRangeMin * MGM.miniGamesDifficulty, randomTiltRangeMax* MGM.miniGamesDifficulty))
            .BuildAs<CounterObject>();
            counterObjects[i].Setup(i, (MGM.miniGamesDifficulty > fantasyNumberThreshold));
            
            PC.AddTapTracker(counterObjects[i]);
            counterObjects[i].tapCB.AddListener(diceSelected);

            MGM.LM2D.PlaceObject(counterObjects[i].stickerSR);
            MGM.LM2D.PlaceObject(counterObjects[i].bodySR);
            MGM.LM2D.PlaceObject(counterObjects[i].sr);
        }
        spawnMargin = (counterObjects[0].sr.bounds.max.x - counterObjects[0].sr.bounds.min.x);
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
        // destroyObjects();
        // Reset();
        // counter = 1;
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        if (GameIsLost)
            return false;
        return (counter > numberCounterObjects[MGM.miniGamesDifficulty - 1]);
    }

    public void SpawnParticles(Vector3 iPosition)
    {
        ParticleSystem new_OnTapPS = GOBuilder.Create(prefab_OnTapParticles)
                .WithName("OnTapPS")
                .WithParent(transform)
                .WithPosition(iPosition)
                .BuildAs<ParticleSystem>();
        inst_OnTapPS.Add(new_OnTapPS);
        GameObject.Destroy(new_OnTapPS.gameObject, new_OnTapPS.main.startLifetime.Evaluate(0f));
    }

    private void diceSelected(int count)
    {
        if (count == counter)
        {
            SpawnParticles(counterObjects[count - 1].transform.position);
            counterObjects[count - 1].Selected();

            counter++;
            if (SuccessCheck())
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

            } while (hasOverlap && safetyCounter < 10); // 50 tentatives max
        }
    }

    #region MODS
    public void ApplyMod()
    {

    }
    public CounterObject Spawn(GameObject iPrefab)
    {
        // no impl atm
        return null;
    }
    #endregion

}