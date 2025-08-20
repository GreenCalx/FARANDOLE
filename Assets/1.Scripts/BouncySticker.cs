using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BouncySticker : MonoBehaviour, ITapTracker
{
    public UnityEvent tapCB;
    public List<Sprite> availableSticker;

    public float speed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Collider2D stickerCollider;

    public Vector2 dir;

    private bool stopped = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (sr != null && availableSticker != null && availableSticker.Count > 0)
        {
            sr.sprite = availableSticker[Random.Range(0, availableSticker.Count)];
        }

        do { dir = Random.insideUnitCircle; } while (dir.x < 0.05f && dir.y < 0.05f);
        dir.Normalize();

        rb.linearVelocity = dir * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0)
        {

            Vector2 normal = collision.GetContact(0).normal;
            rb.linearVelocity = Vector2.Reflect(dir, normal).normalized * speed;
            dir = rb.linearVelocity;
        }
    }

    public void OnTap(Vector2 iVec)
    {
        if (stickerCollider.bounds.Contains(iVec))
        {
            tapCB.Invoke();
        }
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, 0);
        stopped = true;
    }

    private void Update()
    {
        if (rb.linearVelocity.magnitude < 0.05 && !stopped)
        {
            // TODO : too expensive in update
            do { dir = Random.insideUnitCircle; } while (dir.x < 0.05f && dir.y < 0.05f); //des fois les stickers se coince dans les coins
        }
    }
}