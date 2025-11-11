using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public class BouncySticker : MonoBehaviour, ITapTracker
{
    public UnityEvent tapCB;
    public List<Sprite> availableSticker;

    public float speed = 5f;

    private Rigidbody2D rb;
    public SpriteRenderer sr;
    public Collider2D stickerCollider;

    public Vector2 dir;

    private bool stopped = false;
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return 0; }

    private Tween currentTween;

    private ParticleSystem winParticles;
    void Awake()
    {
        winParticles = GetComponentInChildren<ParticleSystem>();
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
        if (collision.contactCount > 0 && !stopped)
        {

            Vector2 normal = collision.GetContact(0).normal;
            rb.linearVelocity = Vector2.Reflect(dir, normal).normalized * speed;
            dir = rb.linearVelocity;
        }
    }

    public bool OnTap(Vector2 iVec)
    {
        if (stickerCollider.bounds.Contains(new Vector3(iVec.x, iVec.y, transform.position.z)) && !stopped)
        {
            tapCB.Invoke();
            return true;
        }
        return false;
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

    public void stickerWinAnim()
    {
        Sequence winSequence = DOTween.Sequence();

        winSequence.Append(transform
            .DORotate(new Vector3(0, 0, 360), 0.25f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic));

        winSequence.Append(transform
            .DOJump(transform.position, 0.3f, 1, 0.25f)
            .SetEase(Ease.OutQuad));

        winSequence.OnComplete(() =>
        {
            if (winParticles != null)
                winParticles.Play();
        });

        currentTween = winSequence;
    }
}