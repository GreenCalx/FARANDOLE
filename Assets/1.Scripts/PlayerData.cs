using UnityEngine;

public class PlayerData
{
    public float HP;
    public int score;
    public float timeScale;
    public int loopLevel = 0;
    public PlayerData()
    {
        HP = 10f;
        score = 0;
        timeScale = 1f;
        loopLevel = 0;
    }

    public void LoseHP(float iLostHP)
    {
        HP -= iLostHP;
    }

    public void AddScore(int iScore)
    {
        score += iScore;
    }
    
}
