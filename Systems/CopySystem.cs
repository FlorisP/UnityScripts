using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Scripts;

[Serializable]
public class PresetObject
{
    public Transform main;
    public Transform copy;
    public Transform copyTwo;
    public bool isDynamic;
}

public class CopySystem : MonoBehaviour
{
    [Title("Objects")]
    public Transform staticCopyParent;
    public List<Transform> deepCopyTransforms;
    public List<PresetObject> presetTransforms;

    [Title("Parameters")]
    public Vector3 copyVector = new(5, 0, 0);
    public Vector3 centerPoint = Vector3.zero;
    public bool isTwoSided = true;

    // Singleton
    static CopySystem _instance;
    public static CopySystem Instance => _instance = _instance != null ? _instance : FindFirstObjectByType<CopySystem>();

    public static Vector3 CopyVector_ => Instance.copyVector;

    public static void CopyTransforms_() => Instance.CopyTransforms();
    public static bool SetPosition_(Transform main, Transform copy = null, Transform copyTwo = null) => Instance.SetPosition(main, copy, copyTwo);
    public static void SetCopy_(Transform main, Transform copy, Transform copyTwo = null) => Instance.SetCopy(main, copy, copyTwo);
    public static void SetActive_(bool active) => Instance.SetChildrenActive(active);
    public static Vector3 CenterPosition_(Vector3 position, float padding = 0f) => Instance.CenterPosition(position, padding);


    void LateUpdate()
    {
        for (int i = 0; i < presetTransforms.Count; i++)
            if (presetTransforms[i].isDynamic)
                SetPosition(presetTransforms[i].main, presetTransforms[i].copy, presetTransforms[i].copyTwo);
    }

    [Button]
    public void CopyTransforms()
    {
        Objects.DestroyChildren(staticCopyParent);

        // Initialize deep copies
        foreach (Transform tf in deepCopyTransforms)
        {
            if (isTwoSided)
            {
                Instantiate(tf.gameObject, tf.position - copyVector, tf.rotation, staticCopyParent);
                Instantiate(tf.gameObject, tf.position + copyVector, tf.rotation, staticCopyParent);
            }
            else
            {
                GameObject copyGO = Instantiate(tf.gameObject, tf.position + copyVector, tf.rotation, staticCopyParent);
                SetPosition(tf, copyGO.transform);
            }
        }

        // Position preset copies
        for (int i = 0; i < presetTransforms.Count; i++)
        {
            PresetObject preset = presetTransforms[i];
            if (!preset.isDynamic)
            {
                Transform copyTwo = preset.copyTwo != null ? preset.copyTwo :
                    isTwoSided ? Instantiate(preset.copy, Vector3.zero, Quaternion.identity, staticCopyParent) : null;
                SetCopy(preset.main, preset.copy, copyTwo);
            }
        }

    }

    public bool SetPosition(Transform main, Transform copy = null, Transform copyTwo = null)
    {
        Vector3 mainPosition = CenterPosition(main.position);
        bool changedMain = main.position != mainPosition;
        main.position = mainPosition;

        if (copy != null)
            SetCopy(main, copy, copyTwo);

        return changedMain;
    }

    public void SetCopy(Transform main, Transform copy, Transform copyTwo = null)
    {
        int sign = (int)Mathf.Sign(Vector3.Dot(centerPoint - main.position, copyVector));

        Vector3 copyPosition = main.position + sign * copyVector;
        copy.SetPositionAndRotation(copyPosition, main.localRotation);
        copy.localScale = main.localScale;

        if (copyTwo != null)
        {
            Vector3 copyTwoPosition = main.position - sign * copyVector;
            copyTwo.SetPositionAndRotation(copyTwoPosition, main.localRotation);
            copyTwo.localScale = main.localScale;
        }
    }

    public void SetChildrenActive(bool active)
    {
        Transform parentTransform = Instance.gameObject.transform;
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            child.gameObject.SetActive(active);
        }
    }

    public Vector3 CenterPosition(Vector3 position, float padding = 0f)
    {
        Vector3 offset = position - centerPoint;
        Vector3 projection = Vector3.Project(offset, copyVector);
        float unitsAway = Mathf.Round(Vector3.Dot(projection, copyVector) / Vector3.Dot(copyVector, copyVector));
        Vector3 centered = position - unitsAway * copyVector;
        
        if (padding == 0f)
            return centered;            
        if (Mathf.Abs((centered - centerPoint).magnitude - (position - centerPoint).magnitude) <= padding)
            return position;
        return centered;
    }

}