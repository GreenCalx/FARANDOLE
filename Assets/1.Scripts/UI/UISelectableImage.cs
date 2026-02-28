using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class UISelectableImage : ManagedAnimation
{
    [Header("UISelectableImage")]
    public UICustomSquircle light;
    public UICustomSquircle BG;
    public UICustomSquircle MASK;

    [Header("Selection")]
    public bool isSelected = false;
    public UnityEvent<UISelectableImage> onSelected;

    protected readonly string showLightStateName = "MiniGameImageShowLight";
    protected readonly string hideLightStateName = "MiniGameImageHideLight";
    protected readonly string showLightParam = "showlight";

    public bool LightShown { get; set; } = false;

    protected virtual void Awake()
    {
        if (onSelected == null)
            onSelected = new UnityEvent<UISelectableImage>();
    }

    public virtual void Select()
    {
        isSelected = true;
        ShowLight(true);
        onSelected?.Invoke(this);
    }

    public virtual void Deselect()
    {
        isSelected = false;
        ShowLight(false);
    }

    public virtual void ShowLight(bool iShow)
    {
        LightShown = iShow;
        if (m_Animator != null)
            m_Animator.SetBool(showLightParam, iShow);
    }

    public virtual void UpdateLightColor(Color iColor)
    {
        if (light != null)
        {
            Color c = iColor;
            c.a = 0f;
            light.color = c;
        }
    }

    public override async UniTask DefaultHide(CancellationToken iCT)
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool(DefaultShowAnimParm, false);
            m_Animator.SetBool(showLightParam, false);

            await UniTask.WhenAny(
                WaitAnimState(DefaultHideStateName, 1f, iCT),
                WaitAnimState(hideLightStateName, 1f, iCT)
            );
        }
        IsShown = false;
    }
}
