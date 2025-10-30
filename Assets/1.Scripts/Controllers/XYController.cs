using UnityEngine;
using UnityEngine.Events;
using static Utils;
public class XYController : MonoBehaviour, IPositionTracker
{
    public Transform knob;
    public bool IsTracking = false;
    public CircleCollider2D h_xyControllerC2D;
    public UnityEvent<Vector2> PositionChangedCB;
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
        if (!IsTracking)
            return;
        knob.position = transform.position + Vector3.ClampMagnitude(iVec2 - anchor, 1f);
        PositionChangedCB?.Invoke(XY);
    }

    public void OnStartTracking(Vector2 iVec2)
    {
        if (!Utils.IsContained2D(iVec2, h_xyControllerC2D.bounds))
        {
            IsTracking = false;
            return;
        }
        IsTracking = true;
    }

    public void OnStopTracking(Vector2 iVec2)
    {
        IsTracking = false;
    }
}
