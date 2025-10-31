using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour, ITapTracker, IRendered
{
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return sr.sortingOrder; }

    public SpriteRenderer sr;
    public SpriteRenderer sr_sticker;
    public Collider2D col;

    public bool OnTap(Vector2 vec)
    {
        if (col.bounds.Contains(vec))
        {
            return true;
        }
        return false;
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
