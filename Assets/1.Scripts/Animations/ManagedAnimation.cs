using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ManagedAnimation : MonoBehaviour
{
    protected readonly string DefaultShowStateName = "Show";
    protected readonly string DefaultHideStateName = "Hide";
    protected readonly string DefaultShowAnimParm = "show";
    [Header("ManagerAnimation : References")]
    public Animator animator;
    [Header("ManagerAnimation : Internals")]
    public bool IsShown = false;
    public CancellationTokenSource cancellationTokenSource;
    public async UniTask WaitAnimState(string iStateName, float iCompletionFrac, CancellationToken iCT)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(iStateName))
        {
            if (iCT.IsCancellationRequested)
                return;
            await UniTask.Yield();
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac)
        {
            if (iCT.IsCancellationRequested)
                return;
            await UniTask.Yield();
        }
    }

    public async UniTask DefaultShow(CancellationToken iCT)
    {
        animator.SetBool(DefaultShowAnimParm, true);
        await WaitAnimState(DefaultShowStateName, 0.5f, iCT);
        if (iCT.IsCancellationRequested)
        {
            animator.SetBool(DefaultShowAnimParm, false);
            IsShown = false;
            return;
        }
        IsShown = true;
    }

    public virtual async UniTask DefaultHide(CancellationToken iCT)
    {
        animator.SetBool(DefaultShowAnimParm, false);
        await WaitAnimState(DefaultHideStateName, 1f, iCT);
        IsShown = false;
    }
}
