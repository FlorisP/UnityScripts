using UnityEngine;
using MoreMountains.NiceVibrations;

public enum Haptics { UI, Soft, Light, Medium, Rigid, Heavy }

public class HapticsSystem : MonoBehaviour
{
    public bool hapticsEnabled = true;

    // Singleton
    static HapticsSystem _instance;
    public static HapticsSystem Instance => _instance = _instance != null ? _instance : FindFirstObjectByType<HapticsSystem>();

    public static bool HapticsEnabled_{ get => Instance.hapticsEnabled; set => Instance.hapticsEnabled = value; }

    public static void Vibrate_(Haptics level) => Instance.Vibrate(level);
    public static void Transient_(float intensity, float sharpness) => Instance.Transient(intensity, sharpness);

    public static void UISuccess_() => Instance.Vibrate(Haptics.UI);
    public static void UIFailure_() => Instance.Vibrate(Haptics.Light);


    void Vibrate(Haptics level)
    {
        switch (level)
        {
            case Haptics.UI:    TransientPlatform(0.05f, 0.25f, 0.08f, 0.25f); break;
            case Haptics.Soft:   TransientPlatform(0.10f, 0.15f, 0.20f, 0.15f); break;
            case Haptics.Light:  TransientPlatform(0.22f, 0.30f, 0.40f, 0.30f); break;
            case Haptics.Medium: TransientPlatform(0.45f, 0.45f, 0.70f, 0.45f); break;
            case Haptics.Rigid:  TransientPlatform(0.70f, 0.95f, 1.00f, 0.95f); break;
            case Haptics.Heavy:  TransientPlatform(0.95f, 0.55f, 1.00f, 0.60f); break;
        }
    }

    void Transient(float intensity, float sharpness)
    {
        if (!hapticsEnabled)
            return;

        MMVibrationManager.TransientHaptic(intensity, sharpness);
    }

    void TransientPlatform(float iosIntensity, float iosSharpness, float androidIntensity, float androidSharpness)
    {
        if (!hapticsEnabled)
            return;

        MMVibrationManager.TransientHaptic(
            true,
            iosIntensity,
            iosSharpness,
            true,
            androidIntensity,
            androidSharpness,
            true,
            false
        );
    }

    void Continuous(float intensity = 1f, float sharpness = 0.5f, float duration = 0.3f)
    {
        if (!hapticsEnabled)
            return;

        MMVibrationManager.ContinuousHaptic(intensity, sharpness, duration);
    }
}
