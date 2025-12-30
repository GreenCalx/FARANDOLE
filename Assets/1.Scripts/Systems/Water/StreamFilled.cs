using UnityEngine;

public class StreamFilled : MonoBehaviour
{
    public Transform splashAnchor;
    public float maxFill = 1f;
    public float CurrentFill { get; private set; }
    public bool IsOverflowing => CurrentFill >= maxFill;

    public void Flush()
    {
        CurrentFill = 0f;
    }

    public void AddLiquid(float amount)
    {
        CurrentFill = Mathf.Clamp(CurrentFill + amount, 0, maxFill);
        UpdateVisual();
    }

    void UpdateVisual()
    {
        // change skin / slight shake ?
    }

    public Vector2 GetSplashPoint()
    {
        return splashAnchor.position;
    }
}
