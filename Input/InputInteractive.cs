using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HitEvent : UnityEvent<RaycastHit> { }

// Add this component to each interactive GO
// InputBasics & InputRaycasts required in project
public class InputInteractive : MonoBehaviour
{
    public HitEvent OnPressBeginGO;
    public HitEvent OnPressHoldGO;
    public HitEvent OnPressEndGO;

    private void Awake()
    {
        OnPressBeginGO ??= new HitEvent();
        OnPressHoldGO ??= new HitEvent();
        OnPressEndGO ??= new HitEvent();
    }

    public void HitBegin(RaycastHit hit)
    {
        OnPressBeginGO.Invoke(hit);
    }

    public void HitHold(RaycastHit hit)
    {
        OnPressHoldGO.Invoke(hit);
    }

    public void HitEnd(RaycastHit hit)
    {
        OnPressEndGO.Invoke(hit);
    }
}