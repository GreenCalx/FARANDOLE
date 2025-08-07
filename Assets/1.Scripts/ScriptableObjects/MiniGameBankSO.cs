using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MiniGameBankSO", menuName = "Scriptable Objects/MiniGameBankSO")]
public class MiniGameBankSO : ScriptableObject
{
    public List<GameObject> GameBank;

    public List<GameObject> GetFromGameMode(GAME_MODE iGameMode)
    {
        switch (iGameMode)
        {
            case GAME_MODE.RANDOM_SEED:
                return GetRandom(GameData.GetSettings.loopSize);
            case GAME_MODE.DAILY_SEED:
                return GetDaily(GameData.GetSettings.loopSize);
            case GAME_MODE.SPRINT:
                return GetRandom(GameData.GetSettings.loopSize);
            default:
                return GetRandom(GameData.GetSettings.loopSize);
        }
    }
    public List<GameObject> GetRandom(int iNumber)
    {
        return GetRandomSelectionFromPool(GameBank, iNumber);
    }

    public List<GameObject> GetDaily(int iNumber)
    {
        // TODO : impl
        return new List<GameObject>(0);
    }

    public List<GameObject> GetByTags(int iNumber, List<GAMETYPE_TAG> iTags)
    {
        List<GameObject> filtered = new List<GameObject>();
        foreach (GameObject go in GameBank)
        {
            MiniGame as_mg = go.GetComponent<MiniGame>();
            if (as_mg == null)
                continue;
            foreach (GAMETYPE_TAG tag in iTags)
            {
                if (as_mg.tags.Contains(tag))
                {
                    filtered.Add(go);
                    break;
                }
            }
        }
        return GetRandomSelectionFromPool(filtered, iNumber);
    }

    List<GameObject> GetRandomSelectionFromPool(List<GameObject> iPool, int iNumber)
    {
        int poolSize = iPool.Count;
        if (iNumber >= poolSize)
        {
            return iPool; // less item in bank than requested number, just give all we can.
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
            pickedGames[i] = selected;
            retval[i] = iPool[selected];
        }
        return retval;
    }
}
