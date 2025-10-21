using UnityEngine;
using Cysharp.Threading.Tasks;

public class DragBall : Dragable
{
    public ParticleSystem OnWallHitPS;
    public Sprite ballClosedSprite;
    public Sprite ballOpenSprite;

    void Start()
    {
        base.InitDragable();
        m_SpriteRenderer.sprite = ballClosedSprite;
    }

    public async UniTaskVoid TryElectrify(float iDuration)
    {
        if (!selected)
            return;
        
        OnStopTracking(transform.position);

        Freeze();
        m_SpriteRenderer.sprite = ballOpenSprite;
        OnWallHitPS.Play() ;
        await UniTask.WaitForSeconds(iDuration);
        m_SpriteRenderer.sprite = ballClosedSprite;
        UnFreeze();
    }
}