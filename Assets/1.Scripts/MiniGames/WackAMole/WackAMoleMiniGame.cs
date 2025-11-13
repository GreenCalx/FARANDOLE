using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using TMPro;
public class WackAMoleMiniGame : MiniGame, ISpawnerMod<Mole>, ITheaterMod
{
    [Header("WackAMoleGame")]

    public GameObject prefab_mole;
    public GameObject prefab_PerspectiveRoom;
    public TextMeshProUGUI UIWorld_targetCounter;
    public AnimationCurve moleOutAnimCurve;
    public AnimationCurve moleBackInAnimCurve;
    private Mole[,] inst_moleMatrix;

    public float spawnInterval = 0.5f;
    public int[] colSizes;
    private int colSize;
    public int[] rowSizes;
    private int rowSize;
    public int[] targets;
    private int target;
    public float[] maxOutDuration;
    public float[] minOutDuration;
    PerspectiveRoom inst_PerspectiveRoom;

    private float[,] m_SpawnProbabilities;
    private float elapsedSpawnInterval = 0f;
    private bool gameIsWon = false;
    #region MiniGame
    public override void Init()
    {
        //Reset();

    }

    public override void Reset()
    {
        if (GetTags().Contains(EMiniGameTags.SPAWNER))
            Debug.Log("Spawner Mini game !");
            
        colSize = colSizes[MGM.miniGamesDifficulty - 1];
        rowSize = rowSizes[MGM.miniGamesDifficulty - 1];
        target = targets[MGM.miniGamesDifficulty - 1];

        // room
        inst_PerspectiveRoom = GOBuilder.Create(prefab_PerspectiveRoom)
                                .WithParent(transform)
                                .BuildAs<PerspectiveRoom>();
        inst_PerspectiveRoom.Init(MGM.LM2D, rowSize+1);
        inst_PerspectiveRoom.Build();
        inst_PerspectiveRoom.InitRoomPlacer(colSize, true);

        UpdateMGUI();

        inst_moleMatrix = new Mole[rowSize, colSize];
        m_SpawnProbabilities = new float[rowSize, colSize];
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < colSize; j++)
            {
                m_SpawnProbabilities[i, j] = UnityEngine.Random.Range(0f, 1f);

                Mole newMole = Spawn(prefab_mole);
                inst_moleMatrix[i, j] = newMole;
                inst_PerspectiveRoom.AddToRoom(newMole.transform, i+1, newMole.GetRenderer());

                newMole.transform.position = new Vector3(
                    inst_PerspectiveRoom.GetXRowLerp(i, inst_PerspectiveRoom.m_RoomRowPlacer.At(j)),
                    newMole.transform.position.y,
                    newMole.transform.position.z);
                newMole.matrixCoordinates = new Vector2Int(i, j);
                newMole.OnHideCB.AddListener((x,y)=>m_SpawnProbabilities[x,y] = 0f);
            }
        }

        // ensure at least one probability is 1 so we have a mole spawn right away
        m_SpawnProbabilities[ UnityEngine.Random.Range(0, rowSize), UnityEngine.Random.Range(0,colSize)] = 1f;

        inst_PerspectiveRoom.PlaceAllOnLayers();

        gameIsWon = false;
    }
    public override void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public override void Stop()
    {
        Clean();
        IsActiveMiniGame = false;
        IsInPostGame = false;
    }
    public override void Win()
    {
        //Clean();
        gameIsWon = true;
        foreach(Mole mole in inst_moleMatrix)
            mole.OnWin();
        MGM.WinMiniGame();
    }
    public override void Lose()
    {
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        if (GameIsLost)
            return false;
        return target <= 0;
    }
    #endregion

    #region ISpawnerMod

    public void ApplyMod()
    {
        // TODO : Change spawn Interval or spawn probability random range
    }
    public Mole Spawn(GameObject iPrefab)
    {
        Mole currMole = GOBuilder.Create(iPrefab)
            .WithParent(transform)
            .WithName("Mole ")
            .BuildAs<Mole>();
        currMole.Init(UnityEngine.Random.Range( minOutDuration[MGM.miniGamesDifficulty - 1], 
                                          maxOutDuration[MGM.miniGamesDifficulty - 1]
                                          ));
        PC.AddTapTracker(currMole);

        currMole.tapCB.AddListener(MoleWhacked);
        
        return currMole;
    }
    #endregion

    void Update()
    {
        if (IsActiveMiniGame && !IsInPostGame && !gameIsWon)
        {
            elapsedSpawnInterval += Time.deltaTime;
            if (elapsedSpawnInterval < spawnInterval)
                return;
            AccumulateProbabilities();
            elapsedSpawnInterval = 0f;
        }
    }

    private void AccumulateProbabilities()
    {
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < colSize; j++)
            {
                m_SpawnProbabilities[i,j] += UnityEngine.Random.Range(0f,1f);

                if (m_SpawnProbabilities[i, j] >= 1f)
                {
                    inst_moleMatrix[i, j].GoOut();
                }
            }
        }
    }

    private void MoleWhacked()
    {
        target -= 1;
        UpdateMGUI();
        if (target <= 0)
        {
            Win();
        }
    }

    public void UpdateMGUI()
    {
        UIWorld_targetCounter.text = (target).ToString();
    }

    public void Clean()
    {
        Debug.Log("Mole Clean");
        foreach (Mole mole in inst_moleMatrix)
        {
            if (mole != null)
            {
                PC.RemoveTapTracker(mole);
                Destroy(mole.gameObject);
            }
        }

        if (inst_PerspectiveRoom != null)
        {
            inst_PerspectiveRoom.Clean();
            Destroy(inst_PerspectiveRoom.gameObject);
        }

    }
}