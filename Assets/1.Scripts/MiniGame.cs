using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum SCREEN_ORIENTATION
{
    PORTRAIT = 0,
    LANDSCAPE = 1,
    BOTH = 2
}

public enum GAMETYPE_TAG
{
    ANY = 0,
    AGILITY = 1,
    REFLEX = 2,
    BRAIN = 3,
    SCIENCE = 4
}

public class MiniGame : MonoBehaviour, IMiniGame
{
    [Header("MiniGame")]
    public MiniGameManager MGM;
    public PlayerController PC;
    public PlaygroundManager PG;
    public bool IsActiveMiniGame;
    // Might want to implement a full FSM..
    public bool IsInPostGame;
    public SCREEN_ORIENTATION orientationRequirement = SCREEN_ORIENTATION.PORTRAIT;
    public List<GAMETYPE_TAG> tags;

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
