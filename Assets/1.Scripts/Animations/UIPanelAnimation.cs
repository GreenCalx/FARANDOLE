using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
public class UIPanelAnimation : ManagedAnimation
{
    public UnityEvent PreHideCB;
    public UnityEvent AfterHideCB;
    public UnityEvent PreShowCB;
    public UnityEvent AfterShowCB;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public override async UniTask DefaultShow(CancellationToken iCT)
    {
        PreShowCB?.Invoke();
        await base.DefaultShow(iCT);
        AfterShowCB?.Invoke();
    }

    public override async UniTask DefaultHide(CancellationToken iCT)
    {
        PreHideCB?.Invoke();
        await base.DefaultHide(iCT);
        AfterHideCB?.Invoke();
    }
}
