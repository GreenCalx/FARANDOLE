using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class LabyrinthLayout : MonoBehaviour
{
#if UNITY_EDITOR
    public bool refreshPreviewMesh;
    public bool cleanPreview;
    public Material previewMeshMat;
    GameObject previewObject;
#endif

    public CompositeCollider2D CC2D;
    public Transform spawnPoint;
    public Transform rangePoint;
    public Vector3 GetSpawnInRange()
    {
        if (rangePoint == null)
            return spawnPoint.position;
        return Vector3.Lerp(spawnPoint.position, rangePoint.position, Random.Range(0f, 1f));
    }

    #if UNITY_EDITOR
    void Update()
    {
        if (Application.isPlaying)
            return;

        if (refreshPreviewMesh)
        {
            CleanPreview();
            Mesh previewMesh = CC2D.CreateMesh(true, true);
            previewObject = GOBuilder.Create()
                            .WithName("LabPreview")
                            .WithParent(transform)
                            .WithLocalPosition(Vector3.zero)
                            .WithMeshFilter(previewMesh, true)
                            .WithRenderer(previewMeshMat)
                            .Build();
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
        GameObject.DestroyImmediate(previewObject);
        previewObject = null;
    }
#endif
}
