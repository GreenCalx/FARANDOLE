using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class CowboyMiniGame : MiniGame
{
    [Header("CowboyMiniGame")]

    public GameObject prefab_cowboy;
    public Cowboy[] inst_cowboys;

    public float[] difficultyAcceleration;
    public float[] difficultySpriteScale;
    public int[] cowboysPerDifficulty;

    public Transform[] spawnerTransform;
    private int numberOfCowboys;

    public override void Init()
    {
        numberOfCowboys = cowboysPerDifficulty[MGM.miniGamesDifficulty - 1];
        inst_cowboys = new Cowboy[numberOfCowboys];
        for (int i = 0; i < numberOfCowboys; i++)
        {
            inst_cowboys[i] = GOBuilder.Create(prefab_cowboy)
                .WithName("Cowboy" + i)
                .WithPosition(spawnerTransform[i].position)
                .BuildAs<Cowboy>();
            inst_cowboys[i].difficultyTimeCoef = difficultyAcceleration[i];
            PC.AddTapTracker(inst_cowboys[i]);
            inst_cowboys[i].hitCB.AddListener(CowboyShot);
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
                cowboy.DestroySelf();           
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
