using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class DynamicObject
{
    public Transform mainTf;
    public GameObject prefab;
}
    
public class WorldCopy : MonoBehaviour
{    
    [Title("Objects")]
    public List<Transform> staticTransforms;
    public List<Transform> dynamicTransforms;
    public List<DynamicObject> dynamicObjects; // For more complicated objects (hierarchy or scripts)

    [Title("Parents")]
    public Transform StaticCopyParent;
    public Transform DynamicCopyParent;

    [Title("Parameters")]
    public Vector3 copyVector = new(5, 0, 0);
    public Vector3 centerPoint = Vector3.zero;

    [Title("Active Copies")]
    [SerializeField, ReadOnly] List<Transform> dynamicTransformCopies;
    [SerializeField, ReadOnly] List<Transform> dynamicObjectCopies;

    void Awake()
    {
        // Static Init
        foreach (Transform tf in staticTransforms)
        {
            GameObject copyGO = Instantiate(tf.gameObject, tf.position + copyVector, tf.rotation, StaticCopyParent);

            SetPositions(tf, copyGO.transform);
        }

        // Dynamic Init
        dynamicTransformCopies = new ();
        foreach (Transform tf in dynamicTransforms)
        {
            GameObject copy = Instantiate(tf.gameObject, tf.position, tf.rotation, DynamicCopyParent);
            copy.transform.localScale = tf.localScale;
            dynamicTransformCopies.Add(copy.transform);
        }

        dynamicObjectCopies = new ();
        foreach (DynamicObject obj in dynamicObjects)
        {
            GameObject copy = Instantiate(obj.prefab, obj.mainTf.position, obj.mainTf.rotation, DynamicCopyParent);
            copy.transform.localScale = obj.mainTf.localScale;
            dynamicObjectCopies.Add(copy.transform);
        }
    }

    void Update()
    {
        // Dynamic Update
        for (int i = 0; i < dynamicTransforms.Count; i++)
            SetPositions(dynamicTransforms[i], dynamicTransformCopies[i]);
        for (int i = 0; i < dynamicObjects.Count; i++)
            SetPositions(dynamicObjects[i].mainTf, dynamicObjectCopies[i]);
    }

    void SetPositions(Transform main, Transform copy)
    {
            // Main Position
            Vector3 centerOffset = main.position - centerPoint;
            Vector3 projection = Vector3.Project(centerOffset, copyVector);
            float unitsAway = Vector3.Dot(projection, copyVector) /  Vector3.Dot(copyVector, copyVector);
            Vector3 mainPosition = main.position - Mathf.Round(unitsAway) * copyVector;

            // Copy Position
            float sign = Mathf.Sign(Vector3.Dot(centerPoint - mainPosition, copyVector));
            Vector3 copyPosition = mainPosition + sign * copyVector;

            // Set values
            main.position = mainPosition;
            copy.SetPositionAndRotation(copyPosition, main.localRotation);
            copy.localScale = main.localScale;  
    }
}