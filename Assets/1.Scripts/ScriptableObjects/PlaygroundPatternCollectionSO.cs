using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlaygroundPatternCollectionSO", menuName = "Scriptable Objects/PlaygroundPatternCollectionSO")]
public class PlaygroundPatternCollectionSO : ScriptableObject
{
    public List<DynamicPatternSO> patterns;
    public int Count => patterns.Count;
}
