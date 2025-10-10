using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Dragable : MonoBehaviour, IPositionTracker
{
    public enum DragMode { Velocity, Force, MoveToCursor}
    [HideInInspector]
    public UnityEvent selectedEvent;
    [HideInInspector]
    public UnityEvent droppedEvent;

    public DragMode dragMode;
    public float dragForce = 10f;
    private Vector2 dragDirection;
    private bool selected = false;

    private Rigidbody2D rb;

    private Collider2D dragCollider;

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
            if (dragMode == DragMode.Velocity)
            {
                if ((delta).magnitude < 0.1f)
                {
                    rb.linearVelocity = delta.normalized * dragForce * Mathf.Lerp(1, accelerationWithDistance, Mathf.Clamp((delta.magnitude - 1) / maxVelocityRange, 0, 1));
                }
            }
            else if (dragMode == DragMode.Force)
            {

                rb.AddForce(delta.normalized * dragForce * Mathf.Lerp(1,accelerationWithDistance,Mathf.Clamp((delta.magnitude - 1)/maxVelocityRange,0,1)), ForceMode2D.Impulse);

                if(delta.magnitude<0.5f && delta.magnitude != 0)
                {
                    rb.AddForce(-rb.linearVelocity * 2  * dragForce / delta.magnitude);
                } 
            }
            else if (dragMode == DragMode.MoveToCursor)
            {
                rb.MovePosition(dragDirection);
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
        if (selected)
        {
            rb.gravityScale = 0;
            dragDirection = iVec2;
            if (dragMode == DragMode.MoveToCursor)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.useFullKinematicContacts = false;
            }
            selectedEvent.Invoke();
        }


    }

    public void OnStopTracking(Vector2 iVec2)
    {
        if (!selected)
            return;

        selected = false;
        rb.gravityScale = baseGScale;
        if (dragMode == DragMode.MoveToCursor)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.useFullKinematicContacts = true;
        }
        droppedEvent.Invoke();        
    }
}
