using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;

public class CowboyMiniGame : MiniGame
{
    [Header("CowboyMiniGame")]
    public GameObject prefab_PerspectiveRoom;

    public GameObject prefab_cowboy;

    public GameObject prefab_obstacle;

    PerspectiveRoom inst_PerspectiveRoom;

    public Obstacle[] inst_obstacles;
    public Cowboy[] inst_cowboys;

    public int[] cowboysPerDifficulty;

    private int numberOfCowboys;
    public int maxObstaclesNumber;

    public float perspectiveMarginDelta; // En pourcentage de reduction
    public float perspectiveShrinkDelta;// En pourcentage de reduction

    public override void Init()
    {
        // Reset();
       
    }

    public override void Reset()
    {
        numberOfCowboys = cowboysPerDifficulty[MGM.miniGamesDifficulty - 1];

        // room
        inst_PerspectiveRoom = GOBuilder.Create(prefab_PerspectiveRoom)
                                .WithParent(transform)
                                .BuildAs<PerspectiveRoom>();
        inst_PerspectiveRoom.Init(MGM.LM2D, numberOfCowboys);
        inst_PerspectiveRoom.Build();

        // cowboys & obstacles

        inst_cowboys = new Cowboy[numberOfCowboys];
        inst_obstacles = new Obstacle[numberOfCowboys * maxObstaclesNumber];

        for (int i = 0; i < numberOfCowboys; i++)
        {
            inst_cowboys[i] = GOBuilder.Create(prefab_cowboy)
                .WithName("Cowboy" + i)
                .WithParent(this.gameObject.transform)
                .BuildAs<Cowboy>();

            PC.AddTapTracker(inst_cowboys[i]);
            inst_cowboys[i].hitCB.AddListener(CowboyShot);

            //MGM.LM2D.PlaceObject(inst_cowboys[i].cowboySR);
            inst_PerspectiveRoom.AddToRoom(inst_cowboys[i].transform, i, inst_cowboys[i].GetRenderer());
            inst_cowboys[i].SetMovement(
                inst_PerspectiveRoom.XMinForRow(i),
                inst_PerspectiveRoom.XMaxForRow(i)
                );
        }
        PositionCowboysAndObstacles();

        inst_PerspectiveRoom.PlaceAllOnLayers();
    }



    void PositionCowboysAndObstacles()
    {
        float y_delta = (PG.bounds.max.y - PG.bounds.min.y) / (numberOfCowboys + 1);
        float cowboyHalfHeight = inst_cowboys[0].hitbox.bounds.size.y / 2;

        int n_obstacles;
        int obstacle_index = 0;
        float obstacle_offset = -0.2f;
        float minX, maxX, posY, scale;

        for (int i = 0; i < numberOfCowboys; i++)
        {
            n_obstacles = Random.Range(1, maxObstaclesNumber + 1);
            for (int j = 0; j < n_obstacles; j++)
            {
                inst_obstacles[j] = GOBuilder.Create(prefab_obstacle)
                    .WithName("Obstacle " + j)
                    .WithParent(this.transform)
                    .BuildAs<Obstacle>();

                PC.AddTapTracker(inst_obstacles[j]);
                RowInfo rowInfo = inst_PerspectiveRoom.AddToRoom(inst_obstacles[j].transform, i, inst_obstacles[j].GetRenderer());
                inst_obstacles[j].transform.position = new Vector3(
                    Random.Range(rowInfo.min.x, rowInfo.max.x),
                    inst_obstacles[j].transform.position.y,
                    inst_obstacles[j].transform.position.z
                );
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
        foreach (Obstacle obstacle in inst_obstacles)
        {
            if (obstacle != null)
            {
                PC.RemoveTapTracker(obstacle);
                Destroy(obstacle.gameObject);
            }
        }
        foreach (Transform c in inst_PerspectiveRoom.transform)
        {
            Destroy(c.gameObject);
        }
        Destroy(inst_PerspectiveRoom.gameObject);
    }
    public override void Lose()
    {

    }
    public override bool SuccessCheck()
    {
        return numberOfCowboys <= 0;
    }

    public void CowboyShot()
    {
        numberOfCowboys--;
        if (SuccessCheck())
        {
            Win();
        }
    }
}
