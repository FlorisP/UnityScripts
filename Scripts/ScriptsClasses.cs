using System;
using UnityEngine;

namespace Scripts
{
    class Classes
    {
        // public static AnimationCurve InvertAnimationCurve(AnimationCurve curve)
        // {
        //     AnimationCurve newCurve = new AnimationCurve();

        //     foreach (Keyframe key in curve.keys)
        //         newCurve.AddKey(key.value, key.time);

        //     // Tangent modes
        //     for (int i = 0; i < newCurve.keys.Length; i++)
        //         newCurve.SmoothTangents(i, 0);

        //     return newCurve;
        // }
        
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

            // Data Classes
        [Serializable]
        public class ObjPos
        {
            public GameObject Object;
            public Vector3 Position;

            public ObjPos(GameObject obj, Vector3 pos)
            {
                Object = obj;
                Position = pos;
            }
        }
    } 
}

