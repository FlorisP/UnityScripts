using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorldSpawn : MonoBehaviour
{
    public GameObject prefab;
    public float prefabWidth = 1.0f;
    public float prefabHeight = 1.0f;
    public Vector2 gridSize = new Vector2(5, 5);
    public Vector3 origin = Vector3.zero;
    public Quaternion localRotation = Quaternion.identity;
    public Quaternion globalRotation = Quaternion.identity;
    public Transform parentTransform;

    [Button] public void SpawnGrid()
    {
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
            DestroyImmediate(parentTransform.GetChild(i).gameObject);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 localPosition = new Vector3(x * prefabWidth, 0, y * prefabHeight);
                Vector3 spawnPosition = globalRotation * localPosition + origin;
                GameObject spawnedPrefab = Instantiate(prefab, spawnPosition, localRotation, parentTransform);
            }
        }
    }
}
