using UnityEngine;
using System.Collections.Generic;
using static Utils;

[RequireComponent(typeof(Collider2D))]
public class FinishLine : MonoBehaviour
{
    public Material finishLineMat;
    public Collider2D C2D;

    void Start()
    {
        C2D = GetComponent<Collider2D>();
        Mesh m = C2D.CreateMesh(true, true);

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
            mf = gameObject.AddComponent<MeshFilter>();
        UnwrapMesh(m);
        mf.mesh = m;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
            mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = finishLineMat;


    }
}
