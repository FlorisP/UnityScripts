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


        
        public static AnimationCurve InvertAnimationCurve(AnimationCurve curve, int resolution = 30)
        {
            AnimationCurve newCurve = new AnimationCurve();
            float startTime = curve.keys[0].time;
            float endTime = curve.keys[curve.length - 1].time;

            for (int i = 0; i <= resolution; i++)
            {
                float t = Mathf.Lerp(startTime, endTime, i / (float)resolution);
                float value = curve.Evaluate(t);
                newCurve.AddKey(value, t);
            }

            // Smooth tangents
            for (int i = 0; i < newCurve.keys.Length; i++)
                newCurve.SmoothTangents(i, 0);

            return newCurve;
        }
        
    }
}

