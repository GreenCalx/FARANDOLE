using UnityEngine;

public interface ISwipeTracker : ITracker
{
    public void OnHorizontalSwipe(float iXVal);
    public void OnVerticalSwipe( float iYVal);
}