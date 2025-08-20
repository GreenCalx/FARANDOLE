using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DartMiniGame : MiniGame
{
    [Header("DartMiniGame")]
    public GameObject prefab_gun;
    List<GameObject> balloons;
    public int n_balloons = 3;
    public GameObject prefab_Balloon;
    DartThrower inst_gun;
    public override void Init()
    {
        if (balloons != null && balloons.Count > 0)
            balloons.ForEach(e => Destroy(e.gameObject));

        int n_spawns = n_balloons * MGM.miniGamesDifficulty;
        balloons = new List<GameObject>(n_spawns);
        int sortingOrder = 0;
        for (int i = 0; i < n_spawns; i++)
        {
            balloons.Add(Instantiate(prefab_Balloon));
            balloons[i].name = "BalloonBundle " + i;
            balloons[i].transform.parent = transform;

            Balloon asBalloon = balloons[i].GetComponentInChildren<Balloon>();
            if (asBalloon != null)
            {
                Vector3 randPos = Random.insideUnitSphere;
                asBalloon.InitPhysxPosition(new Vector3(
                    randPos.x,
                    randPos.y
                ));
                asBalloon.InitSortOrder(sortingOrder);
                sortingOrder += 2;
            }
            ObjectTarget asTarget = balloons[i].GetComponentInChildren<ObjectTarget>();
            if (asTarget != null)
            {
                asTarget.OnTargetHit.AddListener(PopTarget);
            }

        }

        // init gun
        inst_gun = GOBuilder.Create(prefab_gun)
                    .WithName("DartThrower")
                    .WithPosition(new Vector3(0f, PG.GetYPosFromHeightFrac(0.05f), 0f))
                    .BuildAs<DartThrower>();
        PC.AddPositionTracker(inst_gun);
    }

    public void PopTarget(ObjectTarget iTarget)
    {
        GameObject bundleTarget = iTarget.transform.parent.gameObject;
        if (!balloons.Contains(bundleTarget))
            return;

        balloons.Remove(bundleTarget);

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
    }
    public override void Stop()
    {
        PC.RemovePositionTracker(inst_gun);
        Destroy(inst_gun.gameObject);
        IsActiveMiniGame = false;
    }
    public override void Win()
    {
        //PC.RemovePositionTracker(inst_gun);
        MGM.WinMiniGame();
    }
    public override void Lose()
    {
        
    }
    public override bool SuccessCheck()
    {
        return balloons.Count <= 0;
    }
}
