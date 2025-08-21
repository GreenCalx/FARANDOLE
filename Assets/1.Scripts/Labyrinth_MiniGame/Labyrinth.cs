using UnityEngine;

public class Labyrinth : MonoBehaviour
{
    public Material LabMat;
    Rigidbody2D RB;
    Quaternion currRot, startRot;
    public GameObject visualLab;


    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        // Reset();
    }

    public void SetFromLayout(LabyrinthLayout iLayout)
    {
        visualLab = GOBuilder.Create()
                    .WithName("CompositeColliderRenderer")
                    .WithParent(transform)
                    .WithLocalPosition(Vector3.zero)
                    .WithMeshFilter(iLayout.CC2D.CreateMesh(true, true))
                    .WithRenderer(LabMat)
                    .Build();
        Mesh m = visualLab.GetComponent<MeshFilter>().mesh;
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

    public void Reset()
    {
        // transform.rotation = Quaternion.identity;
        // RB.freezeRotation = true;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red);
        Debug.DrawRay(transform.position, transform.up * 2f, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
    }
}
