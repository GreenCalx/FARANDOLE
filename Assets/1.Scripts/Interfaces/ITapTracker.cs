using UnityEngine;
public interface ITapTracker : ITracker
{
    bool stopPropagation { get; }
    int GetDisplayPriority();

    public bool OnTap(Vector2 iVec2);
}