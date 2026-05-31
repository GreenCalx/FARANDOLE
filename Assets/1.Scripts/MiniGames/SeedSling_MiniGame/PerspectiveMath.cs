using UnityEngine;

/// Pure projection helpers mirroring PerspectiveRoom's near->far row lerp.
/// xFrac: 0..1 across a row's width. depthFrac: 0 (near plane) .. 1 (far plane).
public static class PerspectiveMath
{
    public static Rect RowAt(Rect near, Rect far, float depthFrac)
    {
        float t = Mathf.Clamp01(depthFrac);
        return new Rect(
            Mathf.Lerp(near.xMin, far.xMin, t),
            Mathf.Lerp(near.yMin, far.yMin, t),
            Mathf.Lerp(near.width, far.width, t),
            Mathf.Lerp(near.height, far.height, t)
        );
    }

    public static Vector3 GroundPointAt(Rect near, Rect far, float nearZ, float farZ,
                                        float xFrac, float depthFrac)
    {
        float t = Mathf.Clamp01(depthFrac);
        Rect row = RowAt(near, far, t);
        float x = Mathf.Lerp(row.xMin, row.xMax, Mathf.Clamp01(xFrac));
        float y = row.yMin;
        float z = Mathf.Lerp(nearZ, farZ, t);
        return new Vector3(x, y, z);
    }

    public static float ScaleAt(float nearScale, float farScale, float depthFrac)
        => Mathf.Lerp(nearScale, farScale, Mathf.Clamp01(depthFrac));
}
