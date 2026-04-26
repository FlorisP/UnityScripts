using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnPointerDownEvent : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onPointerDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }
}
