using UnityEngine;
using UnityEngine.Events;

public class Fence : MonoBehaviour
{

    public int numberOfSheepInside = 0;
    [HideInInspector]
    public int totalSheepsNumber;
    [HideInInspector]
    public UnityEvent fenceFull;
    [HideInInspector]
    public bool isFull = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        numberOfSheepInside += 1;
        if (numberOfSheepInside >= totalSheepsNumber)
        {
            isFull = true;
            fenceFull.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        numberOfSheepInside -= 1;
    }

    
}
