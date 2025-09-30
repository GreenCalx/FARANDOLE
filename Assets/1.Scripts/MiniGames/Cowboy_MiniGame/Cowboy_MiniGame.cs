using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;

public class CowboyMiniGame : MiniGame
{
    [Header("CowboyMiniGame")]

    public GameObject prefab_cowboy;

    public GameObject prefab_obstacle;

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
            MGM.LM2D.PlaceObject(inst_cowboys[i].cowboySR);
        }
        PositionCowboysAndObstacles();
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
            maxX = PG.bounds.max.x * Mathf.Pow(1 - perspectiveMarginDelta,i);
            minX = PG.bounds.min.x * Mathf.Pow(1 - perspectiveMarginDelta,i);
            posY = PG.bounds.min.y + (y_delta * (i + 1) - cowboyHalfHeight);
            scale = 1 - (perspectiveShrinkDelta * i);

            inst_cowboys[i].transform.position = new Vector3(0, posY, 0);
            inst_cowboys[i].SetMovement(minX, maxX);
            inst_cowboys[i].transform.DOScale(scale, 0);
            n_obstacles = Random.Range(0, maxObstaclesNumber + 1);
            for (int j = 0; j < n_obstacles; j++)
            {
                inst_obstacles[obstacle_index] = GOBuilder.Create(prefab_obstacle)
                    .WithName("Obstacle" + obstacle_index)
                    .WithParent(this.transform)
                    .WithPosition(new Vector3(Random.Range(minX, maxX), posY + obstacle_offset, 0))
                    .BuildAs<Obstacle>();
                inst_obstacles[obstacle_index].transform.DOScale(scale, 0);
                inst_obstacles[obstacle_index].transform.DOLocalMoveZ(0, 0); // sinon Z = 0.016
                PC.AddTapTracker(inst_obstacles[obstacle_index]);
                MGM.LM2D.PlaceObject(inst_obstacles[obstacle_index].sr);
                obstacle_index++;
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
        Clean();
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
