using UnityEngine;

public class MiniGame : MonoBehaviour, IMiniGame
{
    public MiniGameManager MGM;
    public PlayerController PC;
    public PlaygroundManager PG;
    public bool IsActiveMiniGame;
    public float gameClock = 5f;
    public virtual void Init()
    {

    }
    public virtual void Play()
    {
        IsActiveMiniGame = true;
    }
    public virtual void Stop()
    {
        IsActiveMiniGame = false;
    }
    public virtual void Win()
    {

    }
    public virtual void Lose()
    {

    }
    public virtual bool SuccessCheck()
    {
        return false;
    }
}
