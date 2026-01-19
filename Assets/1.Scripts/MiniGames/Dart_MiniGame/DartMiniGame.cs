using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DartMiniGame : MiniGame, IRegularFamily
{
    [Header("DartMiniGame")]
    public GameObject prefab_gun;
    List<GameObject> balloons;
    public SpawnableDef balloonDef;
    public SpawnableDef bigBalloonDef;
    public SpawnableDef rockDef;
    private BlockingRock inst_rock;
    DartThrower inst_gun;

    public override void Init()
    {
        // init gun
        inst_gun = GOBuilder.Create(prefab_gun)
                    .WithName("DartThrower")
                    .WithPosition(new Vector3(0f, PG.GetYPosFromHeightFrac(0.05f), 0f))
                    .WithParent(transform)
                    .BuildAs<DartThrower>();
    }

    public override void Reset()
    {
        if (balloons != null && balloons.Count > 0)
            balloons.ForEach(e => Destroy(e.gameObject));

        ISpawnerExtension spawnerExt = GetExtension<ISpawnerExtension>();

        int n_spawns    = spawnerExt?.Get(balloonDef, MGM.MGLoop.rank) ?? 0;
        int n_bigSpawns = spawnerExt?.Get(bigBalloonDef, MGM.MGLoop.rank) ?? 0;
        int n_rocks     = spawnerExt?.Get(rockDef, MGM.MGLoop.rank) ?? 0;

        if (n_rocks > 0)
        {
            inst_rock = GOBuilder.Create(rockDef.prefab)
                .WithPosition(new Vector3(UnityEngine.Random.Range(PG.bounds.min.x, PG.bounds.max.x), PG.GetYPosFromHeightFrac(0.5f), 0f))
                .WithParent(this.transform)
                .BuildAs<BlockingRock>();
            inst_rock.StartPatrol(PG.bounds.min.x, PG.bounds.max.x);
        }

        balloons = new List<GameObject>(n_spawns+n_bigSpawns*2);
        for (int i = 0; i < n_spawns + n_bigSpawns; i++)
        {
            SpawnBalloon(i, i >= n_spawns,UnityEngine.Random.insideUnitSphere);
        }

        PC.AddPositionTracker(inst_gun);
    }

    public void SpawnBalloon(int index, bool big, Vector2 pos)
    {
        if (big)
        {
            balloons.Add(Instantiate(bigBalloonDef.prefab));
            balloons[index].gameObject.tag = "BigBalloon";
        }
        else
            balloons.Add(Instantiate(balloonDef.prefab));
        balloons[index].name = "BalloonBundle " + index;
        balloons[index].transform.parent = transform;

        Balloon asBalloon = balloons[index].GetComponentInChildren<Balloon>();
        if (asBalloon != null)
        {
            Debug.Log("balloon");
            asBalloon.InitPhysxPosition(new Vector3(
                pos.x,
                pos.y
            ));
            asBalloon.InitSortOrder(MGM.LM2D);
            asBalloon.invulnerabilityTime();
        }
        ObjectTarget asTarget = balloons[index].GetComponentInChildren<ObjectTarget>();
        if (asTarget != null)
        {
            asTarget.OnTargetHit.AddListener(PopTarget);
        } 

        // Balloons can be bumped out of bounds on spawn
        OutOfBoundCheck();
    }

    

    public void PopTarget(ObjectTarget iTarget)
    {
        GameObject bundleTarget = iTarget.transform.parent.gameObject;
        if (!balloons.Contains(bundleTarget))
            return;

        balloons.Remove(bundleTarget);

        if (bundleTarget.gameObject.tag == "BigBalloon")
        {
            SpawnBalloon(balloons.Count, false, iTarget.transform.position);
            SpawnBalloon(balloons.Count, false, iTarget.transform.position);
            SpawnBalloon(balloons.Count, false, iTarget.transform.position);
        }
        Balloon asBalloon = iTarget.GetComponent<Balloon>();
        if (asBalloon != null)
        {
            asBalloon.ExplodeAnim();
        }
        Destroy(bundleTarget, 0.2f);

        if (SuccessCheck())
            Win();
    }
    public override void Play()
    {
        IsActiveMiniGame = true;

        // destroy potential balloons out of the PG
        OutOfBoundCheck();
    }

    public void OutOfBoundCheck()
    {
        if (balloons==null)
            return;
            
        GameObject[] snap = balloons.ToArray();
        foreach (GameObject b in snap)
        {
            ObjectTarget asTarget = b.GetComponentInChildren<ObjectTarget>();
            if (asTarget==null)
                continue;
            if (!Utils.IsContained2D(new Vector2(asTarget.transform.position.x, asTarget.transform.position.y), MGM.PG.bounds))
            {
                PopTarget(asTarget);
            }
        }
    }
    public override void Stop()
    {
        PC.RemovePositionTracker(inst_gun);
        inst_gun.ClearThrownObjects();
        if(inst_rock != null)
            Destroy(inst_rock.gameObject);
        IsActiveMiniGame = false;
    }
    void OnDestroy()
    {
        if(inst_rock != null)
            Destroy(inst_rock.gameObject);
        Destroy(inst_gun.gameObject);
    }
    public override void Win()
    {
        //PC.RemovePositionTracker(inst_gun);
        MGM.WinMiniGame();
    }
    public override void Lose()
    {
        //Destroy(inst_rock.gameObject);
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        if (GameIsLost)
            return false;
        return balloons.Count <= 0;
    }

    void Update()
    {


        
    }
}
