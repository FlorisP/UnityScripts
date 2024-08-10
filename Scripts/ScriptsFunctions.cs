using UnityEngine;

namespace Scripts
{
    public class Functions
    {       
        // Repeating:        
        public static float One()
        {
            return 1;
        }

        public static float Sine(float t)
        {
            return Mathf.Sin(2 * Mathf.PI * t);
        }

        public static float TriangleWave(float t)
        {
            return 2f * Mathf.Abs(2f * (t - Mathf.Floor(t + 0.5f))) - 1f;
        }

        public static float CosineWave(float t)
        {
            return Mathf.Cos(2 * Mathf.PI * t);
        }

        public static float DoubleSine(float t)
        {
            return Mathf.Sin(2 * Mathf.PI * t) + Mathf.Sin(4 * Mathf.PI * t) / 2f;
        }

        public static float AbsSine(float t)
        {
            return Mathf.Abs(Mathf.Sin(2 * Mathf.PI * t));
        }

        public static float Quadratic(float t)
        {
            t = t % 1f;
            return 4f * t * (1f - t);
        }

        public static float Cubic(float t)
        {
            t %= 1f;
            return 4f * t * t * t - 6f * t * t + 3f * t;
        }

        public static float Circle(float t)
        {
            t = t % 1f;
            return Mathf.Sqrt(1f - Mathf.Pow(2f * t - 1f, 2f));
        }

        public static float Ellipse(float t)
        {
            t = t % 1f;
            return Mathf.Sqrt(1f - Mathf.Pow((t - 0.5f) / 0.5f, 2f));
        }

        public static float Pulse(float t)
        {
            return (Mathf.Floor(2 * t) % 2 == 0 ? 1f : -1f) * Mathf.Exp(-t);
        }

        public static float Tangent(float t)
        {
            return Mathf.Tan(2 * Mathf.PI * t);
        }


        // Discontinuous
        public static float Linear(float t)
        {
            return t;
        }

        public static float Square(float t)
        {
            return Mathf.Floor(2f * t) % 2 == 0 ? 1f : -1f;
        }

        public static float Sawtooth(float t)
        {
            return 2f * (t - Mathf.Floor(t + 0.5f));
        }


        // Non-Repeating
        public static float PerlinNoiseFunction(float t)
        {
            return Mathf.PerlinNoise(t, 0f);
        }

        public static float DampedOscillation(float t)
        {
            return Mathf.Exp(-t) * Mathf.Sin(2 * Mathf.PI * t);
        }


        public enum Enum
        {
            One,
            Sine,
            TriangleWave,
            CosineWave,
            DoubleSine,
            AbsSine,
            Quadratic,
            Cubic,
            Circle,
            Ellipse,
            Pulse,
            Tangent,

            Linear,
            Square,
            Sawtooth,

            PerlinNoiseFunction,
            DampedOscillation,
        }

        public static float Invoke(Enum function, float t)
        {
            switch (function)
            {
                case Enum.One:
                    return One();
                case Enum.Sine:
                    return Sine(t);
                case Enum.TriangleWave:
                    return TriangleWave(t);
                case Enum.CosineWave:
                    return CosineWave(t);
                case Enum.DoubleSine:
                    return DoubleSine(t);
                case Enum.AbsSine:
                    return AbsSine(t);
                case Enum.Quadratic:
                    return Quadratic(t);
                case Enum.Cubic:
                    return Cubic(t);
                case Enum.Circle:
                    return Circle(t);
                case Enum.Ellipse:
                    return Ellipse(t);

                case Enum.Linear:
                    return Linear(t);
                case Enum.Square:
                    return Square(t);
                case Enum.Sawtooth:
                    return Sawtooth(t);

                case Enum.PerlinNoiseFunction:
                    return PerlinNoiseFunction(t);
                case Enum.DampedOscillation:
                    return DampedOscillation(t);
                case Enum.Pulse:
                    return Pulse(t);
                case Enum.Tangent:
                    return Tangent(t);
                default:
                    Debug.LogWarning($"Function {function} not recognized");
                    return 0f;
            }
        }
    }
}
