namespace Scripts
{
    using UnityEngine;

    class Components : MonoBehaviour
    {
        // Remove all children from Parent
        public static void CleanParent(Transform tf)
        {
            int totalChildren = tf.childCount;
            for (int i = totalChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(tf.GetChild(i).gameObject);
            }
        }

        public static void CleanParent(RectTransform rect)
        {
            int totalChildren = rect.childCount;
            for (int i = totalChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(rect.GetChild(i).gameObject);
            }
        }

        public static void CleanParent(GameObject go)
        {
            CleanParent(go.transform);
        }   
        
    }
}

