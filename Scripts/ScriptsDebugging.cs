using System.Collections;
using UnityEngine;

namespace Scripts
{
    class Debugging
    {
        public static IEnumerator DrawLineForSeconds(Vector2 start, Vector2 end, Color color, float duration)
        {
            float startTime = Time.time;
            while(Time.time < startTime + duration)
            {
                Debug.DrawLine(start, end, color);
                yield return null;
            }
        }
    }
}