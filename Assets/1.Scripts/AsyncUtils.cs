using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using static Utils;

public static class AsyncUtils
{
    public static async UniTask LerpTask(float iFrom, float iTo, float iDuration, Action<float> iLerpCallback, CancellationToken iTok)
    {
        float startTime = Time.realtimeSinceStartup;
        float frac = 0f;
        while (frac < 1f)
        {
            frac = (Time.realtimeSinceStartup - startTime) / iDuration;
            iLerpCallback?.Invoke( Lerp(iFrom, iTo, frac) );
            if (iTok.IsCancellationRequested)
                return;
            await UniTask.Yield();
        }
    }
}
