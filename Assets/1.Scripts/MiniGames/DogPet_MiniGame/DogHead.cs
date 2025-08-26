using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DogHead : MonoBehaviour, ITapTracker
{
    float tapRadius = 1f;
    public UnityEvent tapCB;
    Rigidbody2D rb2d;
    public SpriteRenderer SR;
    public Sprite idleSprite;
    public Sprite tapSprite;
    public Sprite onWinSprite;
    public float tapAnimDuration = 0.1f;
    [Header("Pop Out Anim")]
    public ParticleSystem PS;
    public float torque = 10f;
    Coroutine tapAnimCo;
    Vector3 baseScale;
    Vector3 animScale;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        SR.sprite = idleSprite;
        baseScale = SR.transform.localScale;
        animScale = baseScale * 0.9f;
    }

    void OnDestroy()
    {
        if (tapAnimCo != null)
        {
            StopCoroutine(tapAnimCo);
            tapAnimCo = null;
        }
    }

    public void StopAnim()
    {
        StopCoroutine(tapAnimCo);
        tapAnimCo = null;
    }
    public void OnTap(Vector2 iVec2)
    {
        if (Vector3.Distance(transform.position, new Vector3(iVec2.x, iVec2.y, 0f)) < tapRadius)
        {
            tapCB.Invoke();
        }
    }
    public void TapEffect(int iMGDifficulty)
    {
        SR.sprite = tapSprite;
        if (rb2d)
        {
            rb2d.AddForce(-Vector3.up * 3f * iMGDifficulty, ForceMode2D.Impulse);
        }
        if (tapAnimCo != null)
        {
            StopCoroutine(tapAnimCo);
            tapAnimCo = null;
        }
        tapAnimCo = StartCoroutine(TapAnim());
    }

    IEnumerator TapAnim()
    {
        float startTime = Time.time;
        SR.transform.localScale = baseScale;
        while ((Time.time - startTime) < tapAnimDuration)
        {
            float frac = (Time.time - startTime) / tapAnimDuration;
            if (frac < 0.5f)
                SR.transform.localScale = Vector3.Lerp(baseScale, animScale, frac * 2);
            else
                SR.transform.localScale = Vector3.Lerp(animScale, baseScale, frac);
            yield return null;
        }
        SR.transform.localScale = baseScale;
        SR.sprite = idleSprite;
    }

    public void FadeOutAnim()
    {
        if (tapAnimCo != null)
            return;
        tapAnimCo = StartCoroutine(FadeOutAnimCo());
    }

    IEnumerator FadeOutAnimCo()
    {
        float animationDuration = GameData.GetSettings.PostMiniGameLatchInMs / 1000f;
        Vector3 baseScale = transform.localScale;
        rb2d.linearVelocity = Vector3.zero;
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        // spin

        // shrink to zero scale
        float startAnimTime = Time.time;
        float lerpFactor = 1f;
        while (lerpFactor > 0f)
        {
            rb2d.AddTorque(torque, ForceMode2D.Impulse);

            lerpFactor = 1f - ((Time.time - startAnimTime) / animationDuration);
            transform.localScale = baseScale * lerpFactor;
            yield return null;
        }
    }
}
