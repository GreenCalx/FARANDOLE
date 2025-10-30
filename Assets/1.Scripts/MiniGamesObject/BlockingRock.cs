using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BlockingRock : MonoBehaviour
{
    public float patrolTime;

    public float randomAngleRange;

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

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.GetComponent<Throwable>())
        {        
            Debug.Log("trigger");
            Rigidbody2D dartRB = col.gameObject.GetComponent<Rigidbody2D>();


            float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);
            dartRB.linearVelocity = Quaternion.Euler(0, 0, randomAngle) * dartRB.linearVelocity;
        }
    }
}
