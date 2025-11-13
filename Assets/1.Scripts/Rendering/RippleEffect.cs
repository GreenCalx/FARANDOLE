using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RippleEffect : MonoBehaviour
{
    public Material rippleMat;
    public float duration = 1.0f;
    private float rippleTime = 0f;
    private bool isPlaying = false;
    public AnimationCurve RippleStrengthOverTime;
    public bool ForcePlay = false;
    public bool StartOnEnabled = false;
    UnityEvent m_CallbackOnDone;
    bool pendingRipple = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        pendingRipple = false;
        isPlaying = false;
        
        // Prewarm
        if (rippleMat != null)
        {
            // Force GPU to compile and cache the shader variant
            Graphics.Blit(Texture2D.blackTexture, RenderTexture.GetTemporary(4, 4), rippleMat);
            RenderTexture.ReleaseTemporary(RenderTexture.GetTemporary(4, 4));
        }

        if (StartOnEnabled)
            StartRipple();
        else
            FullScreenPassManager.Get.EnableRippleSolo(false);
    }

    // Update is called once per frame
    void OnDisable()
    {
        
    }

    void Update()
    {
        if (isPlaying)
        {
            rippleTime += Time.deltaTime;

            if (rippleTime >= duration)
            {
                rippleTime = duration;
                isPlaying = false;
                rippleMat.SetFloat("_RippleTime", 0f); // reset
                StopRipple();
            } else
            {
                float frac = Mathf.Clamp01(rippleTime / duration);
                rippleMat.SetFloat("_RippleTime", rippleTime);
                rippleMat.SetFloat("_RippleStrength", RippleStrengthOverTime.Evaluate(frac));
            }
        }

        // Debug trigger
        if (ForcePlay)
        {
            ForcePlay = false;
            StartRipple();
        }
    }

    public void StopRipple()
    {
        m_CallbackOnDone?.Invoke();
        FullScreenPassManager.Get.EnableRippleSolo(false);
        Destroy(gameObject);
    }

    public void StartRipple(UnityEvent iCallbackOnDone = null)
    {
        if (pendingRipple || isPlaying)
            return;

        rippleTime = 0f;
        m_CallbackOnDone = iCallbackOnDone;
        StartCoroutine(RippleCo());
    }

    IEnumerator RippleCo()
    {
        pendingRipple = true;
        rippleMat.SetFloat("_RippleTime", 0f);
        rippleMat.SetVector("_FocalPoint", new Vector2(0.5f, 0.5f)); // screen center
        rippleMat.SetFloat("_RippleStrength", 0f);

        FullScreenPassManager.Get.EnableRippleSolo(true);

        // Wait a frame
        yield return null;
        
        pendingRipple = false;
        isPlaying = true;
    }
}
