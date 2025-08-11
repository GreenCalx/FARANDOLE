using UnityEngine;
using System;
using System.Collections.Generic;
public static class Utils
{
    public static Vector2 GetWorldPos(Vector3 iScreenPos)
    {
        Vector3 proj = Camera.main.ScreenToWorldPoint(iScreenPos);
        return new Vector2(proj.x, proj.y);
    }

    public static Vector2 GetWorldPos(Vector2 iScreenPos)
    {
        Vector3 proj = Camera.main.ScreenToWorldPoint(new Vector3(iScreenPos.x, iScreenPos.y, 0f));
        return new Vector2(proj.x, proj.y);
    }

    public static Vector2 GetFullScreenWorldSize()
    {
        if ((Screen.orientation == ScreenOrientation.LandscapeLeft) || (Screen.orientation == ScreenOrientation.LandscapeRight))
            return new Vector2(Camera.main.orthographicSize * 2f, Camera.main.orthographicSize);
        return new Vector2(Camera.main.orthographicSize, Camera.main.orthographicSize * 2f);
    }

    public static void UnwrapMesh(Mesh m)
    {
        List<Vector2> uvs = new List<Vector2>(m.vertexCount);
        foreach (Vector3 v in m.vertices)
        {
            uvs.Add(new Vector2(v.x, v.y));
        }
        m.SetUVs(0, uvs);
    }

    public static float CauchySample(float iX0, float iQ, float iXSample)
    {
        float f_den = Mathf.PI * iQ * (1 + Mathf.Pow((iXSample - iX0) / iQ, 2));
        return 1f / f_den;
    }
    public static void CauchyToAnimCurve(ref AnimationCurve ioCurve, float iX0, float iQ)
    {
        ioCurve.ClearKeys();

        int n_steps = 10;
        for (int i = 0; i <= n_steps; i++)
        {
            float x_curve = (float)i / (float)n_steps;
            float y_curve = CauchySample(iX0, iQ, x_curve);

            int key_idx = ioCurve.AddKey(x_curve, y_curve);
            ioCurve.SmoothTangents(key_idx, 0f);
        }
    }

    public static T GetComp<T>(GameObject iGO)
    {
        T comp = iGO.GetComponent<T>();
        if (comp != null)
            return comp;
        return iGO.GetComponentInParent<T>();
    }

    public static T GetCompInParent<T>(GameObject iGO)
    {
        T ret = iGO.GetComponentInParent<T>();
        if (ret != null)
            return ret;
        Transform parent = iGO.transform.parent;
        while (parent != null)
        {
            ret = parent.gameObject.GetComponent<T>();
            if (ret != null)
                break;
            parent = parent.parent;
        }

        return ret;
    }

    public static float Lerp(float a, float b, float f)
    {
        return a * (1f - f) + (b * f);
    }

    public static float Remap(float iVal, float iOldMin, float iOldMax, float iNewMin, float iNewMax)
    {
        return iNewMin + ((iVal - iOldMin) / (iOldMax - iOldMin)) * (iNewMax - iNewMin);
    }

    public static bool IsNaN(Vector3 iVec)
    {
        return (float.IsNaN(iVec.x) || float.IsNaN(iVec.y) || float.IsNaN(iVec.z));
    }

    public static void Split<T>(T[] iSourceArr, int index, out T[] left, out T[] right)
    {
        int rlen = iSourceArr.Length - index;
        left = new T[index];
        right = new T[rlen];

        Array.Copy(iSourceArr, 0, left, 0, index);
        Array.Copy(iSourceArr, index, right, 0, rlen);
    }

    public static void Shuffle<T>(this IList<T> iList)
    {
        int n = iList.Count;
        int last = n - 1;
        for (int i = 0; i < last; i++)
        {
            int rand = UnityEngine.Random.Range(i, n);
            T prev = iList[i];
            iList[i] = iList[rand];
            iList[rand] = prev;
        }
    }
}
