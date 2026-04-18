using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputBasics : MonoBehaviour
{
    public event Action PressBeginEvent;
    public event Action PressHoldEvent;
    public event Action PressEndEvent;

    [Title("Press")]
    [ReadOnly] public bool isPressing;
    [ReadOnly] public bool justPressed;
    [ReadOnly] public bool justReleased;
    [ReadOnly] public float pressTimer;
    [ReadOnly] public float lastTimer;
    float pressTime;

    [Title("Pull")]
    [ReadOnly] public bool touchBeganOnUI;
    [ReadOnly] public Vector2 screenPosition;
    [ReadOnly] public Vector2 screenStartPosition;
    [ReadOnly] public Vector2 pullVector;
    [ReadOnly] public float pullAngle;

    [Title("Tap")]
    public float tapTimeout = 0.2f;
    [ReadOnly] public bool isTapping;
    [ReadOnly] public int consecutiveTaps;
    [ReadOnly] public float timeBetweenTaps;

    [Title("Swipe")]
    public float swipeDuration = 0.3f;
    public float minSwipeLength = 0.1f;
    [ReadOnly] public bool hasSwiped;
    [ReadOnly] public Vector2 swipeVector;
    [ReadOnly] public float swipeLength;
    [ReadOnly] public float swipeAngle;

    [Title("Debug")]
    public bool ignoreStartOnUI = true;
    [ShowInInspector] public float ScreenDiagonal => Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
    [ShowInInspector] public float RelativePullLength => pullVector.magnitude / ScreenDiagonal;

    List<TouchData> touchData = new();
    class TouchData
    {
        public Vector2 Position;
        public float Timer;

        public TouchData(Vector2 position, float timer)
        {
            Position = position;
            Timer = timer;
        }
    }

    static InputBasics instance;
    public static InputBasics Instance => instance ??= FindFirstObjectByType<InputBasics>();

    public bool OnUI => Instance.ignoreStartOnUI && Instance.touchBeganOnUI;
    public static bool JustPressed_ => Instance.justPressed && !Instance.OnUI;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        justPressed = false;
        justReleased = false;

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
        touchBeganOnUI = EventSystem.current.IsPointerOverGameObject();

        float oldPressTime = pressTime;
        pullVector = Vector2.zero;

        isPressing = true;
        justPressed = true;
        pressTime = Time.time;
        pressTimer = 0f;

        timeBetweenTaps = pressTime - oldPressTime;
        if (timeBetweenTaps > tapTimeout)
        {
            isTapping = false;
            consecutiveTaps = 0;
        }

        screenStartPosition = screenPosition;
        hasSwiped = false;
        touchData = new List<TouchData> { new(screenPosition, pressTimer) };

        PressBeginEvent?.Invoke();
    }

    void PressHold()
    {
        pressTimer = Time.time - pressTime;
        pullVector = screenPosition - screenStartPosition;
        pullAngle = Mathf.Atan2(pullVector.y, pullVector.x) * Mathf.Rad2Deg;

        touchData.Add(new TouchData(screenPosition, pressTimer));

        int index = 0;
        while (index < touchData.Count && touchData[index].Timer < pressTimer - swipeDuration)
        {
            index++;
        }
        if (index > 0)
        {
            touchData.RemoveRange(0, index);
        }

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
        swipeLength = swipeVector.magnitude / ScreenDiagonal;
        swipeAngle = Mathf.Atan2(swipeVector.x, swipeVector.y) * Mathf.Rad2Deg;

        if (swipeLength > minSwipeLength)
            hasSwiped = true;

        PressEndEvent?.Invoke();
    }
}
