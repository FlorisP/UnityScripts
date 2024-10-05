using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Scripts;

public class Transformer : MonoBehaviour
{    
    public bool Translate;
        [ShowIf("Translate")] public Settings translation;

    public bool Rotate;
        [ShowIf("Rotate")] public Settings rotation;

    public bool Scale;
        [ShowIf("Scale")] public Settings scaling;

    [Serializable]
    public class Settings
    {
        public Vector3 direction = Vector3.one;
        public float amplitude = 1f;
        public float speed = 0.2f;
        public Functions.Enum function = Functions.Enum.Sine;
    }

    void Update()
    {
        if (Translate)
            ApplyTransformation(TranslateTransform, translation);
        if (Rotate)
            ApplyTransformation(RotateTransform, rotation);
        if (Scale)
            ApplyTransformation(ScaleTransform, scaling);
    }

    private void ApplyTransformation(Action<Vector3> transformAction, Settings settings)
    {
        float T = Time.time * settings.speed;
        float value = Functions.Invoke(settings.function, T);
        float adjustedValue = value * settings.amplitude * settings.speed;
        Vector3 transformation = settings.direction.normalized * adjustedValue;

        transformAction(transformation);
    }

    private void TranslateTransform(Vector3 translation)
    {
        transform.Translate(translation, Space.World);
    }
    private void RotateTransform(Vector3 rotation)
    {
        transform.Rotate(rotation, Space.World);
    }

    private void ScaleTransform(Vector3 scale)
    {
        transform.localScale = Vector3.one * scaling.amplitude + scale;
    }

}