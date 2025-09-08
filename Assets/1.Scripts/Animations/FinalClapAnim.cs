using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
public class FinalClapAnim : MonoBehaviour
{
    public bool startClosed = false;
    [Range(0f, 1f)]
    public float lerpFactor;
    public Transform blind;
    private bool InAnimation;
    Vector3 blindClosed;
    Vector3 blindOpen;
    public UnityEvent OnCloseCB, OnOpenCB;
    readonly string CloseAnimParm = "Close";
    readonly string OpenAnimParm = "Open";
    Animator animator;
    public void Init(Transform iBlind, float iYOffset)
    {
        animator = GetComponent<Animator>();
        OnCloseCB = new UnityEvent();
        OnOpenCB = new UnityEvent();

        blind = iBlind;
        blindClosed = blind.position;
        blindOpen = new Vector3(blindClosed.x, blindClosed.y + iYOffset);

        if (startClosed)
            ForceClose();
        else
            ForceOpen();
        InAnimation = false;
    }

    public void ForceOpen()
    {
        if (InAnimation)
            return;

        blind.transform.position = blindOpen;
        lerpFactor = 1f;
    }

    public void ForceClose()
    {
        if (InAnimation)
            return;

        blind.transform.position = blindClosed;
        lerpFactor = 0f;
    }

    public void OpenAnim()
    {
        if (InAnimation)
            return;
        if (lerpFactor >= 1f)
            return;
        DoorOpenAnim();
    }

    public void CloseAnim()
    {
        if (InAnimation)
            return;
        if (lerpFactor <= 0f)
            return;
        DoorCloseAnim();
    }

    public async UniTask OpenCo()
    {
        while ((lerpFactor < 1f)||(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))
        {
            blind.transform.position = Vector3.Lerp(blindClosed, blindOpen, lerpFactor);
            await Task.Yield();
        }
    }

    public async UniTask CloseCo()
    {
        //float startAnimTime = Time.time;
        while ((lerpFactor > 0f)||(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))
        {
            blind.transform.position = Vector3.Lerp(blindClosed, blindOpen, lerpFactor);
            await Task.Yield();
        }
    }

    async UniTaskVoid DoorCloseAnim()
    {
        InAnimation = true;
        animator.SetTrigger(CloseAnimParm);
        await CloseCo();
        OnCloseCB?.Invoke();
        InAnimation = false;
    }

    async UniTaskVoid DoorOpenAnim()
    {
        InAnimation = true;
        animator.SetTrigger(OpenAnimParm);
        await OpenCo();
        OnOpenCB?.Invoke();
        InAnimation = false;
    }
}
