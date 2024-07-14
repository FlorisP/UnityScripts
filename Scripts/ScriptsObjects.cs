namespace Scripts
{
    using UnityEngine;

    class Objects : MonoBehaviour
    {
        /// <summary> Removes all children from Transform </summary> 
        public static void CleanObject(Transform tf, Transform parent = null)
        {
            int totalChildren = tf.childCount;
            for (int i = totalChildren - 1; i >= 0; i--)
                DestroyImmediate(tf.GetChild(i).gameObject);
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

