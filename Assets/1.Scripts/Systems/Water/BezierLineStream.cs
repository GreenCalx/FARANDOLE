using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierLineStream : MonoBehaviour
{
    public PourStreamSimulator stream;
    public StreamPour teapot;

    public float minWidth = 0.02f;
    public float maxWidth = 0.08f;
    public float minFlowSpeed = 2f;
    public float maxFlowSpeed = 4f;

    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    void LateUpdate()
    {
        if (!teapot.IsPouring)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;

        float endT = stream.IsHittingCup ? stream.HitT : 1f;
        //float endT = Mathf.Max(0f, stream.HitT - 0.02f);

        int pointCount = Mathf.Max(2, Mathf.CeilToInt(stream.segments * endT) + 1);
        lr.positionCount = pointCount;

        float width = Mathf.Lerp(minWidth, maxWidth, teapot.PourStrength);
        lr.startWidth = width;
        lr.endWidth = width * 0.8f;

        for (int i = 0; i < pointCount; i++)
        {
            float t = (i / (float)(pointCount - 1)) * endT;
            lr.SetPosition(i, stream.GetPoint(t));
        }

        //lr.material.SetFloat("_FlowSpeed", Mathf.Lerp(4f, 2f, teapot.PourStrength));
    }
}
