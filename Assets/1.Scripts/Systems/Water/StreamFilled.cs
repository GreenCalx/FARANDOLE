using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class StreamFilled : MonoBehaviour
{
    [Header("Handles")]
    public SpriteRenderer SR;
    public Transform splashAnchor;
    public Rigidbody2D RB;
    [Header("Tweaks")]
    public float maxFill = 1f;
    public float CurrentFill { get; private set; }
    public bool IsOverflowing => CurrentFill >= maxFill;
    public float FillRatio => CurrentFill / maxFill;
    public Sprite idleSprite;
    public Sprite filledSprited;
    [Header("events")]
    public UnityEvent OnLiquidFillCB;
    [Header("Shake config")]
    public float shakeStrengthOnFill =0.2f;
    public float shakeDuration = 0.5f;
    public float shakeVibrato = 1f;
    bool shakeLock = false;


    public void Flush()
    {
        CurrentFill = 0f;
        UpdateMass();
    }

    public void AddLiquid(float amount)
    {
        CurrentFill = Mathf.Clamp(CurrentFill + amount, 0, maxFill);
        UpdateVisual(true);
        UpdateMass();

        OnLiquidFillCB?.Invoke();
    }

    public void UpdateMass()
    {
        RB.mass = 1f + CurrentFill;
    }

    public void UpdateVisual(bool isFilled)
    {
        if (!isFilled)
        {
            SR.sprite = idleSprite;
            return;
        }
        SR.sprite = filledSprited;
        //slight shake ?

        // shake
        // if (shakeLock)
        //     return;

        // transform.DOShakePosition(
        //     shakeDuration,
        //     new Vector3(shakeStrengthOnFill, 0f, 0f),
        //     shakeVibrato,
        //     1f,
        //     false,
        //     false
        // ).OnComplete(()=>shakeLock=false);
        // shakeLock = true;
    }

    public Vector2 GetSplashPoint()
    {
        return splashAnchor.position;
    }
}
