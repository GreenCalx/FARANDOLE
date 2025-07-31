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
