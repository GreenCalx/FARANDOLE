using UnityEngine;

public class FoldPaper : ISwipeTracker
{
    Rect paper;
    
    public void OnHorizontalSwipe(float iXVal)
    {
        if (iXVal > 0f )
        {
            Debug.Log("Swipe Right");
            // fold origin.x to size.x
            // new origin at delta.x / 2 ?
        } else {
            Debug.Log("Swipe Left");
        }
    }
    public void OnVerticalSwipe( float iYVal)
    {
        if (iYVal > 0f )
        {
            Debug.Log("Swipe Up");
        } else {
            Debug.Log("Swipe Down");
        }
    }

    void Fold()
    {

    }
}
