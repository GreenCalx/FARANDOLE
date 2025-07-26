using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MiniGameManager : MonoBehaviour, IManager
{
    [Header("MGM Set")]
    public List<GameObject> prefab_miniGames;
    [Header("Internals")]
    public List<MiniGame> miniGames;
    public float currGameClock;
    public int miniGamesDifficulty;
    int currIndex = -1;
    public UnityEvent<float> OnHPLossCB;
    public UnityEvent<int> OnScoreGainCB;
    public UnityEvent OnLoopComplete;
    float gameStartTime;
    PlayerController PC;
    Playground PG;

    #region IManager
    public void Init(GameManager iGameManager)
    {
        miniGamesDifficulty = 1;
        currIndex = 0;
        OnHPLossCB = new UnityEvent<float>();
        PC = iGameManager.PC;
        PG = iGameManager.PG;

        WarmUpMiniGames();
        //Next();
    }
    public bool IsReady()
    {
        return true;
    }
    #endregion

    public void WarmUpMiniGames()
    {
        miniGames = new List<MiniGame>();
        foreach (GameObject prefab in prefab_miniGames)
        {
            GameObject new_mg = Instantiate(prefab);
            MiniGame as_mg = new_mg.GetComponent<MiniGame>();
            if (as_mg == null)
                return;
            as_mg.MGM = this;
            as_mg.PC = PC;
            as_mg.PG = PG;
            miniGames.Add(as_mg);
            new_mg.SetActive(false);
        }
    }

    public void Play()
    {
        miniGames[currIndex].gameObject.SetActive(true);
        miniGames[currIndex].Init();

        miniGames[currIndex].Play();
        currGameClock = miniGames[currIndex].gameClock;
        gameStartTime = Time.time;
    }

    public void Stop()
    {
        miniGames[currIndex].Stop();
        miniGames[currIndex].gameObject.SetActive(false);
        PC.ClearAllTrackers();
    }

    public void WinMiniGame()
    {
        OnScoreGainCB.Invoke(1);
        Next();
    }

    void Next()
    {
        if (currIndex >= 0)
        {
            Stop();
        }


        if (++currIndex >= miniGames.Count)
        {
            OnLoopComplete.Invoke();
            currIndex = 0;
        }
        Play();
    }

    public float GetRemainingTime()
    {
        return Mathf.Clamp(currGameClock - (Time.time - gameStartTime),0f, currGameClock);
    }

    public void RaiseDifficulty()
    {
        if (miniGamesDifficulty >= GameData.Get.gameSettings.MaxMiniGameDifficulty)
            return;
        miniGamesDifficulty++;
    }

    void Update()
    {
        if (Time.time - gameStartTime > currGameClock)
        {
            // Lose hp
            OnHPLossCB.Invoke(Time.deltaTime);
        }
    }

}
