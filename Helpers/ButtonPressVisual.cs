using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Pressed alleen tijdens echte press over de knop.
// Geen hover/selected-visual; drag-away annuleert pressed (Unity Button houdt die anders vast).
[DisallowMultipleComponent]
public class ButtonPressVisual : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    Button button;
    Animator animator;
    GameObject highlight;
    GameObject normal;
    GameObject depressed;
    RectTransform content;
    Vector2 contentRest;
    bool held;

    void Awake()
    {
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();

        var nav = button.navigation;
        nav.mode = Navigation.Mode.None;
        button.navigation = nav;
        button.transition = Selectable.Transition.None;

        if (animator != null)
            animator.enabled = false;

        highlight = FindChild("Highlight");
        normal = FindChild("Image (normal)");
        depressed = FindChild("Image (depressed)");
        var contentGo = FindChild("Content");
        if (contentGo != null)
        {
            content = contentGo.transform as RectTransform;
            contentRest = content.anchoredPosition;
        }

        SetPressed(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.IsInteractable())
            return;
        held = true;
        SetPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!held)
            return;
        held = false;
        SetPressed(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!held)
            return;
        held = false;
        SetPressed(false);
    }

    void OnDisable()
    {
        held = false;
        SetPressed(false);
    }

    void SetPressed(bool pressed)
    {
        if (highlight != null)
            highlight.SetActive(false);
        if (normal != null)
            normal.SetActive(!pressed);
        if (depressed != null)
            depressed.SetActive(pressed);
        if (content != null)
            content.anchoredPosition = pressed ? contentRest + new Vector2(0f, -11.5f) : contentRest;
    }

    GameObject FindChild(string childName)
    {
        var t = transform.Find(childName);
        if (t != null)
            return t.gameObject;
        foreach (var c in GetComponentsInChildren<Transform>(true))
        {
            if (c != transform && c.name == childName)
                return c.gameObject;
        }
        return null;
    }
}
