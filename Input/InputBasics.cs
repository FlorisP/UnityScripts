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
    public float tapTimeout = 0.2f;
    [ReadOnly] public bool isTapping;
    [ReadOnly] public int consecutiveTaps;
    [ReadOnly] public float timeBetweenTaps;

    [Title("Debug")]
    [ReadOnly] public bool debug_OnUI;
    [ReadOnly] public float debug_pullLength;
    [ReadOnly] public float debug_screenDiagonal;

    List<TouchData> touchData = new();
    public class TouchData
    {
        public Vector2 Position;
        public float Timer;

        public TouchData(Vector2 position, float timer)
        {
            Position = position;
            Timer = timer;
        }
    }


    // Singleton
    static InputBasics instance;
    public static InputBasics Instance => instance = instance != null ? instance : FindFirstObjectByType<InputBasics>();

    public static bool JustPressed_ => Instance.justPressed && !Instance.OnUI;
    public static bool JustReleased_ => Instance.justReleased && !Instance.OnUI;
    public static bool JustSwiped_ => Instance.justReleased && Instance.hasSwiped && !Instance.OnUI;

    public static Vector2 PullDirection_ => Instance.pullVector.normalized;
    public static float PullLength_ => Instance.pullVector.magnitude / ScreenDiagonal_;
    public static float ScreenDiagonal_ =>  Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);

    public static bool IsPressing_ => Instance.isPressing;
    public static bool JustTapped_ => JustReleased_ && Instance.isTapping;
    public static List<TouchData> TouchData_ => Instance.touchData;

    bool OnUI => ignoreStartOnUI && touchBeganOnUI;

    void Update()
    {
        justPressed = false;
        justReleased = false;

        debug_OnUI = OnUI;
        debug_pullLength = PullLength_;
        debug_screenDiagonal = ScreenDiagonal_;

        bool hasTouch = Touch.activeTouches.Count > 0;
        bool pointerDown;
        bool pointerHeld;
        bool pointerUp;

        if (hasTouch)
        {
            Touch touch = Touch.activeTouches[0];
            screenPosition = touch.screenPosition;

            TouchPhase phase = touch.phase;
            pointerDown = phase == TouchPhase.Began;
            pointerHeld = phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
            pointerUp = phase == TouchPhase.Ended || phase == TouchPhase.Canceled;
        }
        else
        {
            Vector2? pos = TryGetPointerPosition();
            if (pos.HasValue) screenPosition = pos.Value;

            pointerDown = Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
            pointerHeld = Pointer.current != null && Pointer.current.press.isPressed;
            pointerUp = Pointer.current != null && Pointer.current.press.wasReleasedThisFrame;
        }

        if (pointerDown) PressBegin();
        else if (pointerHeld) PressHold();
        else if (pointerUp) PressEnd();
    }

    Vector2? TryGetPointerPosition()
    {
        if (Pointer.current == null) return null;

        if (Pointer.current is Mouse mouse) return mouse.position.ReadValue();
        if (Pointer.current is Pen pen) return pen.position.ReadValue();
        if (Pointer.current is Touchscreen touchscreen) return touchscreen.primaryTouch.position.ReadValue();

        return Pointer.current.position.ReadValue();
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
        touchBeganOnUI = EventSystem.current.IsPointerOverGameObject();

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
        touchData = new List<TouchData> { new(screenPosition, pressTimer) };

        PressBeginEvent?.Invoke();
    }

    void PressHold()
    {
        pressTimer = Time.time - pressTime;
        pullVector = screenPosition - pressPosition;
        pullAngle = Mathf.Atan2(pullVector.y, pullVector.x) * Mathf.Rad2Deg;

        touchData.Add(new TouchData(screenPosition, pressTimer));

        int index = 0;
        while (index < touchData.Count && pressTimer > touchData[index].Timer + swipeDuration)
            index++;
        if (index > 0)
            touchData.RemoveRange(0, index);

        PressHoldEvent?.Invoke();
    }

    void PressEnd()
    {
        if (pressTimer < tapTimeout)
        {
            isTapping = true;
            consecutiveTaps++;
        }
        else
        {
            isTapping = false;
            consecutiveTaps = 0;
        }

        isPressing = false;
        justReleased = true;
        pressTimer = 0;
        lastTimer = Time.time - pressTime;

        swipeVector = screenPosition - touchData[0].Position;
        swipeAngle = Mathf.Atan2(swipeVector.x, swipeVector.y) * Mathf.Rad2Deg;
        swipeLength = swipeVector.magnitude / ScreenDiagonal_;

        if (swipeLength > minSwipeLength)
            hasSwiped = true;

        PressEndEvent?.Invoke();
    }
    
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
}
