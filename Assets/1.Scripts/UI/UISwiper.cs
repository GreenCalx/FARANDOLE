using UnityEngine;
using UnityEngine.Events;

public class UISwiper : MonoBehaviour, ISwipeTracker
{
    public bool vertical = true;
    public bool horizontal = false;
    public UnityEvent SwipeCallback;
    public void OnHorizontalSwipe(float iXVal)
    {
        if (!horizontal)
            return;
        SwipeCallback?.Invoke();
    }
    public void OnVerticalSwipe( float iYVal)
    {
        if (!vertical)
            return;
        SwipeCallback?.Invoke();
    }

}
