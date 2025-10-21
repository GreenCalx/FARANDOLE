using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoWalkDelayer : AutoWalkEffector
{
    public float delayTime = 0.5f;
    public float delayBeforeEnablingRB2D = 0.25f;
    Coroutine delayPhysxCo;
    protected void Start()
    {
        base.Start();
        RB2D.bodyType = RigidbodyType2D.Kinematic;
        delayPhysxCo = StartCoroutine(DelayedPhysxCo());

        OnTriggerEnterCB.AddListener( (walker) => DelayWalker(walker));
        OnAnyCollisionEnterCB.AddListener(() => OnObjectCollision());
        OnDestroyCB.AddListener(() => CleanUp());
    }

    IEnumerator DelayedPhysxCo()
    {
        RB2D.bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(delayBeforeEnablingRB2D);
        RB2D.bodyType = RigidbodyType2D.Dynamic;
    }

    public void CleanUp()
    {
        if (delayPhysxCo != null)
        {
            StopCoroutine(delayPhysxCo);
            delayPhysxCo = null;
        }
    }
    
    public void DelayWalker(AutoWalker iWalker)
    {
        if (iWalker.isDelayed)
            return;
        iWalker.Delay(this);
        if (SR != null)
        {
            SR.sprite = OnTriggerSprite;
        }
        RB2D.bodyType = RigidbodyType2D.Static;
        Destroy(gameObject, delayTime);
    }

    public void OnObjectCollision()
    {
        if (SR != null)
        {
            SR.sprite = OnTriggerSprite;
        } 
        RB2D.bodyType = RigidbodyType2D.Static;
        Destroy(gameObject, delayTime);
    }
    
}
