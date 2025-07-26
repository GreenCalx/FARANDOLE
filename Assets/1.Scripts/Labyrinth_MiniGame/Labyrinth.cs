using UnityEngine;

public class Labyrinth : MonoBehaviour, IPositionTracker
{
    public float RotSpeed = 10f;
    public Material LabMat;
    Rigidbody2D RB;
    //CompositeCollider2D CC2D;
    Quaternion currRot;
    public GameObject visualLab;
    float angle;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Reset();
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
        angle = 0f;
        currRot = Quaternion.identity;
        RB.freezeRotation = true;
    }
    public void OnPositionChanged(Vector2 iVec2)
    {
        Vector3 delta = new Vector3(iVec2.x, iVec2.y, 0f) - transform.position;
        if (delta.magnitude > 0.01f)
        {
            angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            Debug.Log("angle" + angle);
            currRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red);
        Debug.DrawRay(transform.position, transform.up * 2f, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
    }
    public void OnStartTracking(Vector2 iVec2)
    {
        RB.freezeRotation = false;
        RB.bodyType = RigidbodyType2D.Dynamic;
    }

    public void OnStopTracking(Vector2 iVec2)
    {
        RB.freezeRotation = true;
        RB.bodyType = RigidbodyType2D.Kinematic;
    }

    void FixedUpdate()
    {
        //RB.MoveRotation(Mathf.LerpAngle(RB.rotation, angle, RotSpeed * Time.fixedDeltaTime));
        transform.rotation = Quaternion.Slerp(transform.rotation, currRot, RotSpeed * Time.fixedDeltaTime);
    }
}
