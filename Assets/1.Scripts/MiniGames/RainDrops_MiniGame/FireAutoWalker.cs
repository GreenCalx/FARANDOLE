using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public class FireAutoWalker : AutoWalker
{
    public Sprite deathSprite;
    public bool isDead = false;
    void Start()
    {
        base.Start();
        isDead = false;
    }

    void Update()
    {
        base.Update();
    }

    void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void StartAnimation()
    {
        transform.DOScaleY(0.88f * baseScale.y, animCycle)
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                     transform.DOScaleY(baseScale.y, animCycle)
                              .SetEase(Ease.Linear)
                              .SetLoops(-1, LoopType.Yoyo); // boucle infinie
                 });

        transform.DOScaleX(1.12f * baseScale.x, animCycle)
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                     transform.DOScaleX(baseScale.x, animCycle)
                              .SetEase(Ease.Linear)
                              .SetLoops(-1, LoopType.Yoyo); // boucle infinie
                 });
    }

    public override void Kill()
    {
        if (isDead)
            return;
        isDead = true;

        base.Kill();
        RB2D.isKinematic = false;
        handle_Renderer.sprite = deathSprite;
        AutoWalk = false;
        
    }

    void OnCollisionEnter2D(Collision2D iCollision)
    {
        if (isDead)
            Destroy(gameObject);
    }

}
