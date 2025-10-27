using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public enum MiniGameSuccessState
{
    NONE = 0,
    PENDING = 1,
    PASSED = 2,
    FAILED = 3
}

public enum EMiniGameTags
{
    NONE = 0,
    REGULAR = 1,
    SPAWNER = 2,
    DOG = 3,
    EXTENDABLE = 4,
    ARCADE = 5,
    SCIENCE = 6,
    PHYSICS = 7,
    THEATER = 8,
    CHESS = 9
}

public class MiniGame : MonoBehaviour, IMiniGame
{

    [Header("MiniGame Mand")]
    public MiniGameSO descriptor;
    [Header("MiniGame Internal View")]
    public MiniGameManager MGM;
    public PlayerController PC;
    public PlaygroundManager PG;
    public bool IsActiveMiniGame;
    // Might want to implement a full FSM..
    public bool IsInPostGame;
    public MiniGameSuccessState successState;


    // Use Reflection to retrieve all interface deriving from IMiniGame for current MiniGame
    private static readonly MethodInfo AssociatedTagMethod =
        typeof(IMiniGameMod).GetMethod(nameof(IMiniGameMod.AssociatedTag));
    public List<EMiniGameTags> GetTags()
    {
        List<EMiniGameTags> retval = new List<EMiniGameTags>();
        foreach (Type tinterface in this.GetType().GetInterfaces())
        {
            if (typeof(IMiniGameMod).IsAssignableFrom(tinterface) && tinterface != typeof(IMiniGameMod))
            {
                MethodInfo method = tinterface.GetMethod(
                    AssociatedTagMethod.Name,
                    BindingFlags.Public | BindingFlags.Instance
                );
                if (method != null)
                {
                    EMiniGameTags tag = (EMiniGameTags)method.Invoke(this, null);
                    retval.Add(tag);
                }
            }
        }
        return retval;
    }

    public bool hasIntro = false;


    public virtual void Init()
    {
        successState = MiniGameSuccessState.NONE;
    }
    public virtual void Reset()
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

    public virtual async UniTask IntroAnim()
    {
        return;
    }
}
