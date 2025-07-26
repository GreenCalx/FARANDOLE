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
        visualBucket = new GameObject();
        visualBucket.name = "CompositeColliderRenderer";
        visualBucket.transform.parent = transform;
        visualBucket.transform.localPosition = Vector3.zero;

        MeshFilter mf = visualBucket.AddComponent<MeshFilter>();
        mf.mesh = m;

        MeshRenderer mr = visualBucket.AddComponent<MeshRenderer>();
        mr.material = bucketMat;

        // child collider is 'merged' one of composite as it represents
        // the hull.
        inst_PlaygroundCollider = new GameObject();
        inst_PlaygroundCollider.name = "PlaygroundCollider";
        inst_PlaygroundCollider.transform.parent = transform;
        inst_PlaygroundCollider.transform.localPosition = Vector3.zero;

        BoxCollider2D bb2d = inst_PlaygroundCollider.AddComponent<BoxCollider2D>();
        foreach (BoxCollider2D b in CC2D.GetComponents<BoxCollider2D>())
        {
            if (b.compositeOperation == Collider2D.CompositeOperation.Merge)
            {
                bb2d.size = b.size;
                break;
            }
        }
        inst_PlaygroundCollider.layer = playgroundColliderLyrMsk;
        
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
