using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CounterObject : MonoBehaviour, ITapTracker
{
    public SpriteRenderer sr;
    public Sprite[] countSprite;
    public int count = 0;
    public Collider2D counterCollider;
    public UnityEvent<int> tapCB;

    public float shrinkAnimDuration = 0.5f;
    public ParticleSystem selectedParticles;
    void Start()
    {
        selectedParticles = GetComponent<ParticleSystem>(); 
        counterCollider = GetComponent<Collider2D>();
        sr.GetComponent<SpriteRenderer>();
    }

    public void Setup(int index)
    {
        count = index + 1;
        if (index > countSprite.Length)
        {
            throw new ArgumentOutOfRangeException("count is higher than number of sprites");
        }
        else
        {
            sr.sprite = countSprite[index];
        }
    }
    public void OnTap(Vector2 vector2)
    {
        if (counterCollider.bounds.Contains(vector2))
        {
            Debug.Log("tap");
            tapCB.Invoke(count);
        }
    }

    public void Selected()
    {
        selectedParticles.Play();
        transform.DOScale(Vector3.zero, shrinkAnimDuration);
        sr.material.DOFade(0, shrinkAnimDuration);
    }
}
