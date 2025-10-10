using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ManagedAnimation : MonoBehaviour
{
    protected readonly string DefaultShowStateName = "Show";
    protected readonly string DefaultHideStateName = "Hide";
    protected readonly string DefaultShowAnimParm = "show";
    protected readonly string animTriggerCancel = "cancel";
    [Header("ManagedAnimation : References")]
    public Animator m_Animator;
    [Header("ManagedAnimation : Internals")]
    public bool IsShown = false;
    public CancellationTokenSource cancellationTokenSource;
    public async UniTask WaitAnimState(string iStateName, float iCompletionFrac, CancellationToken iCT)
    {
        while (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName(iStateName))
        {
            if (iCT.IsCancellationRequested)
                return;
            await UniTask.Yield();
        }
        while (
            (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < iCompletionFrac) &&
            (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(iStateName))
            )
        {
            if (iCT.IsCancellationRequested)
                return;
            await UniTask.Yield();
        }
    }

    public virtual void Cancel()
    {
        m_Animator.SetBool(DefaultShowAnimParm, false);
        IsShown = false;
        m_Animator.SetTrigger(animTriggerCancel);
        cancellationTokenSource?.Cancel();
    }

    public virtual async UniTask DefaultShow(CancellationToken iCT)
    {
        m_Animator.SetBool(DefaultShowAnimParm, true);
        await WaitAnimState(DefaultShowStateName, 0.5f, iCT);
        if (iCT.IsCancellationRequested)
        {
            m_Animator.SetBool(DefaultShowAnimParm, false);
            IsShown = false;
            return;
        }
        IsShown = true;
    }

    public virtual async UniTask DefaultHide(CancellationToken iCT)
    {
        m_Animator.SetBool(DefaultShowAnimParm, false);
        await WaitAnimState(DefaultHideStateName, 0.75f, iCT);
        IsShown = false;
    }
}
