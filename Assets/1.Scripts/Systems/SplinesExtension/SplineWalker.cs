using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
public class SplineWalker : MonoBehaviour
{
    private SplineContainer SC;
    public SplinePathRenderer selfPathRenderer;
    public void SetSpline(SplineContainer iSC)
    {
        if (iSC==null)
            return;
        SC = iSC;
        SC.gameObject.SetActive(true);
        selfPathRenderer.splineContainer = SC;
        selfPathRenderer.Rebuild();
        UpdatePosition(0f);
        //SC.CalculateSplineLength();
    }

    public void UpdatePosition(float iPercent)
    {
        if (SC==null)
            return;
        float percent = Mathf.Clamp01(iPercent);
        float3 position = transform.position;
        float3 tang = Vector3.zero;
        float3 up = Vector3.zero;
        if (SC.Evaluate(percent, out position, out tang, out up))
            transform.position = position;
    }

}
