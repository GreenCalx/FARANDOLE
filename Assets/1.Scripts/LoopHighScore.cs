using UnityEngine;

[System.Serializable]
public class LoopHighScore : ISaveLoad
{
    public GAME_MODE gameMode;
    public byte[] ids;
    public int score;

    public LoopHighScore()
    { }
    public LoopHighScore(GAME_MODE iGameMode, byte[] iIDs, int iScore)
    {
        ids = iIDs;
        score = iScore;
        gameMode = iGameMode;
    }
    public object GetData()
    {
        return this;
    }
}
