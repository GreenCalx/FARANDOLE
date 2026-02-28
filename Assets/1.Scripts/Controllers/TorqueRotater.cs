using UnityEngine;

public class TorqueRotater : MonoBehaviour, IPositionTracker
{
    public Rigidbody2D targetRB;
    public float maxAngularSpeed = 180f;
    public float rotationGain = 5f;
    public float damping = 0.9f;

    [Header("Angle Limits")]
    public bool useAngleLimits = true;
    public float minAngle = -45f;
    public float maxAngle = 90f;
    public float limitBounceFactor = 0.3f;

    [Header("Ballistic")]
    public float angularDrag = 2f;
    public float releaseVelocityScale = 0.8f;

    private bool isTracking;
    private float initialRotation;

    private float rawTargetAngle;
    private float rawInitAngle;
    private float lastRawTargetAngle;
    private float cumulativeRawTargetAngle;

    private float lastRawRotAngle;
    private float cumilativeRotAngle;
    private float currentVelocity;

    public void Init()
    {
        isTracking = false;
        targetRB.freezeRotation = true;
        targetRB.bodyType = RigidbodyType2D.Kinematic;
        targetRB.angularVelocity = 0f;
        cumilativeRotAngle = 0f;
        targetRB.rotation = 0f;
        lastRawTargetAngle = 0f;
        lastRawRotAngle = 0f;
        rawTargetAngle = 0f;
        currentVelocity = 0f;
    }

    public void OnStartTracking(Vector2 pos)
    {
        isTracking = true;
        targetRB.freezeRotation = false;
        targetRB.bodyType = RigidbodyType2D.Dynamic;

        initialRotation = targetRB.rotation;
        cumilativeRotAngle = initialRotation;

        Vector2 center = transform.position;
        Vector2 dirTarget = pos - center;

        rawTargetAngle = Mathf.Atan2(dirTarget.y, dirTarget.x) * Mathf.Rad2Deg;
        rawInitAngle = rawTargetAngle;
        lastRawTargetAngle = rawTargetAngle;
        cumulativeRawTargetAngle = rawTargetAngle;
    }

    public void OnPositionChanged(Vector2 pos)
    {
        Vector2 center = transform.position;
        Vector2 dirTarget = pos - center;
        rawTargetAngle = Mathf.Atan2(dirTarget.y, dirTarget.x) * Mathf.Rad2Deg;

        float smallDelta = Mathf.DeltaAngle(lastRawTargetAngle, rawTargetAngle);
        cumulativeRawTargetAngle += smallDelta;
        lastRawTargetAngle = rawTargetAngle;
    }

    public void OnStopTracking(Vector2 pos)
    {
        isTracking = false;
        currentVelocity = targetRB.angularVelocity * releaseVelocityScale;
    }

    void FixedUpdate()
    {
        if (targetRB == null)
            return;

        float dt = Time.fixedDeltaTime;

        if (isTracking)
        {
            float dRb = targetRB.rotation - lastRawRotAngle;
            cumilativeRotAngle += dRb;
            lastRawRotAngle = targetRB.rotation;

            float targetAngle = initialRotation + (cumulativeRawTargetAngle - rawInitAngle);

            if (useAngleLimits)
                targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

            float angleDiff = targetAngle - cumilativeRotAngle;

            float desiredAngularVelocity = angleDiff * rotationGain;
            desiredAngularVelocity = Mathf.Lerp(targetRB.angularVelocity, desiredAngularVelocity, 1f - damping);
            desiredAngularVelocity = Mathf.Clamp(desiredAngularVelocity, -maxAngularSpeed, maxAngularSpeed);

            targetRB.angularVelocity = desiredAngularVelocity;
            currentVelocity = desiredAngularVelocity;
        }
        else
        {
            currentVelocity -= currentVelocity * angularDrag * dt;

            if (Mathf.Abs(currentVelocity) < 0.1f)
                currentVelocity = 0f;

            float newRotation = targetRB.rotation + currentVelocity * dt;

            if (useAngleLimits)
            {
                if (newRotation < minAngle)
                {
                    newRotation = minAngle;
                    currentVelocity = -currentVelocity * limitBounceFactor;
                }
                else if (newRotation > maxAngle)
                {
                    newRotation = maxAngle;
                    currentVelocity = -currentVelocity * limitBounceFactor;
                }
            }

            targetRB.rotation = newRotation;
            cumilativeRotAngle = newRotation;
            lastRawRotAngle = newRotation;
        }
    }
}
