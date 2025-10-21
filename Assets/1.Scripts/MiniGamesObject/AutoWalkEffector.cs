using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoWalkEffector : MonoBehaviour
{
    public SpriteRenderer handle_Renderer;
    public Sprite OnTriggerSprite;
    protected SpriteRenderer SR;
    protected Rigidbody2D RB2D;
    public UnityEvent<AutoWalker> OnTriggerEnterCB;
    public UnityEvent OnAnyCollisionEnterCB;
    public UnityEvent OnDestroyCB;
    
    
    protected void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        RB2D = GetComponent<Rigidbody2D>();
    }

    protected void OnDestroy()
    {
        OnDestroyCB.Invoke();
    }

    protected void OnTriggerEnter2D(Collider2D iCol)
    {
        AutoWalker walker = iCol.gameObject.GetComponent<AutoWalker>();
        if (!!walker)
        {
            OnTriggerEnterCB?.Invoke(walker);
        }
    }
    
    protected void OnCollisionEnter2D(Collision2D iCollision)
    {
        OnAnyCollisionEnterCB?.Invoke();
    }
}
