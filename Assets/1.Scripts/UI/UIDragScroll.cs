using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Vertical-only UI scroll with drag, inertia, and optional bounce.
/// Designed for custom stencil or masked UI panels.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIDragScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Scrolling")]
    public RectTransform content;       // The transform that moves
    public RectTransform viewArea;      // The visible area (mask or stencil)
    public float scrollDamping = 5f;    // Higher = less momentum
    public float bounceStrength = 80f;  // Spring-back force when out of bounds
    public bool enableBounce = true;

    private Vector2 velocity;
    private bool dragging;

    private float upperLimit;   // max anchoredPosition.y (typically 0)
    private float lowerLimit;   // min anchoredPosition.y (negative when content > view)

    // Rolling velocity tracking during drag
    private Vector2 pointerStartLocalPos;
    private Vector2 contentStartPos;
    private Vector2 prevPointerLocalPos;
    private float   prevDragTime;

    void Start()
    {
        if (content == null)
            content = transform as RectTransform;

        if (viewArea != null)
            CalculateBounds();
    }

    // Call this whenever the content height changes (e.g. after populating a list).
    public void CalculateBounds()
    {
        float viewHeight    = viewArea.rect.height;
        float contentHeight = content.rect.height;

        upperLimit = 0f;
        // Clamp to 0 so lowerLimit <= upperLimit when content is shorter than the view.
        lowerLimit = Mathf.Min(0f, viewHeight - contentHeight);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        velocity = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            viewArea, eventData.position, eventData.pressEventCamera, out pointerStartLocalPos);

        contentStartPos      = content.anchoredPosition;
        prevPointerLocalPos  = pointerStartLocalPos;
        prevDragTime         = Time.unscaledTime;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            viewArea, eventData.position, eventData.pressEventCamera, out localPoint);

        // Rolling velocity: blend instant velocity into a smoothed estimate so that
        // a zero-delta final event (finger lift) doesn't kill inertia.
        float dt = Time.unscaledTime - prevDragTime;
        if (dt > 0f)
        {
            Vector2 instantVel = (localPoint - prevPointerLocalPos) / dt;
            velocity = Vector2.Lerp(velocity, instantVel, 0.5f);
        }
        prevPointerLocalPos = localPoint;
        prevDragTime        = Time.unscaledTime;

        float newY = contentStartPos.y + (localPoint - pointerStartLocalPos).y;

        if (enableBounce)
        {
            // Rubber-band: resistance grows the further past the limit we drag.
            if (newY > upperLimit)
                newY = upperLimit + (newY - upperLimit) * 0.3f;
            else if (newY < lowerLimit)
                newY = lowerLimit + (newY - lowerLimit) * 0.3f;
        }
        else
        {
            newY = Mathf.Clamp(newY, lowerLimit, upperLimit);
        }

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging  = false;
        velocity.x = 0f; // lock horizontal inertia; y is already the rolling average
    }

    void Update()
    {
        if (dragging)
            return;

        float posY       = content.anchoredPosition.y;
        bool  outOfBounds = posY > upperLimit || posY < lowerLimit;

        if (Mathf.Abs(velocity.y) < 0.01f && !outOfBounds)
            return;

        posY += velocity.y * Time.unscaledDeltaTime;

        if (enableBounce)
        {
            // Spring force proportional to overshoot distance pushes back toward the limit.
            if (posY > upperLimit)
                velocity.y -= (posY - upperLimit) * bounceStrength * Time.unscaledDeltaTime;
            else if (posY < lowerLimit)
                velocity.y -= (posY - lowerLimit) * bounceStrength * Time.unscaledDeltaTime;
        }
        else
        {
            if (posY > upperLimit) { posY = upperLimit; velocity.y = 0f; }
            else if (posY < lowerLimit) { posY = lowerLimit; velocity.y = 0f; }
        }

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, posY);
        velocity = Vector2.Lerp(velocity, Vector2.zero, scrollDamping * Time.unscaledDeltaTime);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (viewArea != null && content != null)
            CalculateBounds();
    }
#endif
}
