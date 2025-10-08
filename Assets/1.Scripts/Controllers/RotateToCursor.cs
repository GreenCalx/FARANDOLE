using UnityEngine;

public class RotateToCursor : MonoBehaviour, IPositionTracker
{
    public float RotSpeed = 10f;
    float angle;
    Quaternion currRot;
    bool isTracking = true;
    public void OnStartTracking(Vector2 iVec2)
    {
        isTracking = true;
        currRot = transform.rotation;
    }
    public void OnPositionChanged(Vector2 iVec2)
    {
        Vector3 delta = new Vector3(iVec2.x, iVec2.y, 0f);
        if (delta.magnitude > 0.01f)
        {
            angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            currRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void OnStopTracking(Vector2 iVec2)
    {
        isTracking = false;
    }

    void Update()
    {
        if (isTracking)
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * RotSpeed);
    }
}
