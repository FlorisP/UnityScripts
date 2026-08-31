using UnityEngine;

namespace Scripts
{
    public enum Side { None, Right, Left, Up, Down }

    public static class Constants
    {
        public static float e = 2.71828175f;

        public static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    }
}
