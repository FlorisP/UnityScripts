using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Scripts;

[Serializable]
public class DynamicObject
{
    public Transform main;
    public Transform copy;
}
    
public class CopySystem : MonoBehaviour
{    
    [Title("Objects")]
    public Transform StaticCopyParent;
    public List<Transform> staticTransforms; // Initializes copies
    public List<DynamicObject> dynamicTransforms; // Preset copies

    [Title("Parameters")]
    public Vector3 copyVector = new(5, 0, 0);
    public Vector3 centerPoint = Vector3.zero;
    public bool isTwoSided = true;

    [Button] public void CopyStaticTransforms()
    {
        Objects.DestroyChildren(StaticCopyParent);

        foreach (Transform tf in staticTransforms)
        {
            if(isTwoSided){
                Instantiate(tf.gameObject, tf.position - copyVector, tf.rotation, StaticCopyParent);
                Instantiate(tf.gameObject, tf.position + copyVector, tf.rotation, StaticCopyParent);
            }
            else{
                GameObject copyGO = Instantiate(tf.gameObject, tf.position + copyVector, tf.rotation, StaticCopyParent);
                SetPosition(tf, copyGO.transform);
            }
        }
    }

    void LateUpdate()
    {
        // Dynamic Update
        for (int i = 0; i < dynamicTransforms.Count; i++)
            SetPosition(dynamicTransforms[i].main, dynamicTransforms[i].copy);
    }

    public bool SetPosition(Transform main, Transform copy = null)
    {
        Vector3 mainPosition = ClosestPosition(main.position);
        bool changedMain = main.position != mainPosition;
        main.position = mainPosition;

        if(copy != null)
            SetCopy(main, copy);

        return changedMain;
    }

    public void SetCopy(Transform main, Transform copy)
    {
        int sign = (int)Mathf.Sign(Vector3.Dot(centerPoint - main.position, copyVector));
        Vector3 copyPosition = main.position + sign * copyVector;
        copy.SetPositionAndRotation(copyPosition, main.localRotation);
        copy.localScale = main.localScale;  
    }

    Vector3 ClosestPosition(Vector3 currentPosition)
    {
        Vector3 offset = currentPosition - centerPoint;
        Vector3 projection = Vector3.Project(offset, copyVector);
        float unitsAway = Mathf.Round(Vector3.Dot(projection, copyVector) / Vector3.Dot(copyVector, copyVector));
        return currentPosition - unitsAway * copyVector;
    }
}