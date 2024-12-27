using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteToggle : MonoBehaviour, IPointerDownHandler
{
    public Sprite spriteOn;
    public Sprite spriteOff;

    public SpriteRenderer spriteRenderer;
    private bool isOn;

    public void OnPointerDown(PointerEventData eventData)
    {
        isOn = !isOn;
        spriteRenderer.sprite = isOn ? spriteOn : spriteOff;
    }
}
