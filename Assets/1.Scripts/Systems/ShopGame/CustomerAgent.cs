using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerAgent : MonoBehaviour
{
    protected NavMeshAgent navAgent;
    private Vector3 mTargetPos;
    public Vector3 targetPosition
    {
        set
        {
            mTargetPos = value;
            MoveTo(value);
        }
        get
        {
            return mTargetPos;
        }
    }
    public UnityEvent OnDestinationReachCB;
    private Coroutine MoveToCo;
    public void MoveTo(Vector3 iPosition)
    {
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();
        if (MoveToCo != null)
        {
            StopCoroutine(MoveToCo);
            MoveToCo = null;
        }
        MoveToCo = StartCoroutine(MoveCo(iPosition));
    }

    IEnumerator MoveCo(Vector3 iPosition)
    {
        navAgent.SetDestination(iPosition);
        while (!navAgent.hasPath)
        { yield return null; }
        while (!Mathf.Approximately(navAgent.remainingDistance, 0f))
        { yield return null; }
        OnDestinationReachCB?.Invoke();
    }
}
