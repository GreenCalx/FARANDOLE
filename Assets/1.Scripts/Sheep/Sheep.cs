using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float minIddleTime;

    public float maxIddleTime;

    public float minRunTime;
    public float maxRunTime;

    public float runSpeed;

    private Rigidbody2D rb;

    public bool isIdle;
    public float stateTimer;
    public Vector2 moveDirection;
    public Animator anim;
    public Dragable drag;

    private bool selected = false;
    public SpriteRenderer sr;

    void Start()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        if (drag == null)
        {
            drag = GetComponent<Dragable>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        drag.droppedEvent.AddListener(SheepDropped);
        drag.selectedEvent.AddListener(SheepPicked);
        isIdle = true;
        stateTimer = Random.Range(minIddleTime, maxIddleTime);
        selected = false;
    }

    void SheepPicked()
    {
        selected = true;
        anim.SetBool("Grabbed", true);
        sr.sortingOrder += 100;
    }

    void SheepDropped()
    {
        selected = false;
        anim.SetBool("Grabbed", false);
        sr.sortingOrder -= 100;
    }

    void FixedUpdate()
    {
        if (selected)
            return;
            
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
        {
            if (isIdle)
            {
                isIdle = false;
                moveDirection = Random.insideUnitCircle.normalized;
                sr.flipX = moveDirection.x >= 0;
                stateTimer = Random.Range(minRunTime, maxRunTime);
                anim.SetBool("Running", true);
            }
            else
            {
                isIdle = true;
                moveDirection = Vector2.zero;
                stateTimer = Random.Range(minIddleTime, maxIddleTime);
                anim.SetBool("Running", false);
            }
        }

        rb.MovePosition(rb.position + moveDirection * runSpeed * Time.fixedDeltaTime);
    }
}
