using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;

public class CowboyMiniGame : MiniGame, ITheaterMod, ISpawnerMod<Cowboy>
{
    [Header("CowboyMiniGame")]
    public GameObject prefab_PerspectiveRoom;

    public GameObject prefab_cowboy;

    public List<GameObject> prefab_obstacles;

    PerspectiveRoom inst_PerspectiveRoom;

    public int[] cowboysPerDifficulty;

    private int numberOfRows;
    public int[] minObstaclesNumber;
    public int[] maxObstaclesNumber;
    List<Obstacle> inst_obstacles;
    List<Cowboy> inst_cowboys;
    public override void Init()
    {

    }

    public override void Reset()
    {
        numberOfRows = MGM.miniGamesDifficulty;

        // room
        inst_PerspectiveRoom = GOBuilder.Create(prefab_PerspectiveRoom)
                                .WithParent(transform)
                                .BuildAs<PerspectiveRoom>();
        inst_PerspectiveRoom.Init(MGM.LM2D, numberOfRows);
        inst_PerspectiveRoom.Build();

        // cowboys & obstacles

        inst_cowboys = new List<Cowboy>(numberOfRows);

        for (int i = 0; i < numberOfRows; i++)
        {
            Cowboy newCowboy = GOBuilder.Create(prefab_cowboy)
                .WithName("Cowboy" + i)
                .WithParent(this.gameObject.transform)
                .BuildAs<Cowboy>();

            PC.AddTapTracker(newCowboy);
            newCowboy.hitCB.AddListener(CowboyShot);

            //MGM.LM2D.PlaceObject(newCowboy.cowboySR);
            inst_PerspectiveRoom.AddToRoom(newCowboy.transform, i, newCowboy.GetRenderer());

            newCowboy.SetMovement(
                inst_PerspectiveRoom.XMinForRow(i),
                inst_PerspectiveRoom.XMaxForRow(i)
                );
            inst_cowboys.Add(newCowboy);
        }
        PositionCowboysAndObstacles();

        inst_PerspectiveRoom.PlaceAllOnLayers();
    }



    void PositionCowboysAndObstacles()
    {
        int n_obstacles;

        inst_obstacles = new List<Obstacle>();

        for (int i = 0; i < numberOfRows; i++)
        {
            n_obstacles = Random.Range(minObstaclesNumber[MGM.miniGamesDifficulty-1], maxObstaclesNumber[MGM.miniGamesDifficulty-1]);
            for (int j = 0; j < n_obstacles; j++)
            {
                int randObstacle = UnityEngine.Random.Range(0, prefab_obstacles.Count);
                Obstacle newObstacle = GOBuilder.Create(prefab_obstacles[randObstacle])
                    .WithName("Obstacle " + i + j)
                    .WithParent(this.transform)
                    .BuildAs<Obstacle>();

                PC.AddTapTracker(newObstacle);
                newObstacle.OnTapDo.AddListener( () =>
                    {
                        PC.RemoveTapTracker(newObstacle);
                        newObstacle.Kill();
                    }
                );

                inst_PerspectiveRoom.AddToRoom(newObstacle.transform, i, newObstacle.GetStickerRenderer());
                RowInfo rowInfo = inst_PerspectiveRoom.AddToRoom(newObstacle.transform, i, newObstacle.GetRenderer());

                newObstacle.transform.position = new Vector3(
                    Random.Range(inst_PerspectiveRoom.XMinForRow(i), inst_PerspectiveRoom.XMaxForRow(i)),
                    newObstacle.transform.position.y,
                    newObstacle.transform.position.z
                );
                inst_obstacles.Add(newObstacle);
            }
        }

    }
    public override void Play()
    {
        IsActiveMiniGame = true;
    }
    public override void Stop()
    {
        Clean();
    }
    public override void Win()
    {
        //Clean();
        MGM.WinMiniGame();
    }

    public void Clean()
    {
        foreach (Cowboy cowboy in inst_cowboys)
        {
            if (cowboy != null)
            {
                PC.RemoveTapTracker(cowboy);
                Destroy(cowboy.gameObject);
            }
        }
        inst_cowboys.Clear();
        
        foreach (Obstacle obstacle in inst_obstacles)
        {
            if (obstacle != null)
            {
                PC.RemoveTapTracker(obstacle);
                Destroy(obstacle.gameObject);
            }
        }
        inst_obstacles.Clear();

        inst_PerspectiveRoom.Clean();
        Destroy(inst_PerspectiveRoom.gameObject);
        inst_PerspectiveRoom = null;
    }
    public override void Lose()
    {

    }
    public override bool SuccessCheck()
    {
        return inst_cowboys.Count == 0;
    }

    public void CowboyShot(Cowboy iHitCowboy)
    {
        inst_cowboys.Remove(iHitCowboy);
        iHitCowboy.StopMovement();
        // iHitCowboy.DeathAnimation(
        //         new Vector2(
        //         inst_PerspectiveRoom.GetVanishingPoint().x,
        //         inst_PerspectiveRoom.m_FarPlan.min.y
        //         )
        //     );
                iHitCowboy.DeathAnimation(
                new Vector2(
                iHitCowboy.transform.position.x,
                -5f
                )
            );
        PC.RemoveTapTracker(iHitCowboy);

        if (SuccessCheck())
        {
            Win();
        }
    }

    #region ISpawnerMod
    public void ApplyMod()
    {
        // TODO : Change spawn Interval or spawn probability random range
    }
    public Cowboy Spawn(GameObject iPrefab)
    {
        Cowboy newCowboy = GOBuilder.Create(prefab_cowboy)
            .WithName("Cowboy" )
            .WithParent(this.gameObject.transform)
            .BuildAs<Cowboy>();
    
        return newCowboy;
    }
    #endregion
}
