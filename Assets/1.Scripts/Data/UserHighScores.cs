using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
    public void RemoveHighScore(LoopHighScore iLHS)
    {
        if (highScores == null)
            return;
        if (!highScores.Contains(iLHS))
            return;
        highScores.Remove(iLHS);
    }

    public List<LoopHighScore> GetGameModeHighScores(GAME_MODE iMode)
    {
        return highScores.Where(e => (e.gameMode == iMode)).ToList();
    }
}
