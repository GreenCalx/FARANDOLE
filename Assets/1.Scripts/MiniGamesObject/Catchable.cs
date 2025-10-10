using UnityEngine;

public class Catchable : MonoBehaviour
{
    Rigidbody2D rb2d;
    public Vector2 maxLinearVelocity = new Vector2(5f, 5f);
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb2d.linearVelocity = new Vector2(
            Mathf.Clamp(rb2d.linearVelocity.x, -maxLinearVelocity.x, maxLinearVelocity.x),
            Mathf.Clamp(rb2d.linearVelocity.y, -maxLinearVelocity.y, maxLinearVelocity.y)
        );
    }
}
