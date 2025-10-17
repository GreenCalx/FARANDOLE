using UnityEngine;

public class DragBall : Dragable
{
    public ParticleSystem OnWallHitPS;

    void Start()
    {
        base.InitDragable();
        collisionEvent.AddListener(( dragable, other) => OnWallHitPS.Play() );
    }
}