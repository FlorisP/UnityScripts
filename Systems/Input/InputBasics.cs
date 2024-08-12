using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/*
Event System required
Using InputBasicsEditor script

Set Project Settings => Script Execution Order (before other scripts)
*/

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

    [ReadOnly] public Vector2 screenPosition;
    [ReadOnly] public Vector2 screenStartPosition;
    [ReadOnly] public bool touchBeganOnUI;

    [ReadOnly] public Vector2 dragVector;
    [ReadOnly] public float dragLength;
    [ReadOnly] public float dragAngle;

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

    List<TouchData> touchData = new List<TouchData>();    
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

    // Singleton
    static InputBasics instance;
    public static InputBasics Instance
    {   
        get {
            if (instance == null)
                instance = FindObjectOfType<InputBasics>();
            return instance;
        }
    }

    [Title("Debug")]
    public bool ignoreStartOnUI = true;

    // Drag and Swipe indicator circle shown in Editor script
    [ReadOnly] public float screenDiagonal;

    void Awake()
    {
        screenDiagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
    }

    void Update()
    {
        justPressed = false;
        justReleased = false; 

        // TouchScreen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            screenPosition = touch.position;
            if (touch.phase == TouchPhase.Began)
                PressBegin();
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                PressHold();
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                PressEnd();
        }
        // Mouse
        else
        {
            screenPosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
                PressBegin();
            else if (Input.GetMouseButton(0))
                PressHold();
            else if (Input.GetMouseButtonUp(0))
                PressEnd();
        }
    }

    void PressBegin()
    {
        touchBeganOnUI = EventSystem.current.IsPointerOverGameObject() || (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
        if (ignoreStartOnUI && touchBeganOnUI) 
            return;

        float oldPressTime = pressTime;
        float oldPressTimer = pressTimer;
        dragVector = Vector2.zero;

        // Press
        isPressing = true;
        justPressed = true;
        pressTime = Time.time;
        pressTimer = 0f;

        // Tap
        timeBetweenTaps = pressTime - oldPressTime;
        if (timeBetweenTaps > tapTimeout)
        {
            isTapping = false;
            consecutiveTaps = 0;
        }

        // Swipe
        screenStartPosition = screenPosition;
        hasSwiped = false;
        touchData = new List<TouchData>{new(screenPosition, pressTimer)};

        PressBeginEvent?.Invoke();
    }

    void PressHold()
    {
        if (ignoreStartOnUI && touchBeganOnUI) 
            return;
        
        // Press
        justPressed = false;
        pressTimer = Time.time - pressTime;
        dragVector = screenPosition - screenStartPosition;
        dragLength = dragVector.magnitude / screenDiagonal;
        dragAngle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;

        // Swipe 
        touchData.Add(new TouchData(screenPosition, pressTimer));

        int index = 0;
        while (touchData[index].Timer < pressTimer - swipeDuration  && index < touchData.Count)
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
        if (ignoreStartOnUI && touchBeganOnUI) 
            return;

        // Tap
        if (pressTimer < tapTimeout){
            isTapping = true;
            consecutiveTaps++;
        }
        else{
            isTapping = false;
            consecutiveTaps = 0;
        }
                
        // Press
        isPressing = false;
        justReleased = true;
        pressTimer = 0;
        lastTimer = Time.time - pressTime;

        // Swipe
        swipeVector = screenPosition - touchData[0].Position;        
        swipeLength = swipeVector.magnitude / screenDiagonal;
        swipeAngle = Mathf.Atan2(swipeVector.x, swipeVector.y) * Mathf.Rad2Deg;

        if (swipeLength > minSwipeLength)
            hasSwiped = true;

        PressEndEvent?.Invoke();                
    }

}
