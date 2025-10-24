using UnityEngine;

public static class MeshUtils
{
    public static Mesh QuadMesh(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3 )
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            P0,
            P1,
            P2,
            P3
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        // Compute UV accounting for non regular quad
        Vector2[] uv = new Vector2[4];

        float minX = Mathf.Min(mesh.vertices[0].x, mesh.vertices[1].x, mesh.vertices[2].x, mesh.vertices[3].x);
        float maxX = Mathf.Max(mesh.vertices[0].x, mesh.vertices[1].x, mesh.vertices[2].x, mesh.vertices[3].x);
        float minY = Mathf.Min(mesh.vertices[0].y, mesh.vertices[1].y, mesh.vertices[2].y, mesh.vertices[3].y);
        float maxY = Mathf.Max(mesh.vertices[0].y, mesh.vertices[1].y, mesh.vertices[2].y, mesh.vertices[3].y);
        for (int i = 0; i < 4; i++)
        {
            uv[i] = new Vector2(
                Mathf.InverseLerp(minX, maxX, mesh.vertices[i].x),
                Mathf.InverseLerp(minY, maxY, mesh.vertices[i].y)
            );
        }
        mesh.uv = uv;

        mesh.RecalculateTangents();

        return mesh;
    }
}