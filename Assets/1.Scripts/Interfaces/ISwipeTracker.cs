using UnityEngine;

public interface ISwipeTracker : ITracker
{
    public bool enabled {get; set;}
    public void OnHorizontalSwipe(float iXVal);
    public void OnVerticalSwipe( float iYVal);
}