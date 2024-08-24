using System;
using System.Collections.Generic;
using UnityEngine;

public class CopyWorld : MonoBehaviour
{
    [Serializable]
    public class MainObject
    {
        public GameObject meshObject;
        public Transform transform;
    }

    public Vector3 copyVector = new(5, 0, 0);
    public Vector3 centerPoint = Vector3.zero;
    public List<Transform> staticTransform;
    public List<MainObject> dynamicObjects;
    public Transform StaticParent;
    public Transform DynamicParent;

    List<GameObject> dynamicCopies = new();

    void Start()
    {
        // Static Prefabs
        foreach (Transform tf in staticTransform)
        {
            GameObject copy = Instantiate(tf.gameObject, tf.position + copyVector, tf.rotation);
            copy.transform.SetParent(StaticParent);
        }

        // Dynamic Objects
        foreach (MainObject main in dynamicObjects)
        {
            GameObject copy = Instantiate(main.meshObject, main.transform.position, main.transform.rotation);
            copy.transform.SetParent(DynamicParent);
            copy.transform.localScale = main.transform.transform.localScale;
            dynamicCopies.Add(copy);
        }
    }

    void Update()
    {
        for (int i = 0; i < dynamicObjects.Count; i++)
        {
            MainObject main = dynamicObjects[i];
            GameObject copy = dynamicCopies[i];

            Vector3 offset = main.transform.position - centerPoint;
            Vector3 projection = Vector3.Project(offset, copyVector);
            float unitsAway = Vector3.Dot(projection, copyVector) /  Vector3.Dot(copyVector, copyVector);
            Vector3 closestPosition = main.transform.position - Mathf.Round(unitsAway) * copyVector;

            float sign = Mathf.Sign(Vector3.Dot(centerPoint - closestPosition, copyVector));
            Vector3 closePosition = closestPosition + sign * copyVector;

            main.transform.position = closestPosition;
            copy.transform.position = closePosition;
            copy.transform.rotation = main.transform.rotation;
            copy.transform.localScale = main.transform.localScale;
        }
    }
}