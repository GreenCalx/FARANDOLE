using UnityEngine;

[CreateAssetMenu(fileName = "DynamicPatternSO", menuName = "Scriptable Objects/DynamicPatternSO")]
public class DynamicPatternSO : ScriptableObject
{
    public enum EDynamicPattern { Checker = 0, Boxes = 1, Truchet = 2 }
    public EDynamicPattern pattern;
    [Header("Generics")]
    public float angle;
    public float boxSize;
    public float tiling;
    public Vector2 offset;
    [Header("Truchet")]
    public Vector4 truchetAngles;
}
