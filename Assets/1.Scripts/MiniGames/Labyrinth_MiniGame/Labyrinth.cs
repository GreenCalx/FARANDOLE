using UnityEngine;
using System.Collections.Generic;
using static Utils;

public class Labyrinth : MonoBehaviour, IRendered
{
    public Material LabMat;
    public Material LabMatOutline;
    Rigidbody2D RB;
    Quaternion currRot, startRot;
    public GameObject visualLab;
    public Renderer m_Renderer;
    public List<GameObject> m_OutlineRenderers;
    public List<LineRenderer> m_OutlineLR;
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

    public void ClearOutlines()
    {
        if (m_OutlineLR!=null)
        {
            m_OutlineLR.ForEach(e => GameObject.DestroyImmediate(e));
            m_OutlineLR.Clear();
        }
        if (m_OutlineRenderers != null)
        {
            m_OutlineRenderers.ForEach(e => GameObject.DestroyImmediate(e));
            m_OutlineRenderers.Clear();
        }
    }

    public void SetFromLayout(LabyrinthLayout iLayout, LayerManager2D iLM2D)
    {
        // Draw Lab
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

        // Draw Outlines
        m_OutlineLR = new List<LineRenderer>();
        int n_path = iLayout.CC2D.pathCount;
        for (int i = 0; i < n_path; i++)
        {
            LineRenderer OutlineLR = GOBuilder.Create()
                                .WithName("Outline LR " + i)
                                .WithParent(transform)
                                .WithLocalPosition(Vector3.zero)
                                .WithLineRenderer(LabMatOutline)
                                .BuildAs<LineRenderer>();
            Vector2[] points = new Vector2[iLayout.CC2D.GetPathPointCount(i)];
            iLayout.CC2D.GetPath(i, points);

            OutlineLR.positionCount = points.Length;
            OutlineLR.SetPositions(Utils.ToVec3(points));
            OutlineLR.startWidth = 0.01f;
            OutlineLR.endWidth = 0.01f;
            OutlineLR.loop = true;
            OutlineLR.alignment = LineAlignment.TransformZ;
            OutlineLR.numCapVertices = 0;
            OutlineLR.numCornerVertices = 2;
            m_OutlineLR.Add(OutlineLR);
        }
        
        m_OutlineRenderers = new List<GameObject>();
        foreach (LineRenderer LR in m_OutlineLR)
        {
            Mesh OutlineMesh = new Mesh(); // scope doubt
            LR.BakeMesh(OutlineMesh, true);
            m_OutlineRenderers.Add( 
                GOBuilder.Create()
                            .WithName("LabOutlinePreview")
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .WithMeshFilter(OutlineMesh, true)
                            .WithRenderer(LabMatOutline)
                            .Build()
                            );
            DestroyImmediate(LR.gameObject);
        }
        m_OutlineLR.Clear();
    }
    

    public void Reset()
    {
        
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red);
        Debug.DrawRay(transform.position, transform.up * 2f, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
    }
}
