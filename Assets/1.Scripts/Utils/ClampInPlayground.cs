using UnityEngine;
using UnityEngine.Events;

public class ClampInPlayground : MonoBehaviour
{
    public float clampDistOffset = 0.1f;
    public PlaygroundManager PG;
    public UnityEvent OnClamp;
    void LateUpdate()
    {
        if (PG == null)
            return;

        Vector3 pos = transform.position;
        if (!PG.IsWorldPosOOB(new Vector2(pos.x, pos.y)))
            return;

        OnClamp?.Invoke();

        pos.x = (pos.x > PG.bounds.max.x) ? PG.bounds.max.x - clampDistOffset : pos.x;
        pos.x = (pos.x < PG.bounds.min.x) ? PG.bounds.min.x + clampDistOffset : pos.x;

        pos.y = (pos.y > PG.bounds.max.y) ? PG.bounds.max.y - clampDistOffset : pos.y;
        pos.y = (pos.y < PG.bounds.min.y) ? PG.bounds.min.y + clampDistOffset : pos.y;

        transform.position = pos;

    }
}
