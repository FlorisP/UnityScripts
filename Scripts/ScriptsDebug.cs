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

    }
}

