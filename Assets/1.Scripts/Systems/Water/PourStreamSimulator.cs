using UnityEngine;
using System.Collections.Generic;

public class PourStreamSimulator : MonoBehaviour
{
    [Header("References")]
    public Transform spout;
    public Transform pourDirection;
    public LayerMask cupLayer;

    [Header("Stream Shape")]
    public float ballisticDistance = 3.5f;
    public float controlledDistance = 2.0f;
    public float ballisticGravity = 0.6f;
    public float controlledGravity = 3.5f;
    public int segments = 16;

    [Header("Internal view")]
    float lastStrength;
    // stream hits
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


    static Vector2 QuadraticBezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * b + t * t * c;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!debugDraw || debugPoints.Count < 2)
            return;

        Gizmos.color = IsHittingCup ? Color.green : Color.red;

        for (int i = 0; i < debugPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(debugPoints[i], debugPoints[i + 1]);
            Gizmos.DrawSphere(debugPoints[i], 0.02f);
        }
    }
#endif

public Vector2 GetPoint(float t)
{
    Vector2 start = spout.position;
    Vector2 dir = pourDirection.right.normalized;

    float gravity = Mathf.Lerp(ballisticGravity, controlledGravity, lastStrength);
    float distance = Mathf.Lerp(ballisticDistance, controlledDistance, lastStrength);

    Vector2 end =
        start +
        dir * distance +
        Vector2.down * gravity;

    Vector2 control =
        start +
        dir * distance * 0.5f +
        Vector2.down * gravity * Mathf.Lerp(0.1f, 0.35f, lastStrength);

    float u = 1f - t;
    return u * u * start + 2f * u * t * control + t * t * end;
}

}
