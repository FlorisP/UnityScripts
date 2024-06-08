namespace Scripts
{
    using System;
    using UnityEngine;

    class Objects : MonoBehaviour
    {
        /// <summary> Removes all children from GameObject </summary> 
        public static GameObject CleanObject(GameObject obj, Transform parent = null)
        {
            string name = obj.name;
            DestroyImmediate(obj);
            GameObject CleanObject = new GameObject(name);
            if (parent) CleanObject.transform.SetParent(parent);
            return CleanObject;
        }

        /// <summary> Removes all children from GameObject </summary> 
        public static void CleanRect(RectTransform rect)
        {
            int totalChildren = rect.childCount;
            for (int i = totalChildren - 1; i >= 0; i--)
                DestroyImmediate(rect.GetChild(i).gameObject);
        }

        /// <summary> Clones Object by value </summary> 
        public static T Clone<T>(T source) where T : new()
        {
            T dest = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(dest, prop.GetValue(source));
                }
            }

            foreach (var field in typeof(T).GetFields())
            {
                field.SetValue(dest, field.GetValue(source));
            }

            return dest;
        }
    }
}

