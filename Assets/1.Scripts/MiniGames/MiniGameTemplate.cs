using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
/// <summary>
/// Supposed to be deadcode
/// > only used to create new minigames
///  do not include this file to builds
/// </summary>

#if UNITY_EDITOR
public class MiniGameTemplate : MiniGame
{
    public override void Init()
    {
        base.Init();
    }
    public override void Reset()
    {
        base.Reset();
    }
    public override void Play()
    {
        base.Play();
    }
    public override void Stop()
    {
        base.Stop();
    }
    public override void Win()
    {
        base.Win();
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
#endif