using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
public class UIScoreDeltaAnim : ManagedAnimation
{
    public TextMeshProUGUI tmpro_field;
    public UnityEvent PreShowCB;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public override async UniTask DefaultHide(CancellationToken iCT)
    {
        await base.DefaultHide(iCT);
    }

    public override async UniTask DefaultShow(CancellationToken iCT)
    {
        PreShowCB?.Invoke();
        await base.DefaultShow(iCT);
        DefaultHide(iCT).Forget();
    }

}
