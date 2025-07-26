using UnityEngine;

public class Dart : Throwable
{
    public bool DestroyOnTargetHit = false;
    void OnTriggerEnter2D(Collider2D iCol)
    {
        ObjectTarget target = iCol.gameObject.GetComponent<ObjectTarget>();
        if (target != null)
        {
            target.OnHit();
            if (DestroyOnTargetHit)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D iCol)
    {
        Destroy(gameObject);
    }
    void OnCollisionStay2D(Collision2D iCol)
    {
        Destroy(gameObject);
    }
}
