using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour, ITapTracker, IRendered
{
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return sr.sortingOrder; }
    public bool DestroyOnTap = false;
    public SpriteRenderer sr;
    public SpriteRenderer sr_sticker;
    public Collider2D col;
    public UnityEvent OnTapDo;
    public bool OnTap(Vector2 vec)
    {
        if (Utils.IsContained2D(vec, col.bounds))
        {
            OnTapDo?.Invoke();
            return true;
        }
        return false;
    }

    public void Kill()
    {
        GameObject.Destroy(gameObject);
    }

    public Renderer GetRenderer()
    {
        return sr;
    }
    public Renderer GetStickerRenderer()
    {
        return sr_sticker;
    }
}
