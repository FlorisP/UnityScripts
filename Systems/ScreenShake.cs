using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class ScreenShake : MonoBehaviour
{
    public Transform shakeTransform;
    public bool shakeEnabled = true;

    [Title("Shake Settings")]
    [Range(0f, 2f)] public float duration = 0.15f;
    [Range(0.1f, 30f)] public float frequency = 10f;
    [Range(0f, 2f)] public float amplitude = 1f;
    public AnimationCurve amplitudeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    public bool useRotation = true;
    [ShowIf("useRotation"), Range(0f, 2f)] public float rotationStrength = 1f;

    // Singleton
    static ScreenShake _instance;
    public static ScreenShake Instance => _instance = _instance != null ? _instance : FindFirstObjectByType<ScreenShake>();

    public static bool ShakeEnabled_ { get => Instance.shakeEnabled; set => Instance.shakeEnabled = value; }

    public static void AddShake_(float strength = 1f, float duration = 0.15f, bool unscaled = true, bool overwrite = false) => Instance.AddShake(strength, duration, unscaled, overwrite);
    public static void UIShake_() => Instance.AddShake(1f, _instance.duration);


    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 noiseOffset;
    Coroutine currentShakeCoroutine;
    bool useUnscaledTimeThisShake;

    public void AddShake(float strength, float duration, bool unscaled = true, bool overwrite = false)
    {
        if (!shakeEnabled)
            return;

        if (currentShakeCoroutine != null)
        {
            if(overwrite) StopShake();
            else return;
        }

        useUnscaledTimeThisShake = unscaled;
        currentShakeCoroutine = StartCoroutine(ShakeRoutine(strength, duration));
    }

    public void StopShake()
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            currentShakeCoroutine = null;
        }

        shakeTransform.SetLocalPositionAndRotation(originalPosition, originalRotation);
    }

    IEnumerator ShakeRoutine(float strength, float duration)
    {
        noiseOffset = new Vector3(Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f));

        originalPosition = shakeTransform.localPosition;
        originalRotation = shakeTransform.localRotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float dt = useUnscaledTimeThisShake ? Time.unscaledDeltaTime : Time.deltaTime;
            float time = useUnscaledTimeThisShake ? Time.unscaledTime : Time.time;

            float currentStrength = amplitudeCurve.Evaluate(elapsedTime / duration) * strength;
            ApplyShake(currentStrength, time);

            elapsedTime += dt;
            yield return null;
        }

        shakeTransform.SetLocalPositionAndRotation(originalPosition, originalRotation);
        currentShakeCoroutine = null;
    }

    void ApplyShake(float strength, float time)
    {
        Vector3 shakeOffset = CalculateShakeOffset(strength, time);
        shakeTransform.localPosition = originalPosition + shakeOffset;

        if (useRotation)
        {
            Quaternion shakeRotation = CalculateShakeRotation(strength);
            shakeTransform.localRotation = originalRotation * shakeRotation;
        }
    }

    Vector3 CalculateShakeOffset(float strength, float time)
    {
        Vector3 offset;
        offset.x = Mathf.PerlinNoise(time * frequency + noiseOffset.x, 0f) * 2f - 1f;
        offset.y = Mathf.PerlinNoise(time * frequency + noiseOffset.y, 0f) * 2f - 1f;
        offset.z = Mathf.PerlinNoise(time * frequency + noiseOffset.z, 0f) * 2f - 1f;

        return amplitude * strength * offset;
    }

    Quaternion CalculateShakeRotation(float strength)
    {
        float angle = rotationStrength * strength;

        return Quaternion.Euler(
            Random.Range(-angle, angle),
            Random.Range(-angle, angle),
            Random.Range(-angle, angle)
        );
    }
}
