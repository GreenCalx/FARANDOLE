using UnityEngine;
public interface ITapTracker : ITracker
{
    public void OnTap(Vector2 iVec2);
}