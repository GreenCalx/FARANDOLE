using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Balloon : ObjectTarget
{
    public Animator animator;
    public SpriteRenderer face;
    public SpriteRenderer body;
    public List<Sprite> availableFaces;

    public Color colorRangeMin, colorRangeMax;

    void Start()
    {
        if ((availableFaces != null) && (availableFaces.Count > 0))
        {
            face.sprite = availableFaces[UnityEngine.Random.Range(0, availableFaces.Count)];
        }
        Color c = Color.Lerp(colorRangeMin, colorRangeMax, UnityEngine.Random.Range(0f, 1f));
        face.color = c;
        body.color = c;        
    }

    public void ExplodeAnim()
    {
        face.enabled = false;
        animator.SetTrigger("Explode");
    }
}
