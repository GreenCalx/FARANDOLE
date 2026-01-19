using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Splines;

[CreateAssetMenu(fileName = "PathDef", menuName = "MiniGameExtensions/PathDef")]
[Serializable]
public class PathDef : ScriptableObject
{
    [Tooltip("Prefab that implements IPath")]
    public GameObject prefab;
    public SplineContainer splines;

    [Header("tweaks")]
    public float pathTravelTime = 1f;
}
