using System;
using System.Collections;
using UnityEngine;

namespace Scripts
{
    public static class Coroutines
    {
        // Start with StartCoroutine(Coroutines.ExecuteWithDelay(() => { /* Your code here */ }, delay));
        public static IEnumerator ExecuteWithDelay(Action method, float delay)
        {
            yield return new WaitForSeconds(delay);
            method.Invoke();
        }

    }
}