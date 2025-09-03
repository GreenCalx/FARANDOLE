using UnityEngine;

public class PlayerData
{
    public float HP;
    public int score;
    public float timeScale;
    public PlayerData()
    {
        HP = GameData.Get.gameSettings.PlayerHP;
        score = 0;
        timeScale = 1f;

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
