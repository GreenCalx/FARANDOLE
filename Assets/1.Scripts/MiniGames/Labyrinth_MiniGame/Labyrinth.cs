using UnityEngine;
using static Utils;

public class Labyrinth : MonoBehaviour, IRendered
{
    public Material LabMat;
    public Material LabMatOutline;
    Rigidbody2D RB;
    Quaternion currRot, startRot;
    public GameObject visualLab;
    public GameObject visualLabOutline;
    public Renderer m_Renderer;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        // Reset();
    }

    public Renderer GetRenderer()
    {
        return m_Renderer;
    }

    public void InitColor(Color iColor)
    {
        if (m_Renderer == null)
            return;
        m_Renderer.material.SetColor("_Color", iColor);
    }

    public void SetFromLayout(LabyrinthLayout iLayout, LayerManager2D iLM2D)
    {
        visualLab = GOBuilder.Create()
                .WithName("CompositeColliderRenderer")
                .WithParent(transform)
                .WithLocalPosition(Vector3.zero)
                .WithMeshFilter(iLayout.CC2D.CreateMesh(true, true))
                .WithRenderer(LabMat)
                .Build();
        m_Renderer = visualLab.GetComponent<Renderer>();
        Mesh m = visualLab.GetComponent<MeshFilter>()?.mesh;
        Utils.BoundedUnwrapMesh(m);

        visualLabOutline = GOBuilder.Create()
                    .WithName("LabOutlineRenderer")
                    .WithParent(visualLab.transform)
                    .WithLocalPosition(Vector3.zero)
                    .WithMeshFilter(m, false)
                    .WithRenderer(LabMatOutline)
                    .Build();

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
