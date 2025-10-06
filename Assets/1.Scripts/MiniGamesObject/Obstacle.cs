using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour, ITapTracker, IRendered
{
    public bool stopPropagation => true;
    public int GetDisplayPriority() { return sr.sortingOrder; }

    public SpriteRenderer sr;
    public Collider2D col;

    void Awake()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        if (col == null)
        {
            col = GetComponent<Collider2D>();
        }
    }

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
}
