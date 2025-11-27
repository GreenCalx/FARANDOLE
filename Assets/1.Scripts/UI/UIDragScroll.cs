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
    public float bounceStrength = 80f;  // Bounce elasticity
    public bool enableBounce = true;

    private Vector2 pointerStartLocalPos;
    private Vector2 contentStartPos;
    private Vector2 velocity;
    private bool dragging;

    private float upperLimit;
    private float lowerLimit;

    void Start()
    {
        if (content == null)
            content = transform as RectTransform;

        if (viewArea != null)
            CalculateBounds();
    }

    public void CalculateBounds()
    {
        float viewHeight = viewArea.rect.height;
        float contentHeight = content.rect.height;

        upperLimit = 0f;
        lowerLimit = viewHeight - contentHeight; // negative if content taller
    }

public void OnBeginDrag(PointerEventData eventData)
{
    dragging = true;
    velocity = Vector2.zero;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        viewArea, eventData.position, eventData.pressEventCamera, out pointerStartLocalPos);
    // Store the content's current position
    contentStartPos = content.anchoredPosition;
    // Adjust pointerStartLocalPos to be relative to the content's top
    pointerStartLocalPos.y += content.anchoredPosition.y;
}


public void OnDrag(PointerEventData eventData)
{
    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        viewArea, eventData.position, eventData.pressEventCamera, out localPoint);
    // Adjust localPoint to be relative to the content's top
    localPoint.y += content.anchoredPosition.y;
    // Calculate delta
    Vector2 delta = localPoint - pointerStartLocalPos;
    // Apply delta to content's Y position
    float newY = contentStartPos.y + delta.y;
    // Apply bounce or clamp
    if (enableBounce)
    {
        if (newY > upperLimit)
            newY = Mathf.Lerp(newY, upperLimit, 0.5f);
        else if (newY < lowerLimit)
            newY = Mathf.Lerp(newY, lowerLimit, 0.5f);
    }
    else
    {
        newY = Mathf.Clamp(newY, lowerLimit, upperLimit);
    }
    // Update content position
    content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
}


    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        velocity = eventData.delta / Time.deltaTime;
        velocity.x = 0f; // lock horizontal inertia
    }

    void Update()
    {
        if (dragging)
            return;

        if (Mathf.Abs(velocity.y) > 0.01f)
        {
            Vector2 pos = content.anchoredPosition;
            pos.y += velocity.y * Time.deltaTime;

            // Bounce or clamp
            if (pos.y > upperLimit)
            {
                if (enableBounce)
                    velocity.y = -velocity.y * 0.5f - (pos.y - upperLimit) * bounceStrength * Time.deltaTime;
                else
                    pos.y = upperLimit;
            }
            else if (pos.y < lowerLimit)
            {
                if (enableBounce)
                    velocity.y = -velocity.y * 0.5f - (pos.y - lowerLimit) * bounceStrength * Time.deltaTime;
                else
                    pos.y = lowerLimit;
            }

            content.anchoredPosition = new Vector2(contentStartPos.x, pos.y);
            velocity = Vector2.Lerp(velocity, Vector2.zero, scrollDamping * Time.deltaTime);
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (viewArea != null)
            CalculateBounds();
    }
#endif
}
