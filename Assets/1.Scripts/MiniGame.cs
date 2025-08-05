using UnityEngine;
using UnityEngine.Events;

public class MiniGame : MonoBehaviour, IMiniGame
{
    public MiniGameManager MGM;
    public PlayerController PC;
    public PlaygroundManager PG;
    public bool IsActiveMiniGame;
    // Might want to implement a full FSM..
    public bool IsInPostGame;

    public virtual void Init()
    {

    }
    public virtual void Play()
    {
        IsActiveMiniGame = true;
        IsInPostGame = false;
    }
    public virtual void Stop()
    {
        IsActiveMiniGame = false;
        IsInPostGame = false;
    }
    public virtual void Win()
    {
        IsInPostGame = true;
    }
    public virtual void Lose()
    {
        IsInPostGame = false;
    }
    public virtual bool SuccessCheck()
    {
        return false;
    }
}
