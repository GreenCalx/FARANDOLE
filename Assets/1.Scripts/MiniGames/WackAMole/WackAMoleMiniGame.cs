using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using TMPro;
public class WackAMoleMiniGame : MiniGame
{
    [Header("WackAMoleGame")]

    public GameObject prefab_mole;
    public GameObject prefab_PerspectiveRoom;
    public TextMeshProUGUI UIWorld_targetCounter;

    private Mole[] inst_moles;

    public int[] colSizes;
    private int colSize;
    public int[] rowSizes;
    private int rowSize;
    public int[] targets;
    private int target;
    public float[] maxHoleDuration;
    public float[] minHoleDuration;
    PerspectiveRoom inst_PerspectiveRoom;

    public override void Init()
    {
        //Reset();
    }

    public override void Reset()
    {


        colSize = colSizes[MGM.miniGamesDifficulty - 1];
        rowSize = rowSizes[MGM.miniGamesDifficulty - 1];
        float tempMinHoleDuration = minHoleDuration[MGM.miniGamesDifficulty - 1];
        float tempMaxHoleDuration = maxHoleDuration[MGM.miniGamesDifficulty - 1];
        target = targets[MGM.miniGamesDifficulty - 1];

        // room
        inst_PerspectiveRoom = GOBuilder.Create(prefab_PerspectiveRoom)
                                .WithParent(transform)
                                .BuildAs<PerspectiveRoom>();
        inst_PerspectiveRoom.Init(MGM.LM2D, rowSize+1);
        inst_PerspectiveRoom.Build();
        inst_PerspectiveRoom.InitRoomPlacer(colSize, true);

        UpdateMGUI();
        //float delta_x = PG.bounds.size.x / (rowSize + 1);
        //float delta_y = PG.bounds.size.y / (colSize + 1);
        

        inst_moles = new Mole[colSize * rowSize];
        for (int j = 0; j < colSize; j++)
        {
            for (int i = 0; i < rowSize; i++)
            {
                Mole currMole = GOBuilder.Create(prefab_mole)
                    .WithParent(transform)
                    .WithName("mole" + i + "" + j)
                    .BuildAs<Mole>();
                currMole.Init(tempMinHoleDuration, tempMaxHoleDuration);
                PC.AddTapTracker(currMole);
                inst_PerspectiveRoom.AddToRoom(currMole.transform, i+1, currMole.GetRenderer());

                currMole.transform.position = new Vector3(
                    inst_PerspectiveRoom.GetXRowLerp(i, inst_PerspectiveRoom.m_RoomRowPlacer.At(j)),
                    currMole.transform.position.y,
                    currMole.transform.position.z);
                currMole.tapCB.AddListener(MoleWhacked);

                inst_moles[j * rowSize + i] = currMole;
            }
        }

        inst_PerspectiveRoom.PlaceAllOnLayers();
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
        MGM.WinMiniGame();
    }
    public override void Lose()
    {
        Clean();
        IsInPostGame = false;
    }
    public override bool SuccessCheck()
    {
        return target <= 0;
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
        foreach (Mole mole in inst_moles)
        {
            if (mole != null)
            {
                PC.RemoveTapTracker(mole);
                Destroy(mole.gameObject);
            }
        }

        if (inst_PerspectiveRoom!=null)
        {
            inst_PerspectiveRoom.Clean();
            Destroy(inst_PerspectiveRoom.gameObject);    
        }
        
    }
}