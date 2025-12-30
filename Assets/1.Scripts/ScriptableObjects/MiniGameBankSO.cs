using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static Utils;

[CreateAssetMenu(fileName = "MiniGameBankSO", menuName = "Scriptable Objects/MiniGameBankSO")]
public class MiniGameBankSO : ScriptableObject
{
    public List<MiniGameSO> GameBank;
    public int Size => GameBank.Count;

    public List<GameObject> GetRandom(int iNumber)
    {
        return GetRandomSelectionFromPool(GameBank, iNumber);
    }


    public List<GameObject> GetFromSeeds()
    {
        List<GameObject> filtered = new List<GameObject>();
        foreach (MiniGameSO go in GameBank)
        {
            if (GameData.Get.MiniGameSeeds.Contains(go.ID))
            {
                filtered.Add(go.prefab_MiniGame);
            }
        }
        return filtered;
    }

    public List<GameObject> GetByFamily(int iNumber, List<EMiniGameFamily> iTags)
    {
        List<MiniGameSO> filtered = new List<MiniGameSO>();
        foreach (MiniGameSO go in GameBank)
        {
            MiniGame as_mg = go.prefab_MiniGame.GetComponent<MiniGame>();
            if (as_mg == null)
                continue;
            foreach (EMiniGameFamily tag in iTags)
            {
                if (iTags.Contains(as_mg.descriptor.family))
                {
                    filtered.Add(go);
                    break;
                }
            }
        }
        return GetRandomSelectionFromPool(filtered, iNumber);
    }

    List<GameObject> GetRandomSelectionFromPool(List<MiniGameSO> iBankSO, int iNumber)
    {
        int poolSize = iBankSO.Count;
        if (iNumber >= poolSize)
        {
            List<GameObject> shuffled = iBankSO.Select(e => e.prefab_MiniGame).ToList();
            shuffled.Shuffle(); // less item in bank than requested number, just give all we can.
            return shuffled;
        }

        List<GameObject> retval = new List<GameObject>(iNumber);
        List<int> pickedGames = new List<int>(iNumber);
        int selected = 0;
        for (int i = 0; i < iNumber; i++)
        {
            selected = UnityEngine.Random.Range(0, poolSize);
            while (pickedGames.Contains(selected))
            {
                selected = UnityEngine.Random.Range(0, poolSize);
            }
            pickedGames.Add(selected);
            retval.Add(iBankSO[selected].prefab_MiniGame);
        }
        return retval;
    }
}
