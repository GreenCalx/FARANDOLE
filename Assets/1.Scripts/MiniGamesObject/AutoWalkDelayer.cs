using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoWalkDelayer : MonoBehaviour
{
    public SpriteRenderer handle_Renderer;
    public float delayTime = 0.5f;
    public Sprite OnTriggerSprite;
    public float delayBeforeEnablingRB2D = 0.25f;
    SpriteRenderer SR;
    Rigidbody2D RB2D;
    Coroutine delayPhysxCo;
    public UnityEvent OnDestroyCB;
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        RB2D = GetComponent<Rigidbody2D>();
        RB2D.bodyType = RigidbodyType2D.Kinematic;

        delayPhysxCo = StartCoroutine(DelayedPhysxCo());
    }

    IEnumerator DelayedPhysxCo()
    {
        RB2D.bodyType = RigidbodyType2D.Kinematic;
        yield return new WaitForSeconds(delayBeforeEnablingRB2D);
        RB2D.bodyType = RigidbodyType2D.Dynamic;
    }

    void OnDestroy()
    {
        if (delayPhysxCo!=null)
        {
            StopCoroutine(delayPhysxCo);
            delayPhysxCo = null;
        }
        OnDestroyCB.Invoke();
    }

    void OnTriggerEnter2D(Collider2D iCol)
    {
        AutoWalker walker = iCol.gameObject.GetComponent<AutoWalker>();
        if (!!walker)
        {
            if (walker.isDelayed)
                return;
                
            walker.Delay(this);
            if (SR != null)
            {
                SR.sprite = OnTriggerSprite;
            }
            transform.position = walker.transform.position;
            RB2D.bodyType = RigidbodyType2D.Static;
            Destroy(gameObject, delayTime);
        }
    }
}
