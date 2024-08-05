using UnityEngine;
using Sirenix.OdinInspector;

// TODO Make it work with AnimationCurves and Scripts.Functions

public class Transformer : MonoBehaviour
{    
    public bool translate;
    [SerializeField, ShowIf("translate")]
    public Vector3 direction;
    [SerializeField, ShowIf("translate")]
    public float translationSpeed;

    public bool rotate;
    [SerializeField, ShowIf("rotate")]
    public Vector3 rotationAxis;
    [SerializeField, ShowIf("rotate")]
    public float rotationSpeed;

    public bool scale;
    [SerializeField, ShowIf("scale")]
    public Vector3 minScale;
    [SerializeField, ShowIf("scale")]
    public Vector3 maxScale;

    void Update()
    {
        if (translate)
            Translate(transform);
        if (rotate)
            Rotate(transform);
        if (scale)
            Scale();
    }

    public void Translate(Transform tf)
    {
        tf.localPosition += direction * translationSpeed * Time.deltaTime;
    }

    public void Rotate(Transform tf)
    {
        tf.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
    }

    void Scale()
    {
        // Implement scaling logic here, e.g., oscillate between minScale and maxScale
    }
}
