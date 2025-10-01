using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BlockingRock : MonoBehaviour
{
    public float patrolTime;

    public List<Sprite> faces;
    public SpriteRenderer faceSR;

    void Start()
    {
        faceSR.sprite = faces[Random.Range(0, faces.Count - 1)];
    }
    public void StartPatrol(float begin, float end)
    {
        transform.DOMoveX(begin, patrolTime * Mathf.Abs(transform.position.x - begin) / (end - begin))
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                     transform.DOMoveX(end, patrolTime)
                              .SetEase(Ease.Linear)
                              .SetLoops(-1, LoopType.Yoyo); // boucle infinie
                 });
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Balloon>() == null)
        {
            Destroy(collision.gameObject);
        }
    }
}
