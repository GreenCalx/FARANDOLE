using System.Collections.Generic;
using UnityEngine;
using static Utils;
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class LabyrinthLayout : MonoBehaviour
{
#if UNITY_EDITOR
    public bool refreshPreviewMesh;
    public bool cleanPreview;
    public Material previewMeshMat;
    public Material previewOutlineMat;
    GameObject previewObject;
    List<GameObject> m_OutlineRenderers;
    List<LineRenderer> m_OutlineLR;
#endif

    public CompositeCollider2D CC2D;
    public Transform spawnPoint;
    public Transform rangePoint;

    public int ballNumber = 1;
    public float ballScale =1;

    public Vector3 GetSpawnInRange()
    {
        if (rangePoint == null)
            return spawnPoint.position;
        return Vector3.Lerp(spawnPoint.position, rangePoint.position, Random.Range(0f, 1f));
    }

    public void Draw()
    {

    }
    
    public void Clean()
    {
        
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Application.isPlaying)
            return;

        if (refreshPreviewMesh)
        {
            CleanPreview();

            // lab preview
            Mesh previewMesh = CC2D.CreateMesh(true, true);
            

            previewObject = GOBuilder.Create()
                            .WithName("LabPreview")
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .WithMeshFilter(previewMesh, true)
                            .WithRenderer(previewMeshMat)
                            .Build();
            Utils.BoundedUnwrapMesh(previewMesh);
            
            // outline preview     
            m_OutlineLR = new List<LineRenderer>();
            int n_path = CC2D.pathCount;
            for(int i = 0; i < n_path; i++)
            {
                LineRenderer OutlineLR = GOBuilder.Create()
                                    .WithName("Outline LR " + i)
                                    .WithParent(transform)
                                    .WithLocalPosition(Vector3.zero)
                                    .WithLineRenderer(previewOutlineMat)
                                    .BuildAs<LineRenderer>();
                Vector2[] points = new Vector2[CC2D.GetPathPointCount(i)];
                CC2D.GetPath(i, points);
                
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
                Mesh previewOutlineMesh = new Mesh(); // scope doubt
                LR.BakeMesh(previewOutlineMesh, true);
                m_OutlineRenderers.Add( 
                    GOBuilder.Create()
                                .WithName("LabOutlinePreview")
                                .WithParent(transform)
                                .WithLocalPosition(Vector3.zero)
                                .WithMeshFilter(previewOutlineMesh, true)
                                .WithRenderer(previewOutlineMat)
                                .Build()
                                );
                DestroyImmediate(LR.gameObject);
            }
            m_OutlineLR.Clear();

            cleanPreview = false;
            refreshPreviewMesh = false;
        } else if (cleanPreview)
        {
            CleanPreview();
            cleanPreview = false;
        }
    }

    void CleanPreview()
    {
        
        if (m_OutlineLR!=null)
        {
            m_OutlineLR.ForEach(e => GameObject.DestroyImmediate(e));
            m_OutlineLR.Clear();
        }
        if (m_OutlineRenderers!=null)
        {
            m_OutlineRenderers.ForEach(e => GameObject.DestroyImmediate(e));
            m_OutlineRenderers.Clear();
        }
        
        GameObject.DestroyImmediate(previewObject);
        previewObject = null;
    }
#endif
}
