using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using static Utils;
public class PlayerController : MonoBehaviour
{
    // public RectTransform ui_tracked;
    // public Transform game_tracked;
    PlayerInput playerInput;

    InputAction touchPositionAction;
    InputAction touchPressAction;
    List<IPositionTracker> positionTrackers;
    List<ITapTracker> tapTrackers;
    List<ISwipeTracker> swipeTrackers;

    Vector2 swipeWorldOrigin;
    bool swiperRegistered;
    void Start()
    {
        positionTrackers = new List<IPositionTracker>();
        tapTrackers = new List<ITapTracker>();
        swipeTrackers = new List<ISwipeTracker>();
        Touch.onFingerDown += FingerDown;
        Touch.onFingerUp += FingerUp;
    }

    void Awake()
    {

    }

    void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable(); // mouse
        EnhancedTouch.EnhancedTouchSupport.Enable();
    }
    void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    void FingerDown(EnhancedTouch.Finger finger)
    {
        Vector2 newPos = GetWorldPos(finger.screenPosition);
        swipeWorldOrigin = newPos;
        swiperRegistered = false;
        positionTrackers.ForEach(e => e.OnStartTracking(newPos));
    }

    void FingerUp(EnhancedTouch.Finger finger)
    {
        Vector2 newPos = GetWorldPos(finger.screenPosition);
        positionTrackers.ForEach(e => e.OnStopTracking(newPos));
    }

    void Drag(Touch iTouch)
    {
        Vector2 newPos = GetWorldPos(iTouch.screenPosition);
        positionTrackers.ForEach(e => e.OnPositionChanged(newPos));
    }

    void Tap(Touch iTouch)
    {
        Vector2 tapPos = GetWorldPos(iTouch.screenPosition);
        try
        {
            tapTrackers.ForEach(e =>
            {
                if (e.enabled)
                    e.OnTap(tapPos);
            }
            );
        }
        catch (InvalidOperationException ioe)
        {
            Debug.LogError("InvalidOperationException On PlayerController::Tap. Might happening in minigame switch.");
            Debug.LogError(ioe.ToString());
        }

    }

    void Swipe(Touch iTouch)
    {
        if (swipeTrackers.Count == 0)
            return;

        if (swiperRegistered)
            return;

        Vector2 swipeDir = GetWorldPos(iTouch.screenPosition) - swipeWorldOrigin;
        if (swipeDir.magnitude < 0.05f)
            return;

        float angle = Mathf.Atan2(swipeDir.y, swipeDir.x);
        if ((angle < 45f)&&(angle > 315f))
        {
            swipeTrackers.ForEach( e => e.OnHorizontalSwipe(1f));
        } else if ((angle < 135f)&&(angle > 45f))
        {
            swipeTrackers.ForEach( e => e.OnVerticalSwipe(1f));
        } else if ((angle < 225f)&&(angle > 135f))
        {
            swipeTrackers.ForEach( e => e.OnHorizontalSwipe(-1f));
        } else if ((angle < 315f)&&(angle > 225f)) {
            swipeTrackers.ForEach( e => e.OnVerticalSwipe(-1f));
        } else {
            Debug.LogError("Unknown swipe direction for angle : " + angle);
        }
        swiperRegistered = true;
    }

    public void ClearAllTrackers()
    {
        //positionTrackers.ForEach(e => e.OnStopTracking(newPos));
        positionTrackers.Clear();
        tapTrackers.Clear();
        swipeTrackers.Clear();
    }

    void Update()
    {
        foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches)
        {
            if ((touch.phase == UnityEngine.InputSystem.TouchPhase.Ended) && (touch.tapCount >= 1))
            {
                Tap(touch);
            }
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Swipe(touch);
                Drag(touch);
            }
        }
    }

    public void AddPositionTracker(IPositionTracker iTracker)
    {
        if (!positionTrackers.Contains(iTracker))
        {
            positionTrackers.Add(iTracker);
            iTracker.enabled = true;
        }
    }

    public void RemovePositionTracker(IPositionTracker iTracker)
    {
        if (positionTrackers.Contains(iTracker))
        {
            positionTrackers.Remove(iTracker);
        }
    }
    public void AddTapTracker(ITapTracker iTracker)
    {
        if (!tapTrackers.Contains(iTracker))
        {
            tapTrackers.Add(iTracker);
            iTracker.enabled = true;
        }
    }
    public void RemoveTapTracker(ITapTracker iTracker)
    {
        if (tapTrackers.Contains(iTracker))
        {
            tapTrackers.Remove(iTracker);
        }
    }
    public void EnableTapTracker(ITapTracker iTracker, bool bol)
    {
        if (tapTrackers.Contains(iTracker))
        {
            iTracker.enabled = bol;
        }
    }

    public void AddSwipeTracker(ISwipeTracker iTracker)
    {
        if (!swipeTrackers.Contains(iTracker))
        {
            swipeTrackers.Add(iTracker);
        }
    }
    public void RemoveSwipeTracker(ISwipeTracker iTracker)
    {
        if (swipeTrackers.Contains(iTracker))
        {
            swipeTrackers.Remove(iTracker);
        }
    }
}
