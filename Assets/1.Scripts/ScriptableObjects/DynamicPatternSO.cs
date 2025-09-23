using UnityEngine;

[CreateAssetMenu(fileName = "DynamicPatternSO", menuName = "Scriptable Objects/DynamicPatternSO")]
public class DynamicPatternSO : ScriptableObject
{
    public enum EDynamicPattern { Checker = 0, Boxes = 1 }
    public EDynamicPattern pattern;
    public float angle;
    public float boxSize;
    public float tiling;
    public Vector2 offset;
}
