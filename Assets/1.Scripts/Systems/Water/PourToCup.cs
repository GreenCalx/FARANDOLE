using UnityEngine;
using UnityEngine.Events;

public class PourToCup : MonoBehaviour
{
    public PourStreamSimulator stream;
    public StreamPour teapot;
    public StreamFilled cup;

    public float fillRate = 0.25f;

    public UnityEvent OnCupFilledCB;

    void Update()
    {
        if (!teapot.IsPouring)
        {
            return;
        }

        bool hit = stream.Simulate(teapot.PourStrength);
        if (hit)
        {
            cup.AddLiquid(fillRate * Time.deltaTime);
            if (cup.IsOverflowing)
            {
                OnCupFilledCB?.Invoke();
            }
        }


    }

}
