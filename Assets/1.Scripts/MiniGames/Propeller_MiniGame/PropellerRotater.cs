using UnityEngine;

/// <summary>
/// Rotates a transform directly following the finger's circular movement around its center.
/// Only allows rotation in one direction (configurable).
/// </summary>
public class PropellerRotater : MonoBehaviour, IPositionTracker
{
    // True  = clockwise only   (finger must go clockwise  to produce rotation)
    // False = counterclockwise (finger must go counter-CW to produce rotation)
    public bool clockwise = true;

    public float AngularSpeed { get; private set; }

    private bool isTracking;
    private float lastRawAngle;
    private float cumulativeAngle;

    public bool enabled { get; set; } = true;

    public void Init()
    {
        isTracking = false;
        cumulativeAngle = 0f;
        AngularSpeed = 0f;
        transform.rotation = Quaternion.identity;
    }

    public void OnStartTracking(Vector2 pos)
    {
        isTracking = true;
        Vector2 dir = pos - (Vector2)transform.position;
        lastRawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public void OnPositionChanged(Vector2 pos)
    {
        Vector2 dir = pos - (Vector2)transform.position;
        float rawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float delta = Mathf.DeltaAngle(lastRawAngle, rawAngle);
        lastRawAngle = rawAngle;

        // In Unity 2D: positive delta = counterclockwise, negative delta = clockwise
        bool isClockwise = delta < 0f;
        if (clockwise != isClockwise)
        {
            AngularSpeed = 0f;
            return;
        }

        cumulativeAngle += delta;
        transform.rotation = Quaternion.Euler(0f, 0f, cumulativeAngle);
        AngularSpeed = Mathf.Abs(delta) / Time.deltaTime;
    }

    public void OnStopTracking(Vector2 pos)
    {
        isTracking = false;
        AngularSpeed = 0f;
    }
}
