using UnityEngine;

public interface IPositionTracker : ITracker
{
    public void OnPositionChanged(Vector2 iVec2);

    public void OnStartTracking(Vector2 iVec2);

    public void OnStopTracking(Vector2 iVec2);
}
