using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class WaitLine<T> : MonoBehaviour where T : MonoBehaviour
{
    public CancellationTokenSource CTS;
    public Vector3 WaitLineDir;
    public float WaitLineSpacing;
    public int WaitLineCapacity;
    public Queue<T> waiters = new Queue<T>();
    public int Count
    {
        get { return waiters.Count; }
    }

    public void Setup(float iSpacing, int iCapacity)
    {
        WaitLineSpacing = iSpacing;
        WaitLineCapacity = iCapacity;
        CTS = new CancellationTokenSource();
    }

    public T Peek()
    {
        if (waiters.Count <= 0)
            return null;
        return waiters.Peek();
    }
    public T Dequeue()
    {
        T dequeued = waiters.Dequeue();
        OnDequeue(dequeued);
        return dequeued;
    }
    public void Enqueue(T iWaiter)
    {
        waiters.Enqueue(iWaiter);
        OnEnqueue(iWaiter);
    }

    public void Flush()
    {
        CTS?.Cancel();

        if (waiters.Count < 1)
            return;
            
        T waiter = waiters.Dequeue();
        while (waiter!=null)
        {
            Destroy(waiter.gameObject);
            if (waiters.Count < 1)
                break;
            waiter = waiters.Dequeue();
        }
        waiters = new Queue<T>(WaitLineCapacity);
    }
    public bool IsFull() { return waiters.Count >= WaitLineCapacity; }
    protected Vector3 GetWaitPosition(int iQueuePosition)
    {
        if (iQueuePosition == 0)
            return transform.position;
        return transform.position - (-WaitLineDir * iQueuePosition * WaitLineSpacing);
    }

    public virtual void OnEnqueue(T iWaiter) { }
    public virtual void OnDequeue(T iWaiter) { }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, WaitLineDir * 2f, Color.red);
    }
#endif
}
