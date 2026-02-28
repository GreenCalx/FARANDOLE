using UnityEngine;
using System.Collections.Generic;

public class PourStreamSimulator : MonoBehaviour
{
    [Header("References")]
    public Transform spout;
    public StreamPour teapot;
    public LayerMask cupLayer;

    [Header("Stream Physics")]
    public float baseExitSpeed = 2.5f;
    public float maxExitSpeed = 4f;
    public float gravity = 9.8f;
    public float streamDuration = 0.6f;

    [Header("Arc Shape")]
    [Range(0f, 90f)]
    public float minTiltAngle = 45f;
    [Range(0f, 90f)]
    public float maxTiltAngle = 90f;
    [Range(0f, 1f)]
    public float arcInfluence = 0.7f;
    public bool flipHorizontal = false;

    [Header("Simulation")]
    public int segments = 20;

    [Header("Internal view")]
    float lastStrength;
    float currentTiltAngle;
    Vector2 exitDirection;

    public float HitT { get; private set; } = 1f;
    public Vector2 HitPoint { get; private set; }
    public Vector2 HitNormal { get; private set; }

    public bool IsHittingCup { get; private set; }

#if UNITY_EDITOR
    public bool debugDraw = true;
    readonly List<Vector2> debugPoints = new();
#endif

public bool Simulate(float strength01)
{
    lastStrength = strength01;
    IsHittingCup = false;
    HitT = 1f;

    CalculateExitDirection();

#if UNITY_EDITOR
    debugPoints.Clear();
#endif

    Vector2 prev = spout.position;

    for (int i = 1; i <= segments; i++)
    {
        float t = i / (float)segments;
        Vector2 pos = GetPoint(t);

#if UNITY_EDITOR
        debugPoints.Add(pos);
#endif

        RaycastHit2D hit = Physics2D.Linecast(prev, pos, cupLayer);
        if (hit)
        {
            IsHittingCup = true;
            HitT = t;
            HitPoint = hit.point;
            HitNormal = hit.normal;
            return true;
        }

        prev = pos;
    }

    return false;
}

void CalculateExitDirection()
{
    currentTiltAngle = teapot != null ? teapot.CurrentAngle : 0f;

    float tiltRatio = Mathf.InverseLerp(minTiltAngle, maxTiltAngle, currentTiltAngle);

    float horizontalComponent = Mathf.Lerp(1f, 0.2f, tiltRatio * arcInfluence);
    float verticalComponent = Mathf.Lerp(0.3f, 1f, tiltRatio);

    float sign = flipHorizontal ? -1f : 1f;
    exitDirection = new Vector2(horizontalComponent * sign, -verticalComponent).normalized;
}

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!debugDraw)
            return;

        if (spout != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(spout.position, (Vector2)spout.position + exitDirection * 0.5f);
        }

        if (debugPoints.Count < 2)
            return;

        Gizmos.color = IsHittingCup ? Color.green : Color.red;

        Vector2 prev = spout != null ? (Vector2)spout.position : debugPoints[0];
        for (int i = 0; i < debugPoints.Count; i++)
        {
            Gizmos.DrawLine(prev, debugPoints[i]);
            Gizmos.DrawSphere(debugPoints[i], 0.02f);
            prev = debugPoints[i];
        }
    }
#endif

public Vector2 GetPoint(float t)
{
    Vector2 start = spout.position;

    float speed = Mathf.Lerp(baseExitSpeed, maxExitSpeed, lastStrength);
    float time = t * streamDuration;

    float x = exitDirection.x * speed * time;
    float y = exitDirection.y * speed * time - 0.5f * gravity * time * time;

    return start + new Vector2(x, y);
}

}
