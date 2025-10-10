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

    public List<GameObject> GetFromGameMode(GAME_MODE iGameMode)
    {
        switch (iGameMode)
        {
            case GAME_MODE.DAILY_SEED:
                return GetRandom(GameData.GetSettings.loopSize);
            case GAME_MODE.MUTATION:
                return GetDaily(GameData.GetSettings.loopSize);
            case GAME_MODE.CUSTOM:
                return GetRandom(GameData.GetSettings.loopSize);
            default:
                return GetFromGameMode(GAME_MODE.DAILY_SEED);
        }
    }
    public List<GameObject> GetRandom(int iNumber)
    {
        return GetRandomSelectionFromPool(GameBank, iNumber);
    }

    public List<GameObject> GetDaily(int iNumber)
    {
        // TODO : Fetch remote seed here
        return GetRandomSelectionFromPool(GameBank,iNumber);
    }

    public List<GameObject> GetByTags(int iNumber, List<MINIGAME_TAGS> iTags)
    {
        List<MiniGameSO> filtered = new List<MiniGameSO>();
        foreach (MiniGameSO go in GameBank)
        {
            MiniGame as_mg = go.prefab_MiniGame.GetComponent<MiniGame>();
            if (as_mg == null)
                continue;
            foreach (MINIGAME_TAGS tag in iTags)
            {
                if (as_mg.descriptor.tags.Contains(tag))
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
