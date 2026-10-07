using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class InputBasics : MonoBehaviour
{
    public event Action PressBeginEvent;
    public event Action PressHoldEvent;
    public event Action PressEndEvent;

    public bool ignoreStartOnUI = true;
    const int UILayer = 5;

    [Title("Press")]
    [ReadOnly] public bool isPressing;
    bool justPressed;
    bool justReleased;

    [ReadOnly] public float pressTimer;
    [ReadOnly] public float lastTimer;
    float pressTime;

    [ReadOnly] public Vector2 screenPosition;
    [ReadOnly] public Vector2 pressPosition;
    [ReadOnly] public bool touchBeganOnUI;

    [Title("Pull")]
    [ReadOnly] public Vector2 pullVector;
    [ReadOnly] public float pullAngle;

    [Title("Swipe")]
    public float swipeDuration = 0.3f;
    public float minSwipeLength = 0.1f;
    [ReadOnly] public bool hasSwiped;
    [ReadOnly] public Vector2 swipeVector;
    [ReadOnly] public float swipeLength;
    [ReadOnly] public float swipeAngle;

    [Title("Tap")]
    public float tapTimeout = 0.25f;
    [ReadOnly] public bool isTapping;
    [ReadOnly] public int consecutiveTaps;
    [ReadOnly] public float timeBetweenTaps;

    [Title("Touch")]
    [ReadOnly] public int activeTouchCount;

    [Title("Debug")]
    [ReadOnly] public bool debug_OnUI;
    [ReadOnly] public float debug_pullLength;
    [ReadOnly] public float debug_screenDiagonal;

    // Singleton
    static InputBasics instance;
    public static InputBasics Instance => instance = instance != null ? instance : FindFirstObjectByType<InputBasics>();

    public static Vector2 ScreenPosition_ => Instance.screenPosition;

    public static bool IsPressing_ => Instance.isPressing && !Instance.OnUI;
    public static bool JustPressed_ => Instance.justPressed && !Instance.OnUI;
    public static bool JustReleased_ => Instance.justReleased && !Instance.OnUI;
    public static bool JustTapped_ => JustReleased_ && Instance.isTapping && !Instance.OnUI;
    public static bool JustSwiped_ => Instance.justReleased && Instance.hasSwiped && !Instance.OnUI;

    public static int ActiveTouchCount_ => Instance.activeTouchCount;
    public static bool HasMultipleTouches_ => Instance.activeTouchCount > 1;

    public static float PressTimer_ => Instance.pressTimer;

    public static Vector2 PullDirection_ => Instance.pullVector.normalized;
    public static float PullLength_ => Instance.pullVector.magnitude / ScreenDiagonal_;
    public static Vector2 SwipeDirection_ => Instance.swipeVector.normalized;
    public static float SwipeLength_ => Instance.swipeLength;

    public static float ScreenDiagonal_ => Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);

    bool OnUI => ignoreStartOnUI && touchBeganOnUI;

    void Update()
    {
        justPressed = false;
        justReleased = false;

        activeTouchCount = Touch.activeTouches.Count;

        debug_OnUI = OnUI;
        debug_pullLength = PullLength_;
        debug_screenDiagonal = ScreenDiagonal_;

        bool hasTouch = activeTouchCount > 0;
        bool pointerDown = false;
        bool pointerHeld = false;
        bool pointerUp = false;

        if (hasTouch)
        {
            Touch touch = Touch.activeTouches[0];
            screenPosition = touch.screenPosition;

            TouchPhase phase = touch.phase;
            pointerDown = phase == TouchPhase.Began;
            pointerHeld = phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
            pointerUp = phase == TouchPhase.Ended || phase == TouchPhase.Canceled;
        }
        else if (Pointer.current != null)
        {
            Pointer pointer = Pointer.current;
            screenPosition = pointer.position.ReadValue();
            pointerDown = pointer.press.wasPressedThisFrame;
            pointerHeld = pointer.press.isPressed;
            pointerUp = pointer.press.wasReleasedThisFrame;
        }

        if (pointerDown) PressBegin();
        else if (pointerHeld) PressHold();
        else if (pointerUp) PressEnd();
    }

    void PressBegin()
    {
        pullVector = Vector2.zero;
        swipeVector = Vector2.zero;
        swipeLength = 0f;
        swipeAngle = 0f;
        hasSwiped = false;

        isPressing = true;
        justPressed = true;

        touchBeganOnUI = false;
        if (EventSystem.current != null)
        {
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = screenPosition }, results);
            touchBeganOnUI = results.Exists(result => result.gameObject.layer == UILayer);
        }

        float oldPressTime = pressTime;
        pressTime = Time.time;
        pressTimer = 0f;

        timeBetweenTaps = pressTime - oldPressTime;
        if (timeBetweenTaps > tapTimeout)
        {
            isTapping = false;
            consecutiveTaps = 0;
        }

        pressPosition = screenPosition;

        if (!OnUI) PressBeginEvent?.Invoke();
    }

    void PressHold()
    {
        pressTimer = Time.time - pressTime;
        pullVector = screenPosition - pressPosition;
        pullAngle = Mathf.Atan2(pullVector.y, pullVector.x) * Mathf.Rad2Deg;

        if (!OnUI) PressHoldEvent?.Invoke();
    }

    void PressEnd()
    {
        isPressing = false;
        justReleased = true;
        lastTimer = Time.time - pressTime;

        swipeVector = screenPosition - pressPosition;
        swipeAngle = Mathf.Atan2(swipeVector.x, swipeVector.y) * Mathf.Rad2Deg;
        swipeLength = swipeVector.magnitude / ScreenDiagonal_;

        bool swipeLengthMoved = swipeLength > minSwipeLength;

        if (swipeLengthMoved && lastTimer <= swipeDuration)
            hasSwiped = true;

        if (pressTimer < tapTimeout && !swipeLengthMoved)
        {
            isTapping = true;
            consecutiveTaps++;
        }
        else
        {
            isTapping = false;
            consecutiveTaps = 0;
        }

        pressTimer = 0f;

        if (!OnUI) PressEndEvent?.Invoke();
    }

    // Vereist voor Touch.activeTouches
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
}
