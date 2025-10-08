using UnityEngine;
using System;

[System.Serializable]
public class LoopHighScore : ISaveLoad
{
    public DateTime dateTime;
    public GAME_MODE gameMode;
    public byte[] ids;
    public int score;

    public LoopHighScore()
    { }
    public LoopHighScore(GAME_MODE iGameMode, byte[] iIDs, int iScore, DateTime iDateTime)
    {
        ids = iIDs;
        score = iScore;
        gameMode = iGameMode;
        dateTime = iDateTime;
    }
    public object GetData()
    {
        return this;
    }
}
