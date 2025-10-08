using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ManagedAnimation : MonoBehaviour
{
    protected readonly string DefaultShowStateName = "Show";
    protected readonly string DefaultHideStateName = "Hide";
    protected readonly string DefaultShowAnimParm = "show";
    protected readonly string animTriggerCancel = "cancel";
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
        while (
            (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac) &&
            (animator.GetCurrentAnimatorStateInfo(0).IsName(iStateName))
            )
        {
            if (iCT.IsCancellationRequested)
                return;
            await UniTask.Yield();
        }
    }

    public virtual void Cancel()
    {
        animator.SetBool(DefaultShowAnimParm, false);
        IsShown = false;
        animator.SetTrigger(animTriggerCancel);
        cancellationTokenSource?.Cancel();
    }

    public virtual async UniTask DefaultShow(CancellationToken iCT)
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
        await WaitAnimState(DefaultHideStateName, 0.75f, iCT);
        IsShown = false;
    }
}
