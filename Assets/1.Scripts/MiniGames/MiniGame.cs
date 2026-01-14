using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using static LocalUtils;

public enum MiniGameSuccessState
{
    NONE = 0,
    PENDING = 1,
    PASSED = 2,
    FAILED = 3
}

public enum EMiniGameFamily
{
    REGULAR=0,
    DOG=1,
    CHESS=2,
    ARCADE=4,
    PHYSICS=8
}
public enum EMiniGameMods
{
    NONE=0,
    SPAWNER=1,
    EXTENDABLE=2,
    CHESSBOARD=3,
    BALL=4
}

public abstract class MiniGame : MonoBehaviour, IMiniGame
{
    [Header("MiniGame Mand")]
    public MiniGameSO descriptor;
    [SerializeReference, SerializeField]
    public List<MiniGameExtension> extensions;
    private Dictionary<Type, MiniGameExtension> _extensionCache = null;
    private void BuildExtensionCache()
    {
        _extensionCache = new Dictionary<Type, MiniGameExtension>();

        foreach (var ext in extensions)
        {
            var type = ext.GetType();
            _extensionCache[type] = ext;

            foreach (var iface in type.GetInterfaces())
            {
            if (LocalUtils.IsMiniGameExtensionInterface(iface))
            {
                _extensionCache[iface] = ext;
            }
            }
        }
    }

    public T GetExtension<T>()
    {
        if (_extensionCache == null)
            BuildExtensionCache();

        return _extensionCache.TryGetValue(typeof(T), out var ext)
            ? (T)(object)ext
            : default;
    }

    [Header("MiniGame Internal View")]
    public MiniGameManager MGM;
    public PlayerController PC;
    public PlaygroundManager PG;
    public bool IsActiveMiniGame;
    // Might want to implement a full FSM..
    public bool IsInPostGame;
    public bool GameIsLost;
    public MiniGameSuccessState successState;
    public float CompletionTime = 0f;
    protected CancellationTokenSource introCTS;


    // Use Reflection to retrieve all interface deriving from IMiniGame for current MiniGame
    private static readonly MethodInfo AssociatedTagMethod =
        typeof(IMiniGameMod).GetMethod(nameof(IMiniGameMod.AssociatedTag));
    public List<EMiniGameFamily> GetTags()
    {
        List<EMiniGameFamily> retval = new List<EMiniGameFamily>();
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
                    EMiniGameFamily tag = (EMiniGameFamily)method.Invoke(this, null);
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
        CompletionTime = 0f;
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
        MGM.WinMiniGame();
        IsInPostGame = true;
    }
    public virtual void Lose()
    {
        GameIsLost = false;
    }
    public virtual bool SuccessCheck()
    {
        if (GameIsLost)
            return false;
        return false;
    }

    public virtual async UniTask IntroAnim(CancellationToken token)
    {
        return;
    }
}
