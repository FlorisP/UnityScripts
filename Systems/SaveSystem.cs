using UnityEngine;
using Sirenix.OdinInspector;

public class SaveSystem : MonoBehaviour
{
    static SaveSystem _instance;
    public static SaveSystem Instance => _instance = _instance != null ? _instance : FindFirstObjectByType<SaveSystem>();

    public static T Load<T>(string key, T fallback) => ES3.Load(key, fallback);
    public static void Save<T>(string key, T value) => ES3.Save(key, value);

    [Button]
    public void RemoveES3File() => ES3.DeleteFile();
}

