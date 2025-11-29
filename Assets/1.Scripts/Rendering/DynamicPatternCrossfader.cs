using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using static Utils;
public class DynamicPatternCrossfader : MonoBehaviour
{
    [Header("Tweaks")]
    public float fading_duration = 1f;

    [Header("Internals")]
public Material renderedMat;
    readonly string shadParmLerp = "_LerpPatternAB";
    [Range(0f, 1f)]
    public float crossfader = 0f;
    public DynamicPatternSO patternA;
    public DynamicPatternSO patternB;
    private int fadeDir = 0;
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
        renderedMat.SetVector("_PatternOffset", new Vector4(lerpOffset.x, lerpOffset.y, 0f, 0f) );

        Vector4 truchetAngles = Vector4.Lerp(patternA.truchetAngles, patternB.truchetAngles, crossfader);
        Vector4 tAngles = new Vector4( truchetAngles.x, truchetAngles.y, truchetAngles.z, truchetAngles.w );
        renderedMat.SetVector("_TruchetRotations", tAngles);

        _faderLock = false;
    }

    public bool TrySetNewCrossfadeTarget(DynamicPatternSO iNewPattern)
    {
        if (!_faderLock)
        {
            _faderLock = true;
            if (crossfader >= 1f)
            {
                patternA = iNewPattern;
                renderedMat.SetInt("_PatternA", (int)iNewPattern.pattern);
                fadeDir = -1;
            }
            else
            {
                patternB = iNewPattern;
                renderedMat.SetInt("_PatternB", (int)iNewPattern.pattern);
                fadeDir = 1;
            }
            return true;
        }
        fadeDir = 0;
        return false;
    }

    public void SetToNewPattern(DynamicPatternSO iNewPattern)
    {
        Crossfade(1f);
        _faderLock = false;
        fadeDir = 0;
    }
    
    public void Crossfade(float iLerp)
    {
        if (fadeDir == 0)
            return;
        crossfader = fadeDir > 0 ? iLerp : 1f - iLerp;
        crossfader = Mathf.Clamp01(crossfader);
        renderedMat.SetFloat("_LerpPatternAB", crossfader);
        renderedMat.SetFloat("_Angle", Utils.Lerp(patternA.angle, patternB.angle, crossfader));
        renderedMat.SetFloat("_BoxSize", Utils.Lerp(patternA.boxSize, patternB.boxSize, crossfader));
        renderedMat.SetFloat("_Tiling", Utils.Lerp(patternA.tiling, patternB.tiling, crossfader));
        Vector2 lerpOffset = Vector2.Lerp(patternA.offset, patternB.offset, crossfader);
        renderedMat.SetVector("_PatternOffset", new Vector4( lerpOffset.x, lerpOffset.y, 0, 0 ));
        Vector4 truchetAngles = Vector4.Lerp(patternA.truchetAngles, patternB.truchetAngles, crossfader);
        Vector4 tAngles = new Vector4( truchetAngles.x, truchetAngles.y, truchetAngles.z, truchetAngles.w );
        renderedMat.SetVector("_TruchetRotations", tAngles);
    }
}
