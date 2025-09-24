using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using TMPro;
public class WackAMoleMiniGame : MiniGame
{
    [Header("WackAMoleGame")]
    public GameObject prefab_mole;
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




    public override void Init()
    {
        colSize = colSizes[MGM.miniGamesDifficulty - 1];
        rowSize = rowSizes[MGM.miniGamesDifficulty - 1];
        float tempMinHoleDuration = minHoleDuration[MGM.miniGamesDifficulty - 1];
        float tempMaxHoleDuration = maxHoleDuration[MGM.miniGamesDifficulty - 1];
        target = targets[MGM.miniGamesDifficulty - 1];

        UpdateMGUI();
        float delta_x = PG.bounds.size.x / (rowSize + 1);
        float delta_y = PG.bounds.size.y / (colSize + 1);

        inst_moles = new Mole[colSize * rowSize];
        for (int j = 0; j < colSize; j++)
        {
            for (int i = 0; i < rowSize; i++)
            {
                inst_moles[j * rowSize + i] = GOBuilder.Create(prefab_mole)
                    .WithParent(transform)
                    .WithLocalPosition(new Vector3(PG.bounds.min.x + (delta_x * (i + 1)), PG.bounds.max.y - (delta_y * (j + 1))))
                    .WithName("mole" + i + "" + j)
                    .BuildAs<Mole>();
                inst_moles[j * rowSize + i].Init(tempMinHoleDuration, tempMaxHoleDuration);
                PC.AddTapTracker(inst_moles[j * rowSize + i]);
                MGM.LM2D.PlaceObject(inst_moles[j * rowSize + i].GetComponent<SpriteRenderer>());

                inst_moles[j * rowSize + i].tapCB.AddListener(MoleWhacked);
            }
        }
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
        Clean();
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
        foreach (Mole mole in inst_moles)
        {
            if (mole != null)
            {
                PC.RemoveTapTracker(mole);
                Destroy(mole.gameObject);
            }
        }
    }
}