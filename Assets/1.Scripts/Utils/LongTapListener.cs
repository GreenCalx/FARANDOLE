using UnityEngine;
using UnityEngine.Events;

public class LongTapListener : MonoBehaviour, IPositionTracker
{
    public UnityEvent m_OnTapEvent;
    public bool DestroyOnEventFired = true;
    public float HoldTimeToTrigger = 1f;
    private float elapsedHoldTime = 0f;
    bool isTapped = false;

    void Start()
    {
        isTapped = false;
    }

    void Update()
    {
        if (isTapped)
        {
            elapsedHoldTime += Time.deltaTime;
            // if (elapsedHoldTime < HoldTimeToTrigger)
            //     return;
            
            m_OnTapEvent.Invoke();
            GameObject.Destroy(gameObject);
        }
    }
    public void OnStartTracking(Vector2 iPos)
    {
        elapsedHoldTime = 0f;
        isTapped = true;
    }
    public void OnPositionChanged(Vector2 iVec2)
    {
        
    }

    public void OnStopTracking(Vector2 iVec2)
    {
        elapsedHoldTime = 0f;
        isTapped = false;
    }
}
