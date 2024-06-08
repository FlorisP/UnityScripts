using System;
using UnityEngine;

namespace Scripts
{
    class Debugging
    {
        public static void Print<T>(T variable, string comment = "")
        {
            Debug.Log($"{comment} {variable}");
        }

        internal static void DrawLine(UnityEngine.Vector2 center, UnityEngine.Vector2 point, UnityEngine.Color red)
        {
            throw new NotImplementedException();
        }
    }
}

