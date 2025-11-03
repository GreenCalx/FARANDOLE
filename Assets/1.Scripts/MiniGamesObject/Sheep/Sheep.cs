using UnityEngine;
using Cysharp.Threading.Tasks;

public class Sheep : Dragable
{
    public Color outlineInFence;
    public Color outlineOutFence;
    public float minIddleTime;

    public float maxIddleTime;

    public float minRunTime;
    public float maxRunTime;

    public float runSpeed;

    public bool isIdle;
    public float stateTimer;
    public Vector2 moveDirection;
    public Animator anim;
    public SpriteRenderer sr;
    public ClampInPlayground PGClamper;

    Vector3 initScale = Vector3.zero;

    void Start()
    {
        base.InitDragable();

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        droppedEvent.AddListener(SheepDropped);
        selectedEvent.AddListener(SheepPicked);
        isIdle = true;
        stateTimer = Random.Range(minIddleTime, maxIddleTime);
        initScale = transform.localScale;
    }
    

    void SheepPicked()
    {
        anim.SetBool("Grabbed", true);
        sr.sortingLayerName = Constants.LYR_FORGROUND;

        transform.localScale = initScale * 1.2f;
    }

    void SheepDropped()
    {
        anim.SetBool("Grabbed", false);
        sr.sortingLayerName = Constants.LYR_GAME;

        transform.localScale = initScale;
    }

    void Update()
    {
        if (selected)
            return;

        stateTimer -= Time.deltaTime;

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

    public void SetInTargetZone()
    {
        sr.material.SetColor("_OutlineColor", outlineInFence);
    }

    public void SetOutTargetZone()
    {
        sr.material.SetColor("_OutlineColor", outlineOutFence);
    }
}
