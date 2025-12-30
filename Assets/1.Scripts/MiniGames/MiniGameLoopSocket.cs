using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class MiniGameLoopSocket
{
    public MiniGame inst_miniGame;
    public List<IMiniGameMod> mods;
    public MiniGameLoopSocket(MiniGame iMiniGame)
    {
        inst_miniGame = iMiniGame;
        mods = new List<IMiniGameMod>();
    }

    public void Run()
    {
        inst_miniGame.gameObject.SetActive(true);
        inst_miniGame.IsInPostGame = false;
        inst_miniGame.successState = MiniGameSuccessState.PENDING;
    }

    public void Reset()
    {
        inst_miniGame.successState = MiniGameSuccessState.PENDING;
        inst_miniGame.IsInPostGame = false;
    }

    public bool IsValidated()
    {
        return (inst_miniGame.successState == MiniGameSuccessState.PASSED);
    }

    public void ApplyCompatibleMods()
    {
        foreach(IMiniGameMod mod in mods)
        {
            if (inst_miniGame.descriptor.compatibleMods.Contains(mod.AssociatedTag()))
                mod.ApplyOn(this);
        }
    }
}