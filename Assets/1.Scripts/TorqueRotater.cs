using UnityEngine;

public class TorqueRotater : MonoBehaviour, IPositionTracker
{
    public float RotSpeed = 10f;
    public bool invertTorque = true;
    public Rigidbody2D targetRB;
    float torque = 0f;
    bool goingLeft = false;
    bool directionChanged = false;
    public UIVisualSlider visualSlider;
    public float maxAngularVelocity = 10f;
    

    public void Init()
    {
        torque = 0f;
        targetRB.angularVelocity = 0f;
        targetRB.bodyType = RigidbodyType2D.Dynamic;

        directionChanged = false;
    }

    public void OnStartTracking(Vector2 iVec2)
    {
        targetRB.freezeRotation = false;
        targetRB.bodyType = RigidbodyType2D.Dynamic;

        Vector3 delta = new Vector3(iVec2.x, iVec2.y, 0f) - transform.position;
        if (delta.x > 0f)
        {
            goingLeft = false;
        }
        else if (delta.x < 0f)
        {
            goingLeft = true;
        }
        directionChanged = false;
        torque = Mathf.Clamp(delta.x, -1f, 1f);
    }

    public void OnPositionChanged(Vector2 iVec2)
    {
        Vector3 delta = new Vector3(iVec2.x, iVec2.y, 0f) - transform.position;

        if (delta.x > 0f)
        {
            if (goingLeft && !directionChanged)
            {
                directionChanged = true;
                goingLeft = false;
            }
        }
        else if (delta.x < 0f)
        {
            if (!goingLeft && !directionChanged)
            {
                directionChanged = true;
                goingLeft = true;
            }
        }
        torque = Mathf.Clamp(delta.x, -1f, 1f);
    }
    public void OnStopTracking(Vector2 iVec2)
    {
       
    }

    void ClampAngularVelocity()
    {
        targetRB.angularVelocity = Mathf.Clamp(targetRB.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    void Update()
    {
        visualSlider.UpdateVisual(torque);
    }

    void FixedUpdate()
    {
        if (directionChanged)
        {
            directionChanged = false;
            targetRB.angularVelocity = 0f;
            targetRB.AddTorque(torque * RotSpeed);
        }
        if (invertTorque)
            targetRB.AddTorque(torque * -RotSpeed);
        else
            targetRB.AddTorque(torque * RotSpeed);
        ClampAngularVelocity();
    } 

}
