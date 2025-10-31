using UnityEngine;

public static class MeshUtils
{
    public enum FacePlane
    {
        XY, XZ, YZ
    }
/// <summary>
/// Builds a mesh for an arbitrary planar quad (P0..P3 in any order).
/// The full texture rect (0..1 x 0..1) will be mapped to the quad.
/// Use flipU/flipV to adjust orientation if needed.
/// </summary>
public static Mesh QuadMesh(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, bool flipU = false, bool flipV = false)
{
    Mesh mesh = new Mesh();
    Vector3[] verts = new Vector3[] { p0, p1, p2, p3 };
    mesh.vertices = verts;
    mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };

    // Compute normal
    Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0);
    if (normal.sqrMagnitude < 1e-6f) normal = Vector3.up;
    normal.Normalize();
    mesh.normals = new Vector3[] { normal, normal, normal, normal };

    // Compute 4 edge vectors (in vertex order)
    Vector3 e01 = (p1 - p0).normalized;
    Vector3 e12 = (p2 - p1).normalized;
    Vector3 e23 = (p3 - p2).normalized;
    Vector3 e30 = (p0 - p3).normalized;

    // Opposite edge pairs: (e01, e23) and (e12, e30).
    // Choose the pair where edges are most parallel (largest dot).
    float pairA = Mathf.Abs(Vector3.Dot(e01, e23));
    float pairB = Mathf.Abs(Vector3.Dot(e12, e30));

    Vector3 uDir, vDir;
    if (pairA >= pairB)
    {
        // use average of e01 and e23 as U
        uDir = (e01 + e23) * 0.5f;
        if (uDir.sqrMagnitude < 1e-6f) uDir = e01;
        uDir = (uDir - Vector3.Dot(uDir, normal) * normal).normalized; // project to plane & normalize

        // v is orthogonal on plane
        vDir = Vector3.Cross(normal, uDir).normalized;
    }
    else
    {
        // use average of e12 and e30 as U (rotate)
        uDir = (e12 + e30) * 0.5f;
        if (uDir.sqrMagnitude < 1e-6f) uDir = e12;
        uDir = (uDir - Vector3.Dot(uDir, normal) * normal).normalized;
        vDir = Vector3.Cross(normal, uDir).normalized;
    }

    // Project verts into (uDir, vDir) coordinates and compute min/max
// Project vertices
Vector2[] proj = new Vector2[4];
for (int i = 0; i < 4; ++i)
{
    Vector3 rel = verts[i] - p0;
    proj[i] = new Vector2(Vector3.Dot(rel, uDir), Vector3.Dot(rel, vDir));
}

float minU = (proj[0].x + proj[2].x) * 0.5f;
float maxU = (proj[1].x + proj[3].x) * 0.5f;
float minV = (proj[0].y + proj[1].y) * 0.5f;
float maxV = (proj[2].y + proj[3].y) * 0.5f;


    // Normalize to 0..1
    float spanU = Mathf.Max(1e-6f, maxU - minU);
    float spanV = Mathf.Max(1e-6f, maxV - minV);
    Vector2[] uv = new Vector2[4];
    for (int i = 0; i < 4; ++i)
    {
        float nu = (proj[i].x - minU) / spanU;
        float nv = (proj[i].y - minV) / spanV;
        uv[i] = new Vector2(Mathf.Clamp01(nu), Mathf.Clamp01(nv));
        
    }

    mesh.uv = uv;
    mesh.RecalculateTangents();
    mesh.RecalculateBounds();
    return mesh;
}


    public static Mesh WallQuad_FullRect(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight)
{
    Mesh mesh = new Mesh();
    mesh.name = "WallQuad_FullRect";

    Vector3[] vertices = new Vector3[4] { bottomLeft, bottomRight, topLeft, topRight };
    mesh.vertices = vertices;

    mesh.triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

    Vector3 normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).normalized;
    mesh.normals = new Vector3[4] { normal, normal, normal, normal };

    // UVs: full quad (no per-corner normalization)
    Vector2[] uvs = new Vector2[4];
    uvs[0] = new Vector2(0f, 0f); // bottom-left -> (0,0)
    uvs[1] = new Vector2(1f, 0f); // bottom-right -> (1,0)
    uvs[2] = new Vector2(0f, 1f); // top-left -> (0,1)
    uvs[3] = new Vector2(1f, 1f); // top-right -> (1,1)

    mesh.uv = uvs;
    mesh.RecalculateBounds();
    mesh.RecalculateTangents();
    return mesh;
}









}