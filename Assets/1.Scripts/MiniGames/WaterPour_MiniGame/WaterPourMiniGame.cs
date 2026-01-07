using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Splines;
public class WaterPourMiniGame : MiniGame
{
    public TorqueRotater rotater;
    public PourToCup pourToCup;
    public StreamFilled cup;
    public SplineWalker cupWalker;
    public float[] fillOverLevels;
    public float[] maxFillRateOverLevels;
    public SplineContainer[] splinesOverLevels;

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
        cupWalker.SetSpline(splinesOverLevels[MGM.LoopRank]);

        pourToCup.OnCupFilledCB.AddListener(() => {Win();} );
        cup.OnLiquidFillCB.AddListener(() => cupWalker.UpdatePosition(cup.FillRatio));

        PC.AddPositionTracker(rotater);
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
