using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplinePathRenderer : MonoBehaviour
{
    [Header("Source")]
    public SplineContainer splineContainer;

    [Header("Shape")]
    public int resolution = 32;
    public float width = 0.1f;

    Mesh mesh;

    // Cached buffers (no allocations after Awake)
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    void Awake()
    {
        mesh = new Mesh
        {
            name = "SplinePathMesh"
        };

        GetComponent<MeshFilter>().sharedMesh = mesh;

        AllocateBuffers();
    }

    void AllocateBuffers()
    {
        vertices = new Vector3[resolution * 2];
        uvs = new Vector2[resolution * 2];
        triangles = new int[(resolution - 1) * 6];
    }

    /// <summary>
    /// Call this ONCE whenever the spline or path visibility changes.
    /// </summary>
    public void Rebuild()
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
            return;

        var spline = splineContainer.Splines[0];

        float totalLength = 0f;
        Vector3 prev = Vector3.zero;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);

            Vector3 worldCenter =
                splineContainer.transform.TransformPoint(
                    spline.EvaluatePosition(t)
                );

            Vector3 center = transform.InverseTransformPoint(worldCenter);

            Vector3 worldTangent =
                splineContainer.transform.TransformDirection(
                    spline.EvaluateTangent(t)
                ).normalized;

            Vector3 tangent =
                transform.InverseTransformDirection(worldTangent).normalized;

            // World-space 2D normal (perpendicular in XY plane)
            Vector3 normal = Vector3.Cross(tangent, Vector3.forward).normalized;



            if (i > 0)
                totalLength += Vector3.Distance(prev, center);

            prev = center;

            int v = i * 2;
            vertices[v]     = center + normal * (width * 0.5f);
            vertices[v + 1] = center - normal * (width * 0.5f);

            // Temporarily store distance in UV.y (normalized later)
            uvs[v]     = new Vector2(0f, totalLength);
            uvs[v + 1] = new Vector2(1f, totalLength);

            if (i < resolution - 1)
            {
                int tIdx = i * 6;
                triangles[tIdx]     = v;
                triangles[tIdx + 1] = v + 2;
                triangles[tIdx + 2] = v + 1;

                triangles[tIdx + 3] = v + 1;
                triangles[tIdx + 4] = v + 2;
                triangles[tIdx + 5] = v + 3;
            }
        }

        // Normalize UV.y → 0..1
        if (totalLength > 0f)
        {
            float invLen = 1f / totalLength;
            for (int i = 0; i < uvs.Length; i++)
                uvs[i].y *= invLen;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

#if UNITY_EDITOR
    // Optional: rebuild automatically in editor when values change
    void OnValidate()
    {
        if (Application.isPlaying)
            Rebuild();
    }
#endif
}
