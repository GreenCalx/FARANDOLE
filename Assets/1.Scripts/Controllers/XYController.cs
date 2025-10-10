using UnityEngine;

public class XYController : MonoBehaviour, IPositionTracker
{
    public Transform knob;
    public float maxRadius = 1f;
    public Vector2 anchor
    {
        get { return new Vector2(transform.position.x, transform.position.y); }
    }
    public Vector2 XY
    {
        get { return new Vector2(knob.position.x,knob.position.y ) - anchor;  }
    }
    public void Reset()
    {
        knob.localPosition = Vector3.zero;
    }
    public void OnPositionChanged(Vector2 iVec2)
    {
        knob.position = transform.position + Vector3.ClampMagnitude(iVec2 - anchor, 1f);
    }

    public void OnStartTracking(Vector2 iVec2)
    {

    }

    public void OnStopTracking(Vector2 iVec2)
    {

    }
}
