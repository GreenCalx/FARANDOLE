using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DartMiniGame : MiniGame
{
    public DartThrower gun;
    List<GameObject> balloons;
    public int n_balloons = 3;
    public GameObject prefab_Balloon;
    public override void Init()
    {
        if (balloons!=null && balloons.Count>0)
            balloons.ForEach(e => Destroy(e.gameObject));
            
        int n_spawns = n_balloons * MGM.miniGamesDifficulty;
        balloons = new List<GameObject>(n_spawns);
        for (int i = 0; i < n_spawns; i++)
        {
            balloons.Add(Instantiate(prefab_Balloon));
            balloons[i].name = "Balloon " + i;
            balloons[i].transform.parent = transform;

            balloons[i].transform.position = new Vector2(
                UnityEngine.Random.Range(PG.bounds.min.x, PG.bounds.max.x),
                UnityEngine.Random.Range(0, PG.bounds.max.y)
            );

            ObjectTarget asTarget = balloons[i].GetComponent<ObjectTarget>();
            if (asTarget != null)
            {
                asTarget.OnTargetHit.AddListener(PopTarget);
            }
        }
        PC.AddPositionTracker(gun);
    }

    public void PopTarget(ObjectTarget iTarget)
    {
        if (balloons.Contains(iTarget.gameObject))
            balloons.Remove(iTarget.gameObject);
        Destroy(iTarget.gameObject);

        if (SuccessCheck())
            Win();
    }
    public override void Play()
    {
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        PC.RemovePositionTracker(gun);
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
        return balloons.Count <= 0;
    }
}
