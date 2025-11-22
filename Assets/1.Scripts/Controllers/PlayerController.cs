using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using static Utils;
using System.Linq;
using UnityEngine.Events;
public class PlayerController : MonoBehaviour
{
    // public RectTransform ui_tracked;
    // public Transform game_tracked;
    PlayerInput playerInput;

    public UnityEvent onInputEvent = new UnityEvent();
    List<IPositionTracker> positionTrackers;
    List<ITapTracker> tapTrackers;
    List<ITapTracker> sortedTapTrackers;
    List<ISwipeTracker> swipeTrackers;
    
    Vector2 firstTouchWorldPos;
    Vector2 lastTouchWorldPos;

    bool freezeInputs = false;

    float lastTimeTouch;
    float firstTimeTouch;

    public bool IsFrozen
    {
        get { return freezeInputs; }
    }
    bool tapTrackersModified = false;
    void Start()
    {
        positionTrackers = new List<IPositionTracker>();
        tapTrackers = new List<ITapTracker>();
        swipeTrackers = new List<ISwipeTracker>();
        Touch.onFingerDown += FingerDown;
        Touch.onFingerUp += FingerUp;

        freezeInputs = false;
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

    public void Flush()
    {
        positionTrackers.Clear();
        tapTrackers.Clear();
        sortedTapTrackers.Clear();
        swipeTrackers.Clear();
    }

    void FingerDown(EnhancedTouch.Finger finger)
    {
        if (freezeInputs)
            return;

        firstTimeTouch = Time.fixedUnscaledTime;
        firstTouchWorldPos = GetWorldPos(finger.screenPosition);
        positionTrackers.ForEach(e => e.OnStartTracking(firstTouchWorldPos));
    }

    void FingerUp(EnhancedTouch.Finger finger)
    {
        if (freezeInputs)
            return;

        lastTimeTouch = Time.fixedUnscaledTime;
        lastTouchWorldPos = GetWorldPos(finger.screenPosition);
        positionTrackers.ForEach(e => e.OnStopTracking(lastTouchWorldPos));
        if (HaveSwipers())
            Swipe();
    }

    void Drag(Touch iTouch)
    {
        if (freezeInputs)
            return;

        Vector2 newPos = GetWorldPos(iTouch.screenPosition);
        positionTrackers.ForEach(e => e.OnPositionChanged(newPos));
    }

    void Tap(Touch iTouch)
    {
        if (freezeInputs)
            return;

        Vector2 tapPos = GetWorldPos(iTouch.screenPosition);
        try
        {
            if (tapTrackersModified)
            {
                tapTrackers = tapTrackers
                .Where(e => (e!=null) && e.enabled)
                .OrderByDescending(e => e.GetDisplayPriority())
                .ToList();
                tapTrackersModified = false;
            }
            foreach (ITapTracker tracker in tapTrackers)
            {
                if (tracker == null)
                    continue;
                if (tracker.OnTap(tapPos) && tracker.stopPropagation)
                {
                    return;
                }
            }   
        }
        catch (InvalidOperationException ioe)
        {
            Debug.LogError("InvalidOperationException On PlayerController::Tap. Might happening in minigame switch.");
            Debug.LogError(ioe.ToString());
        }
    }

    void Swipe()
    {
        if (freezeInputs)
            return;

        Vector2 swipeDir = lastTouchWorldPos - firstTouchWorldPos;
        if (swipeDir.magnitude < 0.01f)
             return; // min swipe length requirement

        if (Mathf.Abs(swipeDir.x) >= Mathf.Abs(swipeDir.y))
            swipeTrackers.ForEach(e => e.OnHorizontalSwipe(swipeDir.x));
        else
            swipeTrackers.ForEach(e => e.OnVerticalSwipe(swipeDir.y));
    }

    public void ClearAllTrackers()
    {
        //positionTrackers.ForEach(e => e.OnStopTracking(newPos));
        positionTrackers.Clear();
        tapTrackers.Clear();
        swipeTrackers.Clear();
    }

    public void Freeze(bool iState) { freezeInputs = iState; }
    public void Freeze() { freezeInputs = true; }
    public void UnFreeze() { freezeInputs = false; }

    void Update()
    {
        foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches)
        {

            if ((touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)) //tapCount >= 1, problemes avec time scale?
            {
                if (freezeInputs)
                {
                    TapInput(); // Call onInputEvent
                    return;
                }
                float deltaTime = lastTimeTouch - firstTimeTouch;
                //Tap sur la même position et moins d'une seconde
                if (deltaTime < 0.1 || ((lastTouchWorldPos - firstTouchWorldPos).sqrMagnitude < 0.05f && deltaTime <= 1))
                    Tap(touch);
                // Swipe(touch);
            }
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Drag(touch);
            }
        }
    }

    bool HavePositionTrackers()
    {
        return ((positionTrackers != null) && (positionTrackers.Count > 0));
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

    bool HaveTappers()
    {
        return ((tapTrackers != null) && (tapTrackers.Count > 0));
    }
    public void AddTapTracker(ITapTracker iTracker)
    {
        if (!tapTrackers.Contains(iTracker))
        {
            tapTrackers.Add(iTracker);
            iTracker.enabled = true;
            tapTrackersModified = true;
        }
    }
    public void RemoveTapTracker(ITapTracker iTracker)
    {
        if (tapTrackers.Contains(iTracker))
        {
            tapTrackers.Remove(iTracker);
            tapTrackersModified = true;
        }
    }
    public void EnableTapTracker(ITapTracker iTracker, bool bol)
    {
        if (tapTrackers.Contains(iTracker))
        {
            iTracker.enabled = bol;
        }
    }

    bool HaveSwipers()
    {
        return ((swipeTrackers != null) && (swipeTrackers.Count > 0));
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

    public void TapInput()
    {
        onInputEvent.Invoke();   
    }
}
