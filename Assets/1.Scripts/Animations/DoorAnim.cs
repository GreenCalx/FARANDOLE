using UnityEngine;
using System.Threading.Tasks;
public class DoorAnim : MonoBehaviour
{
    public bool startClosed = true;
    [Range(0f, 1f)]
    public float lerpFactor;
    public Transform left;
    public Transform right;
    float animationDuration;
    float half_w;
    private bool InAnimation;
    Vector3 leftClosedPos, rightClosedPos;
    Vector3 leftOpenPos, rightOpenPos;
    public void Init(Transform iLeft, Transform iRight, float iXOffset)
    {
        half_w = Screen.width / 2f;
        animationDuration = 0.5f * GameData.GetSettings.PreMiniGameLatchInMs / 1000f;
        // leftImage.rectTransform.sizeDelta = new Vector2(half_w, leftImage.rectTransform.sizeDelta.y);
        // rightImage.rectTransform.sizeDelta = new Vector2(half_w, leftImage.rectTransform.sizeDelta.y);
        left = iLeft;
        leftClosedPos = left.position;
        leftOpenPos = new Vector3(leftClosedPos.x - iXOffset, leftClosedPos.y);

        right = iRight;
        rightClosedPos = right.position;
        rightOpenPos = new Vector3(rightClosedPos.x + iXOffset, rightClosedPos.y);

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

        // leftImage.rectTransform.anchoredPosition = new Vector2(-half_w, 0f);
        // rightImage.rectTransform.anchoredPosition = new Vector2(half_w, 0f);
        left.transform.position = leftOpenPos;
        right.transform.position = rightOpenPos;
        lerpFactor = 1f;
    }

    public void ForceClose()
    {
        if (InAnimation)
            return;

        // leftImage.rectTransform.anchoredPosition = Vector2.zero;
        // rightImage.rectTransform.anchoredPosition = Vector2.zero;
        left.transform.position = leftClosedPos;
        right.transform.position = rightClosedPos;
        lerpFactor = 0f;
    }

    public void ClapAnim()
    {
        if (InAnimation)
            return;
        DoorClapAnim();
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

    async Task OpenCo()
    {
        float startAnimTime = Time.time;
        while (lerpFactor < 1f)
        {
            lerpFactor = (Time.time - startAnimTime) / animationDuration;
            // leftImage.rectTransform.anchoredPosition = new Vector2(-lerpFactor * half_w, 0f);
            // rightImage.rectTransform.anchoredPosition = new Vector2(lerpFactor * half_w, 0f);
            left.transform.position = Vector3.Lerp(leftClosedPos, leftOpenPos, lerpFactor);
            right.transform.position = Vector3.Lerp(rightClosedPos, rightOpenPos, lerpFactor);
            await Task.Yield();
        }
        ForceOpen();
    }

    async Task CloseCo()
    {
        float startAnimTime = Time.time;
        while (lerpFactor > 0f)
        {
            lerpFactor = 1f - ((Time.time - startAnimTime) / animationDuration);
            // leftImage.rectTransform.anchoredPosition = new Vector2(-lerpFactor * half_w, 0f);
            // rightImage.rectTransform.anchoredPosition = new Vector2(lerpFactor * half_w, 0f);
            left.transform.position = Vector3.Lerp(leftClosedPos, leftOpenPos, lerpFactor);
            right.transform.position = Vector3.Lerp(rightClosedPos, rightOpenPos, lerpFactor);
            await Task.Yield();
        }
        ForceClose();
    }

    async void DoorClapAnim()
    {
        InAnimation = true;
        await CloseCo();
        await OpenCo();
        InAnimation = false;
    }

    async void DoorOpenAnim()
    {
        InAnimation = true;
        await OpenCo();
        InAnimation = false;
    }
    async void DoorCloseAnim()
    {
        InAnimation = true;
        await CloseCo();
        InAnimation = false;
    }
}
