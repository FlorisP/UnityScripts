namespace Scripts
{
    using UnityEngine;

    class Objects : MonoBehaviour
    {
        public static void DestroyChildren(Transform target)
        {
            for (int i = target.childCount - 1; i >= 0; i--)
                DestroyImmediate(target.GetChild(i).gameObject);
        }

        public static void DestroyChildrenGO(GameObject go) 
            => DestroyChildren(go.transform);
        
        // Camera
        (Vector2 widthSpan, Vector2 heightSpan) FrameSpanFromCamera(float distance)
        {
            Camera cam = Camera.current;
            distance = Mathf.Abs(distance);

            float frameHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frameWidth = frameHeight * cam.aspect;

            Vector3 center = cam.transform.position + cam.transform.forward * distance;
            Vector3 right = cam.transform.right * (frameWidth / 2);
            Vector3 up = cam.transform.up * (frameHeight / 2);

            Vector2 widthSpan = new((center - right).x, (center + right).x);
            Vector2 heightSpan = new((center - up).y, (center + up).y);

            return (new Vector2(Mathf.Min(widthSpan.x, widthSpan.y), Mathf.Max(widthSpan.x, widthSpan.y)),
                    new Vector2(Mathf.Min(heightSpan.x, heightSpan.y), Mathf.Max(heightSpan.x, heightSpan.y)));
        }

        // AnimationCurve
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

