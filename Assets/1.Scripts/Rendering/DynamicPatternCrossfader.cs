using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using static Utils;
public class DynamicPatternCrossfader : MonoBehaviour
{
    readonly string shadParmLerp = "_LerpPatternAB";
    [Range(0f, 1f)]
    public float crossfader = 0f;
    public DynamicPatternSO patternA;
    public DynamicPatternSO patternB;
    [Header("Internals")]
    public Material renderedMat;
    private bool _faderLock = false;

    public void Init(Material iTargetMat, DynamicPatternSO iPatternA, DynamicPatternSO iPatternB)
    {
        renderedMat = iTargetMat;
        patternA = iPatternA;
        patternB = iPatternB;

        // Init rendererd Mat with crossfader at 0
        crossfader = 0f;

        renderedMat.SetInt("_PatternA", (int)patternA.pattern);
        renderedMat.SetFloat("_LerpPatternAB", crossfader);
        renderedMat.SetFloat("_Angle", Utils.Lerp(patternA.angle, patternB.angle, crossfader));
        renderedMat.SetFloat("_BoxSize", Utils.Lerp(patternA.boxSize, patternB.boxSize, crossfader));
        renderedMat.SetFloat("_Tiling", Utils.Lerp(patternA.tiling, patternB.tiling, crossfader));
        Vector2 lerpOffset = Vector2.Lerp(patternA.offset, patternB.offset, crossfader);
        renderedMat.SetFloatArray("_Offset", new float[]{patternA.offset.x, patternA.offset.y} );

        _faderLock = false;
    }
    public void FadeToNewPattern(DynamicPatternSO iNewPattern)
    {
        if (!_faderLock)
        {
            _faderLock = true;
            if (crossfader >= 1f)
            {
                patternA = iNewPattern;
                GoToA();
            } else {
                patternB = iNewPattern;
                GoToB();
            }
        }
    }

    async UniTaskVoid GoToA()
    {
        renderedMat.SetInt("_PatternA", (int)patternA.pattern);
        await CrossfadeTask(1f, 0f, 1f);
    }

    async UniTaskVoid GoToB()
    {
        renderedMat.SetInt("_PatternB", (int)patternB.pattern);
        await CrossfadeTask(0f, 1f, 1f);
    }

    async UniTask CrossfadeTask(float from, float to, float duration)
    {
        float frac = 0f;
        float startTime = Time.time;
        crossfader = from;
        while (frac < 1f)
        {
            frac = (Time.time - startTime) / duration;
            crossfader = Utils.Lerp(from, to, frac);

            renderedMat.SetFloat("_LerpPatternAB", crossfader);
            renderedMat.SetFloat("_Angle", Utils.Lerp(patternA.angle, patternB.angle, crossfader));
            renderedMat.SetFloat("_BoxSize", Utils.Lerp(patternA.boxSize, patternB.boxSize, crossfader));
            renderedMat.SetFloat("_Tiling", Utils.Lerp(patternA.tiling, patternB.tiling, crossfader));
            Vector2 lerpOffset = Vector2.Lerp(patternA.offset, patternB.offset, crossfader);
            renderedMat.SetFloatArray("_Offset", new float[]{lerpOffset.x, lerpOffset.y} );
            await Task.Yield();
        }
        crossfader = to;
        _faderLock = false;
    }
}
