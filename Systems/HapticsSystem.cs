using UnityEngine;
using MoreMountains.NiceVibrations;

public class HapticsSystem : MonoBehaviour
{
    public bool hapticsEnabled = true;

    const float Sharpness = 0.35f;

    static HapticsSystem _instance;
    public static HapticsSystem Instance => _instance = _instance != null ? _instance : FindFirstObjectByType<HapticsSystem>();

    bool continuousActive;

    // intensity = sterkte (beide platforms). sharpness vast semi-scherp (alleen iOS).
    public static void PlayTick_() => Instance.Transient(0.10f);
    public static void PlayUI_() => Instance.Transient(0.20f);
    public static void PlaySoft_() => Instance.Transient(0.30f);
    public static void PlayLight_() => Instance.Transient(0.40f);
    public static void PlayMedium_() => Instance.Transient(0.55f);
    public static void PlayRigid_() => Instance.Transient(0.70f);
    public static void PlayHeavy_() => Instance.Transient(0.85f);

    // Continuous: start (of intensity bijstellen als al actief) / stop.
    // Intensity-update mid-play vooral iOS; Android = start + cancel.
    public static void StartContinuous_(float intensity, float duration = 2f) =>
        Instance.StartContinuous(intensity, duration);
    public static void StopContinuous_() =>
        Instance.StopContinuous();

    void Transient(float intensity)
    {
        if (!hapticsEnabled)
            return;

        MMVibrationManager.TransientHaptic(
            true, intensity, Sharpness,
            true, intensity, Sharpness,
            false, false, 1f, 1f, -1, this, false);
    }

    void StartContinuous(float intensity, float duration)
    {
        if (!hapticsEnabled)
            return;

        intensity = Mathf.Clamp01(intensity);

        if (continuousActive)
        {
            MMVibrationManager.UpdateContinuousHaptic(intensity, Sharpness);
            return;
        }

        continuousActive = true;
        MMVibrationManager.ContinuousHaptic(
            intensity, Sharpness, duration,
            HapticTypes.None, this, false, -1, false, true);
    }

    void StopContinuous()
    {
        if (!continuousActive)
            return;

        continuousActive = false;
        MMVibrationManager.StopContinuousHaptic();
    }
    
    void OnApplicationPause(bool pause)
    {
        if (pause)
            StopContinuous();
    }
}
