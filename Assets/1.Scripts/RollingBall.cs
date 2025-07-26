using UnityEngine;

public class RollingBall : MiniGameEntity, IRacer
{

    public void OnFinish()
    {
        if (!MG.IsActiveMiniGame)
            return;
            
        MG.Win();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D iCol)
    {
        if (iCol.GetComponent<FinishLine>())
        {
            OnFinish();
        }
    }
    void OnTriggerStay2D(Collider2D iCol)
    {
        if (iCol.GetComponent<FinishLine>())
        {
            OnFinish();
        }
    }
}
