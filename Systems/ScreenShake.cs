using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

// Set as child of Camera Follow and include Camera in this or child object

public class ScreenShake : MonoBehaviour
{
    [Title("Shake Settings")]
    [Range(0.1f, 30f), Tooltip("How quickly the shake moves")]
    public float frequency = 10f;
    
    [ Range(0f, 2f), Tooltip("Maximum position offset")]
    public float maxAmplitude = 0.5f;
    
    [Tooltip("Animation curve controlling shake strength over time (0-1 on Y axis)")]
    public AnimationCurve amplitudeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    [Range(0f, 2f), Tooltip("Duration of the shake in seconds")]
    public float shakeDuration = 0.5f;
    
    [Tooltip("Whether to also rotate the camera during shake")]
    public bool useRotation = true;
    
    [ShowIf("useRotation"), Range(0f, 15f), Tooltip("Maximum rotation angle")]
    public float maxRotationStrength = 5f;

    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 noiseOffset;
    Coroutine currentShakeCoroutine;
    
    [Button]
    public void AddShake(float strength = 1f)
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
        }
        currentShakeCoroutine = StartCoroutine(ShakeRoutine(strength));
    }
    
    public void StopShake()
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            currentShakeCoroutine = null;
        }
        
        transform.SetLocalPositionAndRotation(originalPosition, originalRotation);
    }

    IEnumerator ShakeRoutine(float strength)
    {
        noiseOffset = new Vector3(Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f));

        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            float currentStrength = amplitudeCurve.Evaluate(elapsedTime / shakeDuration) * strength;
            ApplyShake(currentStrength);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Clean up when shake is finished
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentShakeCoroutine = null;
    }
    
    void ApplyShake(float strength)
    {
        // Calculate position offset
        Vector3 shakeOffset = CalculateShakeOffset(strength);
        transform.localPosition = originalPosition + shakeOffset;
        
        // Calculate rotation if enabled
        if (useRotation)
        {
            Quaternion shakeRotation = CalculateShakeRotation(strength);
            transform.localRotation = originalRotation * shakeRotation;
        }
    }
    
    Vector3 CalculateShakeOffset(float strength)
    {
        float time = Time.time;
        
        Vector3 offset;
        offset.x = Mathf.PerlinNoise(time * frequency + noiseOffset.x, 0f) * 2f - 1f;
        offset.y = Mathf.PerlinNoise(time * frequency + noiseOffset.y, 0f) * 2f - 1f;
        offset.z = Mathf.PerlinNoise(time * frequency + noiseOffset.z, 0f) * 2f - 1f;
        
        return maxAmplitude * strength * offset;
    }
    
    Quaternion CalculateShakeRotation(float strength)
    {
        float angle = maxRotationStrength * strength;
        
        return Quaternion.Euler(
            Random.Range(-angle, angle),
            Random.Range(-angle, angle),
            Random.Range(-angle, angle)
        );
    }
}