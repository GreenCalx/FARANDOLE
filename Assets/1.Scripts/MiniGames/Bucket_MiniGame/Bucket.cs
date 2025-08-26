using UnityEngine;

public class Bucket : MonoBehaviour, IPositionTracker
{
    Rigidbody2D RB;
    CompositeCollider2D CC2D;
    public Material bucketMat;
    GameObject visualBucket;
    public Vector2 velocity;
    public float rotSpeed;
    Vector2 currPosition;
    Quaternion currRot;
    GameObject inst_PlaygroundCollider;
    public LayerMask playgroundColliderLyrMsk;

    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.y);  }
    }

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        CC2D = GetComponent<CompositeCollider2D>();

        Mesh m = CC2D.CreateMesh(true, true);
        visualBucket = GOBuilder.Create()
                        .WithName("CompositeColliderRenderer")
                        .WithParent(transform)
                        .WithLocalPosition(Vector3.zero)
                        .WithMeshFilter(m)
                        .WithRenderer(bucketMat)
                        .Build();
    }
    public void OnPositionChanged(Vector2 iVec2)
    {
        Vector3 delta = new Vector3(iVec2.x, iVec2.y, 0f) - transform.position;

        //RB.MovePosition(iVec2);
        currPosition = iVec2;

        if (delta.magnitude > 0.01f)
        {
            var angle = Mathf.Atan2(-delta.x, delta.y) * Mathf.Rad2Deg;
            currRot = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void OnStartTracking(Vector2 iVec2) {}

    public void OnStopTracking(Vector2 iVec2) {}

    void FixedUpdate()
    {
        //RB.MovePosition(currPosition);
        RB.AddForce(currPosition - position, ForceMode2D.Force);
        RB.MoveRotation(currRot);
    }
}
