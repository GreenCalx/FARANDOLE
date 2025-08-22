using UnityEngine;
using System.Collections.Generic;
using static Utils;

[RequireComponent(typeof(Collider2D))]
public class FinishLine : MonoBehaviour
{
    public Material finishLineMat;
    public Collider2D C2D;
    public ParticleSystem PS;
    public ParticleSystemForceField PFF;

    void Start()
    {
        // C2D = GetComponent<Collider2D>();
        // Mesh m = C2D.CreateMesh(true, true);

        // MeshFilter mf = GetComponent<MeshFilter>();
        // if (mf == null)
        //     mf = gameObject.AddComponent<MeshFilter>();
        // UnwrapMesh(m);
        // mf.mesh = m;

        // MeshRenderer mr = GetComponent<MeshRenderer>();
        // if (mr == null)
        //     mr = gameObject.AddComponent<MeshRenderer>();
        // mr.material = finishLineMat;

        ResetParticles();
    }

    public void ResetParticles()
    {
        PS.Clear();
        PS.Play();
        PFF.enabled = false;
    }

    public void ExplodeAt(Vector3 iPosition)
    {
        PFF.transform.position = new Vector3(iPosition.x, iPosition.y);
        PFF.enabled = true;
    }
}
