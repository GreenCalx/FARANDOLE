using UnityEngine;

public class StreamPour : MonoBehaviour
{
    public float pourStartAngle = 45f;
    public float maxPourStrength = 6f;
    public bool invertDirection = false;

    public bool IsPouring { get; private set; }
    public float PourStrength { get; private set; }
    public float CurrentAngle { get; private set; }

    void Update()
    {
        float rawAngle = transform.eulerAngles.z;
        if (rawAngle > 180f) rawAngle -= 360f;

        CurrentAngle = invertDirection ? -rawAngle : rawAngle;

        if (CurrentAngle >= pourStartAngle)
        {
            IsPouring = true;
            float t = Mathf.InverseLerp(pourStartAngle, 90f, CurrentAngle);
            PourStrength = Mathf.Clamp01(t) * maxPourStrength;
        }
        else
        {
            IsPouring = false;
            PourStrength = 0f;
        }
    }
}
