using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CounterObject : MonoBehaviour, ITapTracker, ISpawnable
{
    public SpriteRenderer sr;
    public SpriteRenderer bodySR;
    public SpriteRenderer stickerSR;
    public Sprite[] countSprite;
    public Sprite[] fantasyNumbersSprite;
    public int count = 0;
    public Collider2D counterCollider;
    public UnityEvent<int> tapCB;

    public float shrinkAnimDuration = 0.5f;
    public bool stopPropagation => true;


    public int GetDisplayPriority(){ return sr.sortingOrder; }
    void Start()
    {
        counterCollider = GetComponent<Collider2D>();
    }

    public void Setup(int index, bool iFantasyNumber)
    {
        count = index + 1;
        if (index > countSprite.Length)
        {
            throw new ArgumentOutOfRangeException("count is higher than number of sprites");
        }
        else
        {
            sr.sprite = iFantasyNumber ? fantasyNumbersSprite[index] : countSprite[index];
            if (iFantasyNumber)
                sr.color = Color.black;
            else
                sr.color = Color.white;
        }
    }
    public bool OnTap(Vector2 vector2)
    {
        if (!counterCollider)
            return false;
            
        if (counterCollider.bounds.Contains(vector2))
        {
            tapCB.Invoke(count);
            return true;
        }
        return false;
    }

    public void Selected()
    {
        transform.DOScale(Vector3.zero, shrinkAnimDuration);
        // TODO Make it compliant with custom shaders
        //sr.material.DOFade(0, shrinkAnimDuration);
    }
}
