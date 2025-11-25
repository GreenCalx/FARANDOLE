using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameOverEffect : MonoBehaviour
{
    public Material FXMat;
    public float duration = 1.0f;
    private float elapsedTime = 0f;
    private bool isPlaying = false;
    public AnimationCurve StrengthOverTime;
    public bool ForcePlay = false;
    public bool StartOnEnabled = false;
    UnityEvent m_CallbackOnDone;
    bool pendingFX = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        isPlaying = false;
        pendingFX = false;

        // Prewarm
        if (FXMat != null)
        {
            // Force GPU to compile and cache the shader variant
            Graphics.Blit(Texture2D.blackTexture, RenderTexture.GetTemporary(4, 4), FXMat);
            RenderTexture.ReleaseTemporary(RenderTexture.GetTemporary(4, 4));
        }
        
        if (StartOnEnabled)
            StartFX();
        // else
        //     FullScreenPassManager.Get.EnableMeltSolo(false);
    }

    // Update is called once per frame
    void OnDisable()
    {
        StopFX();
    }

    void Update()
    {
        if (isPlaying)
        {
            elapsedTime += Time.deltaTime;

            // if (elapsedTime >= duration)
            // {
            //     //elapsedTime = duration;
            //     //FXMat.SetFloat("_ControlledTime", duration); // reset
            //     //StopFX();
            // } else
            // {
                //float frac = Mathf.Clamp01(elapsedTime / duration);
                FXMat.SetFloat("_ControlledTime", elapsedTime);
                //FXMat.SetFloat("_RippleStrength", RippleStrengthOverTime.Evaluate(frac));
            // }
        }

        // Debug trigger
        if (ForcePlay)
        {
            ForcePlay = false;
            StartFX();
        }
    }

    public void SetupFX(PlayerData iPlayerData)
    {
        // Shadergraph expose bool as floats..
        FXMat.SetFloat("_UseWinGradient", iPlayerData.FullLoopCompleted ? 1f : 0f);
    }

    public void StopFX()
    {
        isPlaying = false;
        m_CallbackOnDone?.Invoke();
        FullScreenPassManager.Get.EnableMeltSolo(false);
        FXMat.SetFloat("_ControlledTime", 0f);
        Destroy(gameObject);
    }

    public void StartFX(UnityEvent iCallbackOnDone = null)
    {
        if (pendingFX || isPlaying)
            return;

        elapsedTime = 0f;
        m_CallbackOnDone = iCallbackOnDone;
        StartCoroutine(FXCo());
    }

    IEnumerator FXCo()
    {
        //Debug.Log("FX RUNNING");
        pendingFX = true;
        FXMat.SetFloat("_ControlledTime", 0f);
        
        //FXMat.SetVector("_FocalPoint", new Vector2(0.5f, 0.5f)); // screen center
        //FXMat.SetFloat("_RippleStrength", 0f);

        // Wait a frame
        yield return null;

        FullScreenPassManager.Get.EnableMeltSolo(true);

        // Wait a frame
        yield return null;
        
        pendingFX = false;
        isPlaying = true;
    }
}
