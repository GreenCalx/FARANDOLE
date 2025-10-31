using UnityEngine;
using UnityEngine.Events;

public class RollingBall : MiniGameEntity, IRacer, IRendered
{
    public UnityEvent OnFinishCB;
    public SpriteRenderer m_Renderer;

    public void InitColor(Color iColor)
    {
        m_Renderer.material.SetColor("_Color", iColor);
    }
    public void OnFinish()
    {
        if (!MG.IsActiveMiniGame)
            return;
        OnFinishCB?.Invoke();
        //Destroy(gameObject);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D iCol)
    {
        if (iCol.GetComponent<FinishLine>())
        {
            OnFinish();
        }
    }
    void OnTriggerStay2D(Collider2D iCol)
    {
        if (iCol.GetComponent<FinishLine>())
        {
            OnFinish();
        }
    }

    public Renderer GetRenderer()
    {
        return m_Renderer;
    }
    public Renderer GetStickerRenderer()
    {
        return null;
    }
}
