using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class AutoWalker : MonoBehaviour
{
    [Header("MAND")]
    public SplineWalker splineWalker;
    [Header("Tweaks")]
    public SpriteRenderer handle_Renderer;
    public float animCycle = 0.3f;
    public float walkDuration = 2.5f;
    public bool ReverseBehaviour = false;
    public UnityEvent<bool> OnAutoWalkToggleCB;
    public UnityEvent OnReachCB;
    public UnityEvent OnPreDelayedyCB;
    public UnityEvent OnPostDelayedCB;
    public UnityEvent OnKilledCB;
    [Header("Internals")]
    public bool AutoWalk = false;
    protected float elapsedTime;
    protected Coroutine DelayedCo;
    public bool isDelayed = false;
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return 0; }
    protected Vector3 baseScale;
    protected Rigidbody2D RB2D;
    public float pathFrac = 0f;
    protected void Start()
    {
        if (ReverseBehaviour)
        {
            AutoWalk = true;
        }
        elapsedTime = 0f;
        splineWalker.UpdatePosition(0f);;
        OnReachCB.AddListener(() => transform.DOKill());
        baseScale = transform.localScale;
        RB2D = GetComponent<Rigidbody2D>();
        StartAnimation();
    }

    protected void Update()
    {
        if ((splineWalker!=null) && AutoWalk)
        {
            elapsedTime += Time.deltaTime;
            pathFrac = Mathf.Clamp01(elapsedTime / walkDuration);
            splineWalker.UpdatePosition(pathFrac);
            if (pathFrac >= 1f)
            {
                AutoWalk = false;
                OnReachCB.Invoke();
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
