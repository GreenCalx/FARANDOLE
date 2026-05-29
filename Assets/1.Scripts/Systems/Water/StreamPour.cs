using UnityEngine;

public class StreamPour : MonoBehaviour
{
    public float pourStartAngle = 45f;
    public float maxPourStrength = 6f;

    public bool IsPouring { get; private set; }
    public float PourStrength { get; private set; }

    void Update()
    {
        float angle = NormalizeAngle(transform.eulerAngles.z);

        if (angle >= pourStartAngle)
        {
            IsPouring = true;
            float t = Mathf.InverseLerp(pourStartAngle, 90f, angle);
            PourStrength = t * maxPourStrength;
        }
        else
        {
            IsPouring = false;
            PourStrength = 0f;
        }
    }

    float NormalizeAngle(float a)
    {
        if (a > 180f) a -= 360f;
        return Mathf.Abs(a);
    }
}
