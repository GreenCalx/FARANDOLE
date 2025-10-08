using UnityEngine;
using UnityEngine.UI.Extensions;
public static class DrawUtils
{

    public static void DrawUICircle(UILineRenderer iLR, float iRadius, int iResolution = 64)
    {
        float angle_step = Mathf.PI * 2f / iResolution;
        float angle = 0f;
        Vector3 pos = Vector3.zero;
        iLR.Points = new Vector2[iResolution + 1];
        for (int i = 0; i < iResolution; i++)
        {
            angle = i * angle_step;
            pos = new Vector3(
                iRadius * Mathf.Cos(angle),
                iRadius * Mathf.Sin(angle),
                0f);
            iLR.Points[i] = pos;
        }
        // bool loop
        iLR.Points[iLR.Points.Length - 1] = iLR.Points[0];
    }
}