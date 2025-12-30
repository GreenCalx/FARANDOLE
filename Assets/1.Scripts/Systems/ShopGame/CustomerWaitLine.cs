using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CustomerWaitLine : WaitLine<Customer>
{
    public override void OnEnqueue(Customer iCustomer)
    {
        iCustomer.targetPosition = GetWaitPosition(Count - 1);
    }
    public override void OnDequeue(Customer iCustomer)
    {
        if (iCustomer.SuccessfullOrder)
            iCustomer.OnCommandSuccess();

        int i = 0;
        foreach (Customer c in waiters)
        {
            c.targetPosition = GetWaitPosition(i);
            i++;
        }
    }

    public async UniTask WaitForCustomer()
    {
        while (Count <= 0)
        { 
            if (CTS.IsCancellationRequested)
                return;
            await UniTask.Yield(); 
        }
        return;
    }

}
