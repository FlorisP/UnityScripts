using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

// TODO Spin, Particles

public class Collectable : MonoBehaviour
{
    public AnimationCurve collectScale;
    public Vector3 originalScale;

    [Button] 
    public void Reset()
    {
        transform.localScale = originalScale;
    }

    bool InPlayMode() { return Application.isPlaying;}
    [Button, EnableIf("InPlayMode")] 
    public void Collect()
    {
        StartCoroutine(ScaleCollectable());
    }

    IEnumerator ScaleCollectable()
    {
        float timer = 0f;
        float totalTime = collectScale.keys[collectScale.length - 1].time; // ~0.15s

        while (timer < totalTime)
        {
            timer += Time.deltaTime;
            float scaleValue = collectScale.Evaluate(timer);
            transform.localScale = originalScale * scaleValue;
            yield return null;
        }
        transform.localScale = originalScale * collectScale.Evaluate(totalTime);
    }
}
