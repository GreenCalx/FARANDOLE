using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ManagedAnimation : MonoBehaviour
{
    public Animator animator;
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
}
