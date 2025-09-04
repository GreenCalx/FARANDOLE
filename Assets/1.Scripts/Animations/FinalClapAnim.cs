using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Threading;
public class FinalClapAnim : MonoBehaviour
{
    public bool startClosed = false;
    [Range(0f, 1f)]
    public float lerpFactor;
    public Transform blind;
    float animationDuration;
    private bool InAnimation;
    Vector3 blindClosed;
    Vector3 blindOpen;
    public UnityEvent OnCloseCB, OnOpenCB;
    public Animator animator;
    public void Init(Transform iBlind, float iYOffset)
    {
        OnCloseCB = new UnityEvent();
        OnOpenCB = new UnityEvent();

        animationDuration = 0.5f * GameData.GetSettings.PreMiniGameLatchInMs / 1000f;

        blind = iBlind;
        blindClosed = blind.position;
        blindOpen = new Vector3(blindClosed.x, blindClosed.y - iYOffset);

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

    public async Task OpenCo()
    {
        float startAnimTime = Time.time;
        while (lerpFactor < 1f)
        {
            lerpFactor = (Time.time - startAnimTime) / animationDuration;
            blind.transform.position = Vector3.Lerp(blindClosed, blindOpen, lerpFactor);
            await Task.Yield();
        }
        ForceOpen();
    }

    public async Task CloseCo()
    {
        float startAnimTime = Time.time;
        while (lerpFactor > 0f)
        {
            lerpFactor = 1f - ((Time.time - startAnimTime) / animationDuration);
            blind.transform.position = Vector3.Lerp(blindClosed, blindOpen, lerpFactor);
            await Task.Yield();
        }
        ForceClose();
    }

    async void DoorCloseAnim()
    {
        InAnimation = true;
        await CloseCo();
        OnCloseCB?.Invoke();
        InAnimation = false;
    }

    async void DoorOpenAnim()
    {
        InAnimation = true;
        await OpenCo();
        OnOpenCB?.Invoke();
        InAnimation = false;
    }
}
