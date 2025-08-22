using UnityEngine;
using UnityEngine.Events;

public class RollingBall : MiniGameEntity, IRacer
{
    public UnityEvent OnFinishCB;
    public void OnFinish()
    {
        if (!MG.IsActiveMiniGame)
            return;
        OnFinishCB?.Invoke();
        MG.Win();
        //Destroy(gameObject);
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
