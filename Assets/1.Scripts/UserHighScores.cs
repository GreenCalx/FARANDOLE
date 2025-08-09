using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UserHighScores
{
    public List<LoopHighScore> highScores;

    public UserHighScores()
    {
        highScores = new List<LoopHighScore>();
    }
    public void AddHighScore(LoopHighScore iLHS)
    {
        if (highScores == null)
            highScores = new List<LoopHighScore>();
        highScores.Add(iLHS);
    }
}
