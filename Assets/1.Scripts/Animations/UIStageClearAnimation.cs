using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
public class UIStageClearAnimation : ManagedAnimation
{
    public const string StageClearTrigger = "StageClear";
    public const string StageClearState = "OnStageClear";
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public override async UniTask DefaultShow(CancellationToken iCT)
    {
        animator.SetTrigger(StageClearTrigger);
        await WaitAnimState(StageClearState, 0.75f, iCT);
    }
}
