using UnityEngine;

public class Dragable : MonoBehaviour, IPositionTracker
{
    public float dragForce = 10f;
    private Vector2 dragDirection;
    private bool selected = false;

    private Rigidbody2D rb;

    private Collider2D dragCollider;

    public bool DragWithVelocity;
    public float accelerationWithDistance = 1f;
    public float maxVelocityRange = 1f;
    private float baseGScale = 1f;
    void Start()
    {

        rb = this.gameObject.GetComponent<Rigidbody2D>();
        dragCollider = this.gameObject.GetComponent<Collider2D>();
        baseGScale = rb.gravityScale;
    }

    void FixedUpdate()
    {
        if (selected)
        {
            Vector2 delta = dragDirection - rb.position;
            if(DragWithVelocity){
                if((delta).magnitude < 0.1f)
                {
                    rb.linearVelocity = delta;
                }
                else
                {
                    rb.linearVelocity = delta.normalized * dragForce * Mathf.Lerp(1,accelerationWithDistance,Mathf.Clamp((delta.magnitude - 1)/maxVelocityRange,0,1)); 
                }       
            }
            rb.AddForce(delta.normalized * dragForce * Mathf.Lerp(1,accelerationWithDistance,Mathf.Clamp((delta.magnitude - 1)/maxVelocityRange,0,1)), ForceMode2D.Impulse);

            if(delta.magnitude<0.5f && delta.magnitude != 0)
            {
                rb.AddForce(-rb.linearVelocity * 2  * dragForce / delta.magnitude);
            } 
        }
    }

    public void OnPositionChanged(Vector2 iVec2)
    {
        dragDirection = iVec2;
    }

    public void OnStartTracking(Vector2 iVec2)
    {
        Vector3 iVec3 = new Vector3(iVec2.x, iVec2.y, transform.position.z);
        selected = dragCollider.bounds.Contains(iVec3);
        if(selected){
            rb.gravityScale = 0;
        }
    }

    public void OnStopTracking(Vector2 iVec2)
    {
        selected = false;
        rb.gravityScale = baseGScale;
    }
}
