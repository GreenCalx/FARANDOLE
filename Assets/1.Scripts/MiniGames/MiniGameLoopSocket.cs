using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class MiniGameLoopSocket
{
    public MiniGame inst_miniGame;
    public int alpha_channel = 0;
    public int beta_channel = 0;
    public MiniGameLoopSocket(MiniGame iMiniGame)
    {
        inst_miniGame = iMiniGame;
    }

    public void Run()
    {
        ApplyMutation();
        
        inst_miniGame.gameObject.SetActive(true);
        inst_miniGame.IsInPostGame = false;
        inst_miniGame.successState = MiniGameSuccessState.PENDING;

        inst_miniGame.Play();
    }

    public void Stop()
    {
        inst_miniGame.IsInPostGame = true;
        inst_miniGame.Stop();
        inst_miniGame.gameObject.SetActive(false);
    }

    public void Reset()
    {
        inst_miniGame.successState = MiniGameSuccessState.PENDING;
        inst_miniGame.IsInPostGame = false;
    }

    public void ResetChannels()
    {
        alpha_channel = 0;
        beta_channel = 0;
    }

    public bool IsValidated()
    {
        return (inst_miniGame.successState == MiniGameSuccessState.PASSED);
    }

    public void ApplyMutation()
    {
        // Reset mutation values to base
        foreach (var ext in inst_miniGame.extensions)
            ext.ResetMutation();

        // Apply Alpha mutation
        inst_miniGame.AlphaMut?.ApplyMutation(alpha_channel);
        inst_miniGame.BetaMut?.ApplyMutation(beta_channel);
    }
}