using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
public class WaterPourMiniGame : MiniGame
{
    public TorqueRotater rotater;
    public PourToCup pourToCup;
    public StreamFilled cup;
    public float[] fillOverLevels;
    public float[] maxFillRateOverLevels;
    // TODO : Move the cup

    public override void Init()
    {
        base.Init();
    }
    public override void Reset()
    {
        base.Reset();

        rotater.Init();
        
        cup.Flush();
        cup.maxFill = fillOverLevels[MGM.LoopRank];
        pourToCup.fillRate = maxFillRateOverLevels[MGM.LoopRank];

        PC.AddPositionTracker(rotater);
        pourToCup.OnCupFilledCB.AddListener(() => {Win();} );
    }
    public override void Play()
    {
        base.Play();
    }
    public override void Stop()
    {
        base.Stop();
        PC.RemovePositionTracker(rotater);
        pourToCup.OnCupFilledCB.RemoveAllListeners();
    }
    public override void Win()
    {
        base.Win();
        PC.RemovePositionTracker(rotater);
        pourToCup.OnCupFilledCB.RemoveAllListeners();
    }
    public override void Lose()
    { // not used atm
        base.Lose();
    }
    public override bool SuccessCheck()
    {
        return base.SuccessCheck();
    }

    public override async UniTask IntroAnim(CancellationToken token)
    {
        return;
    }

    #region MODS
    
    // public void ApplyMod()
    // {

    // }

    #endregion
}
