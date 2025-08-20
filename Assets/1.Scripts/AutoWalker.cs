using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AutoWalker : MonoBehaviour, ITapTracker
{
    [Header("Tweaks")]
    public SpriteRenderer handle_Renderer;
    public float walkDuration = 3f;
    public bool ReverseBehaviour = false;
    public UnityEvent<bool> OnAutoWalkToggleCB;
    public UnityEvent OnReachCB;
    public UnityEvent OnPreDelayedyCB;
    public UnityEvent OnPostDelayedCB;
    [Header("Internals")]
    public bool AutoWalk = false;
    public Vector3 from;
    public Vector3 to;
    float elapsedTime;
    Coroutine DelayedCo;
    public bool isDelayed = false;
    void Start()
    {
        if (ReverseBehaviour)
        {
            AutoWalk = true;
        }
        transform.position = from;
        elapsedTime = 0f;
    }
    public void OnTap(Vector2 iVec2)
    {
        if (isDelayed)
            return;
        
        AutoWalk = !AutoWalk;
        OnAutoWalkToggleCB.Invoke(AutoWalk);
    }

    void Update()
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

    void OnDestroy()
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

    IEnumerator DelayCo(float iTime)
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

}
