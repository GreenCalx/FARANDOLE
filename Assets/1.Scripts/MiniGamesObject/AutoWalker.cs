using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class AutoWalker : MonoBehaviour
{
    [Header("Tweaks")]
    public SpriteRenderer handle_Renderer;
    public float walkDuration = 3f;
    public float animCycle = 0.3f;
    public bool ReverseBehaviour = false;
    public UnityEvent<bool> OnAutoWalkToggleCB;
    public UnityEvent OnReachCB;
    public UnityEvent OnPreDelayedyCB;
    public UnityEvent OnPostDelayedCB;
    public UnityEvent OnKilledCB;
    [Header("Internals")]
    public bool AutoWalk = false;
    public Vector3 from;
    public Vector3 to;
    protected float elapsedTime;
    protected Coroutine DelayedCo;
    public bool isDelayed = false;
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return 0; }
    protected Vector3 baseScale;
    protected Rigidbody2D RB2D;
    protected void Start()
    {
        if (ReverseBehaviour)
        {
            AutoWalk = true;
        }
        transform.position = from;
        elapsedTime = 0f;
        OnReachCB.AddListener(() => transform.DOKill());
        baseScale = transform.localScale;
        RB2D = GetComponent<Rigidbody2D>();
        StartAnimation();
    }

    protected void Update()
    {
        if (AutoWalk)
        {
            elapsedTime += Time.deltaTime;
            float frac = elapsedTime / walkDuration;
            transform.position = Vector3.Lerp(from, to, frac);
            if (Vector3.Distance(transform.position, to) <= 0.01f)
            {
                transform.position = to;
                OnReachCB.Invoke();
                AutoWalk = false;
            }
        }
    }

    protected virtual void StartAnimation()
    {

    }

    protected void OnDestroy()
    {
        if (DelayedCo != null)
        {
            StopCoroutine(DelayedCo);
            DelayedCo = null;
        }
    }

    public void Delay(AutoWalkDelayer iDelayer)
    {
        if (isDelayed)
            return;

        if (DelayedCo != null)
        {
            StopCoroutine(DelayedCo);
            DelayedCo = null;
        }
        DelayedCo = StartCoroutine(DelayCo(iDelayer.delayTime));
    }

    protected IEnumerator DelayCo(float iTime)
    {
        isDelayed = true;
        OnPreDelayedyCB.Invoke();

        bool wasAutoWalking = AutoWalk;
        AutoWalk = false;
        yield return new WaitForSeconds(iTime);
        AutoWalk = wasAutoWalking;

        isDelayed = false;
        OnPostDelayedCB.Invoke();
    }

    public virtual void Kill()
    {
        OnKilledCB?.Invoke();
    }

}
