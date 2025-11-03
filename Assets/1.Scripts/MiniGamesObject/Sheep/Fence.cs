using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Fence : MonoBehaviour
{

    public List<Sheep> SheepsInside = new List<Sheep>();
    [HideInInspector]
    public int totalSheepsNumber;
    [HideInInspector]
    public bool isFull => (SheepsInside.Count >= totalSheepsNumber);

    void OnTriggerEnter2D(Collider2D collision)
    {
        Sheep s = collision.gameObject.GetComponent<Sheep>();
        if (s == null)
            return;
        s.SetInTargetZone();

        SheepsInside.Add(s);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Sheep s = collision.gameObject.GetComponent<Sheep>();
        if (s == null)
            return;
        s.SetOutTargetZone();
        SheepsInside.Remove(s);
    }

    
}
