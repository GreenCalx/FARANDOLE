using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
/// <summary>
/// Generic util methods
/// should stay decorrelated from any game logic. 
/// </summary>
public static class Utils
{
    public static Vector2[] ToVec2(Vector3[] iVec3)
    {
        Vector2[] oVec2 = new Vector2[iVec3.Length];
        for (int i = 0; i < iVec3.Length; i++)
        { oVec2[i] = new Vector2(iVec3[i].x, iVec3[i].y); }
        return oVec2;
    }

    public static Vector3[] ToVec3(Vector2[] iVec2)
    {
        Vector3[] oVec3 = new Vector3[iVec2.Length];
        for (int i = 0; i < iVec2.Length; i++)
        { oVec3[i] = new Vector3(iVec2[i].x, iVec2[i].y, 0f); }
        return oVec3;
    }

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

    public static void BoundedUnwrapMesh(Mesh m)
    {
        Vector3[] vertices = m.vertices;
        Vector2[] uvs = m.uv;

        // find smallest & highest x/y
        float maxX = 0f, minX = 0f;
        float maxY = 0f, minY = 0f;
        for (int v = 0; v < vertices.Length; v++)
        {
            if (maxX < vertices[v].x)
                maxX = vertices[v].x;
            if (minX > vertices[v].x)
                minX = vertices[v].x;
            if (maxY < vertices[v].y)
                maxY = vertices[v].y;
            if (minY > vertices[v].y)
                minY = vertices[v].y;
        }

        // remap uv on a square
        // thus we just need to remap uv according to its vertex pos remaped between prev min/max
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            Vector2 new_uv = new Vector2(
                Utils.Remap(v.x, minX, maxX, 0f, 1f),
                Utils.Remap(v.y, minY, maxY, 0f, 1f)
            );
            uvs[i] = new_uv;
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

    public static bool IsContained2D(Vector2 iPos, Bounds iBounds)
    {
        return iBounds.Contains(new Vector3(iPos.x, iPos.y, iBounds.center.z));
    }

    public static Vector2 Uniform2DVec(float iXY)
    {
        return new Vector2(iXY, iXY);
    }

    public static Vector2 RandomInsideDisc(float iRadius, float iExclusionRadius)
    {
        Vector2 randPoint = UnityEngine.Random.insideUnitCircle * iRadius;
        if (iExclusionRadius <= 0f)
        {
            return randPoint; // asking for a circle
        }
        float exclusionRadius = Mathf.Clamp(iExclusionRadius, 0f, iRadius);
        if (randPoint.magnitude < exclusionRadius)
        {
            float randX = UnityEngine.Random.Range(iExclusionRadius - randPoint.magnitude, iRadius - randPoint.magnitude);
            if (randPoint.x > 0f)
                randPoint.x += randX;
            else
                randPoint.x -= randX;

            float randY = UnityEngine.Random.Range(iExclusionRadius - randPoint.magnitude, iRadius - randPoint.magnitude);
            if (randPoint.y > 0f)
                randPoint.y += randY;
            else
                randPoint.y -= randY;
        }
        return randPoint;
    }

    public static int GetTodaySeed()
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        int rawseed = 0;
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(today));
            rawseed = Mathf.Abs(BitConverter.ToInt32(hash, 0)); 
        }
        return rawseed;
    }

}
