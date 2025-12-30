
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemWaitLine : WaitLine<ShopItem>
{
    public override void OnEnqueue(ShopItem iItem)
    {
        iItem.transform.position = GetWaitPosition(Count - 1);
    }
    public override void OnDequeue(ShopItem iItem)
    {
        Destroy(iItem.gameObject);

        int i = 0;
        foreach (ShopItem item in waiters)
        {
            item.transform.position = GetWaitPosition(i);
            i++;
        }
    }

    public async UniTask WaitForItemAtPeekAndCrafted(ShopItem iItem, int iShopOrderLatchInMs = 0)
    {
        while ((Peek() != null) && (iItem != Peek()))
        {
            if (CTS.IsCancellationRequested)
                return;
            await UniTask.Yield(); 
        }
        while (!Peek().isCrafted)
        {
            if (CTS.IsCancellationRequested)
                return;
            await UniTask.Yield();
        }
        await UniTask.Delay(iShopOrderLatchInMs);
        return;
    }
}
