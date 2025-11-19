using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
public class UIGameModesPanel : UIPanel
{
    public override void OnNavEnter(CancellationToken iCT)
    {
        base.OnNavEnter(iCT);
        foreach(Transform child in transform)
        {
            PremiumButtonLocker locker = child.GetComponent<PremiumButtonLocker>();
            if (locker != null)
                locker.Refresh();
        }
    }
}
