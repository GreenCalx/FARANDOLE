using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShatterEffect : MonoBehaviour
{
    public float shatterDuration = 1f;
    public float fadeDuration = 1f;
    private Material material;
    private float shatterTime;
    private bool isShattering;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        if (isShattering)
        {
            float progress = (Time.time - shatterTime) / shatterDuration;
            material.SetFloat("_ShatterAmount", Mathf.Clamp01(progress));

            if (progress >= 1f)
            {
                float fadeProgress = (Time.time - shatterTime - shatterDuration) / fadeDuration;
                Color color = material.color;
                color.a = 1f - Mathf.Clamp01(fadeProgress);
                material.color = color;

                if (fadeProgress >= 1f)
                    Destroy(gameObject);
            }
        }
    }

    public void TriggerShatter()
    {
        isShattering = true;
        shatterTime = Time.time;
    }
}
