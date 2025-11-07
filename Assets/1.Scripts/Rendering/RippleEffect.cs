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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        // Prewarm
        if (rippleMat != null)
        {
            // Force GPU to compile and cache the shader variant
            Graphics.Blit(Texture2D.blackTexture, RenderTexture.GetTemporary(4, 4), rippleMat);
            RenderTexture.ReleaseTemporary(RenderTexture.GetTemporary(4, 4));
        }
        
        if (StartOnEnabled)
            StartRipple();
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
            float frac = Mathf.Clamp01(rippleTime / duration);
            rippleMat.SetFloat("_RippleTime", rippleTime);
            rippleMat.SetFloat("_RippleStrength", RippleStrengthOverTime.Evaluate(frac));
            if (rippleTime >= duration)
            {
                rippleTime = 0f;
                isPlaying = false;
                rippleMat.SetFloat("_RippleTime", 0f); // reset
                StopRipple();
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
        BlitRendererFeature.BlitPass.EnableRipple = false;
        Destroy(gameObject);
    }

    public void StartRipple(UnityEvent iCallbackOnDone = null)
    {
        rippleTime = 0f;
        m_CallbackOnDone = iCallbackOnDone;
        StartCoroutine(RippleCo());
    }

    IEnumerator RippleCo()
    {   
        rippleMat.SetFloat("_RippleTime", 0f);
        rippleMat.SetVector("_FocalPoint", new Vector2(0.5f, 0.5f)); // screen center
        rippleMat.SetFloat("_RippleStrength", 0f);

        BlitRendererFeature.BlitPass.EnableRipple = true;

        yield return null;

        isPlaying = true;
    }
}
