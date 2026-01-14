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
        ApplyCompatibleMods();
        
        inst_miniGame.gameObject.SetActive(true);
        inst_miniGame.IsInPostGame = false;
        inst_miniGame.successState = MiniGameSuccessState.PENDING;

        inst_miniGame.Play();
    }

    public void Reset()
    {
        inst_miniGame.successState = MiniGameSuccessState.PENDING;
        inst_miniGame.IsInPostGame = false;
    }

    public void ResetMods()
    {
        if (mods!=null)
            mods.Clear();
    }

    public bool IsValidated()
    {
        return (inst_miniGame.successState == MiniGameSuccessState.PASSED);
    }

    public void AddMod(IMiniGameMod iMGMod)
    {
        mods.Add(iMGMod);
    }

    public void RemoveMod(IMiniGameMod iMGMod)
    {
        mods.Remove(iMGMod);
    }

    public void ApplyCompatibleMods()
    {
        foreach(IMiniGameMod mod in mods)
        {
            if (inst_miniGame.descriptor.compatibleMods.Contains(mod.AssociatedTag()))
                mod.Apply(this);
        }
    }
}