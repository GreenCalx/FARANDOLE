using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class FoldPaperCell : MonoBehaviour
{
    public Vector2 position;
    public Mesh m;
    public Vector2 uv;
    public Color c = Color.white;
    public float cell_w, cell_h;
    MeshFilter MF;
    MeshRenderer MR;
    public bool InRotation = false;
    Coroutine selfRotCo;
    GameObject inst_duplicatedFace;

    public void Init(Vector2 iPosition)
    {
        position = iPosition;
        MF = GetComponent<MeshFilter>();
        MR = GetComponent<MeshRenderer>();
        inst_duplicatedFace = null;
    }

    public void RotateCellAround(Vector3 iRotAxis, float iTotalAngle, float iRotAnimDuration = 2f )
    {
        if (selfRotCo != null)
            return;

        selfRotCo = StartCoroutine(RotCo(iRotAxis, iTotalAngle, iRotAnimDuration));
    }
    IEnumerator RotCo(Vector3 iRotAxis, float iTotalAngle, float iRotAnimDuration)
    {
        InRotation = true;
        Quaternion initRot = transform.rotation;
        Quaternion targetRot = Quaternion.AngleAxis(iTotalAngle, iRotAxis);
        float startTime = Time.time;
        float performedRot = 0f;
        while ((Time.time - startTime) < iRotAnimDuration)
        {
            float frac = (Time.time - startTime) / iRotAnimDuration;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, frac);
            yield return null;
        }
        transform.rotation = targetRot;
        OnRotationEnd();
    }

    public void OnRotationEnd()
    {
        InRotation = false;
        StopCoroutine(selfRotCo);
        selfRotCo = null;
    }

    public void CreatePaperCellMesh(Vector2 iSize, Vector2 iUVMin, Vector2 iUVMax)
    {
        m = new Mesh();
        VertexHelper vh = new VertexHelper();

        cell_w = iSize.x;
        cell_h = iSize.y;

        float half_w = cell_w / 2;
        float half_h = cell_h / 2;
        // Add unit Square
        vh.AddVert(new Vector3(0, 0), c, new Vector2(iUVMin.x, iUVMin.y));
        vh.AddVert(new Vector3(0, cell_h), c, new Vector2(iUVMin.x, iUVMax.y));
        vh.AddVert(new Vector3(cell_w, cell_h), c, new Vector2(iUVMax.x, iUVMax.y));
        vh.AddVert(new Vector3(cell_w, 0), c, new Vector2(iUVMax.x, iUVMin.y));

        // build triangles

        // front face
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);

        // back face
        vh.AddTriangle(2,1,0);
        vh.AddTriangle(0,3,2);

        // Build mesh
        vh.FillMesh(m);
        MF.mesh = m;
    }
}