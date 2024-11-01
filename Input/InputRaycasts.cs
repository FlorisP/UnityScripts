using UnityEngine;
using System;
using Sirenix.OdinInspector;

// Add InputInteractive to each interactive GameObject
[RequireComponent(typeof(InputBasics))] 
public class InputRaycasts : MonoBehaviour
{
    public bool is2D;
    public int maxHits = -1; // TODO    
    [ReadOnly] public RaycastHit[] hits;

    InputBasics input;
    public event Action<RaycastHit[]> PressBeginEventGOs;
    public event Action<RaycastHit[]> PressHoldEventGOs;
    public event Action<RaycastHit[]> PressEndEventGOs;

    void Awake() 
    {        
        input = GetComponent<InputBasics>();
        input.PressBeginEvent += PressBegin;
        input.PressHoldEvent += PressHold;
        input.PressEndEvent += PressEnd;
    }

    void OnDestroy()
    {
        input.PressBeginEvent -= PressBegin;
        input.PressHoldEvent -= PressHold;
        input.PressEndEvent -= PressEnd;
    }
    
    void UpdateHitList()
    {
        Ray ray = Camera.main.ScreenPointToRay(input.screenPosition);
        hits = Physics.RaycastAll(ray);

        // if (is2D)
        //     hits2D = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
    }

    void PressBegin()
    {
        UpdateHitList();
        PressBeginEventGOs?.Invoke(hits);
        HandleHits(hits, (interactiveComponent, hit) => interactiveComponent.HitBegin(hit));
    }

    void PressHold()
    {
        UpdateHitList();
        PressHoldEventGOs?.Invoke(hits);
        HandleHits(hits, (interactiveComponent, hit) => interactiveComponent.HitHold(hit));
    }

    void PressEnd()
    {                      
        UpdateHitList();
        PressEndEventGOs?.Invoke(hits);
        HandleHits(hits, (interactiveComponent, hit) => interactiveComponent.HitEnd(hit));
    }

    void HandleHits(RaycastHit[] hits, Action<InputInteractive, RaycastHit> hitAction)
    {
        foreach (RaycastHit hit in hits)
        {
            InputInteractive interactiveComponent = hit.collider.gameObject.GetComponent<InputInteractive>();            
            if (interactiveComponent != null)
                hitAction(interactiveComponent, hit);
        }
    }
}