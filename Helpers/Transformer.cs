using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Scripts;

public class Transformer : MonoBehaviour
{    
    public bool Translate;
    [ShowIf("Translate")] 
    public Settings translation;

    public bool Rotate;
    [ShowIf("Rotate")] 
    public Settings rotation;

    public bool Scale;
    [ShowIf("Scale")] 
    public Settings scaling;

    [Serializable]
    public class Settings
    {
        public Vector3 direction = Vector3.one;
        public float amplitude = 1f;
        public float rads = 1f;
        public Functions.Enum function = Functions.Enum.One;
    }

    void Update()
    {
        if (Translate)
            ApplyTransformation(TranslateAction, translation);
        if (Rotate)
            ApplyTransformation(RotateAction, rotation);
        if (Scale)
            ApplyTransformation(ScaleAction, scaling);
    }

    void ApplyTransformation(Action<Vector3> transformAction, Settings settings)
    {
        float T = Time.time * settings.rads;
        float value = Functions.Invoke(settings.function, T);
        float adjustedValue = value * settings.amplitude;
        Vector3 transformation = settings.direction.normalized * adjustedValue;

        transformAction(transformation);
    }

    void TranslateAction(Vector3 translation)
    {
        transform.Translate(translation, Space.World);
    }
    void RotateAction(Vector3 rotation)
    {
        transform.Rotate(rotation, Space.World);
    }

    void ScaleAction(Vector3 scale)
    {
        transform.localScale = Vector3.one * scaling.amplitude + scale;
    }

    public void RelativeTranslate(Transform objectToMove)
    {
        if (!Translate) return;

        float T = Time.time * translation.rads;
        float value = Functions.Invoke(translation.function, T);
        float adjustedValue = value * translation.amplitude;
        Vector3 translationVector = translation.direction.normalized * adjustedValue;

        // Convert the world space translation to local space relative to this transformer
        Vector3 localTranslation = transform.InverseTransformDirection(translationVector);
        
        // Apply the translation in local space
        objectToMove.Translate(localTranslation, Space.Self);
    }

    public void RelativeRotate(Transform objectToRotate)
    {
        if (!Rotate) return;

        float T = Time.time * rotation.rads;
        float value = Functions.Invoke(rotation.function, T);
        float adjustedValue = value * rotation.amplitude;
        Vector3 rotationVector = rotation.direction.normalized * adjustedValue;

        // Convert the world space rotation to local space relative to this transformer
        Vector3 localRotation = transform.InverseTransformDirection(rotationVector);
        
        // Apply the rotation in local space
        objectToRotate.Rotate(localRotation, Space.Self);
    }

}