using UnityEngine;

namespace Scripts
{
    class Functions
    {       
        // Repeating:
        public static float Sine(float t, float A=1, float f=1)
        {
            return A * Mathf.Sin(2 * Mathf.PI * f * t);
        }
        public static float TriangleWave(float t, float A=1, float f=1)
        {
            return 2f * Mathf.Abs(2f * (t * f - Mathf.Floor(t * f + 0.5f))) * A - A;
        }
        public static float CosineWave(float t, float A=1, float f=1)
        {
            return A * Mathf.Cos(2 * Mathf.PI * f * t);
        }
        public static float DoubleSine(float t, float A=1, float f=1)
        {
            return A * (Mathf.Sin(2 * Mathf.PI * f * t) + Mathf.Sin(2 * Mathf.PI * f * 2 * t) / 2);
        }
        public static float AbsSine(float t, float A=1, float f=1)
        {
            return A * Mathf.Abs(Mathf.Sin(2 * Mathf.PI * f * t));
        }
        public static float Quadratic(float t, float A=1, float f=1)
        {
            t = t * f % 1f;
            return A * 4 * t * (1 - t);
        }
        public static float Cubic(float t, float A=1, float f=1)
        {
            t = t * f % 1f;
            return A * (4 * t * t * t - 6 * t * t + 3 * t);
        }
        public static float Circle(float t, float A=1, float f=1)
        {
            t = t * f % 1f;
            return A * Mathf.Sqrt(1 - Mathf.Pow((2 * t - 1), 2));
        }
        public static float Ellipse(float t, float A=1, float f=1)
        {
            t = t * f % 1f;
            float a = A, b = A / 2;
            return b * Mathf.Sqrt(1 - Mathf.Pow((t - a) / a, 2));
        }
        //Discontinous
        public static float Square(float t, float A=1, float f=1)
        {
            return A * (Mathf.Floor(2 * t * f) % 2 == 0 ? 1 : -1);
        }
        public static float Sawtooth(float t, float A=1, float f=1)
        {
            return 2f * (t * f - Mathf.Floor(t * f + 0.5f)) * A;
        }

        // Non-Repeating
        public static float PerlinNoiseFunction(float t, float A=1, float f=1)
        {
            return A * Mathf.PerlinNoise(t * f, 0f);
        }
        public static float DampedOscillation(float t, float A=1, float f=1)
        {
            return A * Mathf.Exp(-t * f) * Mathf.Sin(2 * Mathf.PI * f * t);
        }
        public static float Pulse(float t, float A=1, float f=1)
        {
            return A * (Mathf.Floor(2 * t * f) % 2 == 0 ? 1 : -1) * Mathf.Exp(-t * f);
        }
        public static float Tangent(float t, float A=1, float f=1)
        {
            return A * Mathf.Tan(2 * Mathf.PI * f * t);
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

