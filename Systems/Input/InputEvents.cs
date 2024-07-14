using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Add listeners to Pointer events in Inspector
// Works with Images and Collider2D
// Add Graphic Raycaster to Canvas GameObject with Images
// Add Physics 2D Raycaster to Camera GameObject with Collider2D
// Add EventSystem and SIM to Hierarchy
// Only top image is triggered

public class InputEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public UnityEvent OnDown;
    [SerializeField] public UnityEvent OnUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnDown != null)
            OnDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnUp != null)
            OnUp.Invoke();
    }
}