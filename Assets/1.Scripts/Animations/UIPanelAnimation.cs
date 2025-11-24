using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
public class UIPanelAnimation : ManagedAnimation
{
    public UnityEvent AfterHideCB;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public override async UniTask DefaultHide(CancellationToken iCT)
    {
        base.DefaultHide(iCT);
        AfterHideCB?.Invoke();
    }
}
