using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
public class UIPanelAnimation : ManagedAnimation
{
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }
}
