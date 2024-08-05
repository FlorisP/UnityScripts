using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public static class Coroutines
    {
        private static readonly Dictionary<Action, Coroutine> runningCoroutines = new Dictionary<Action, Coroutine>();
        private static MonoBehaviour coroutineRunner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (coroutineRunner == null)
            {
                GameObject gameObject = new GameObject("CoroutineRunner");
                coroutineRunner = gameObject.AddComponent<CoroutineRunnerComponent>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
        }

        public static void ExecuteWithDelay(Action method, float delay, bool overrideExisting = true)
        {
            if (runningCoroutines.TryGetValue(method, out Coroutine existingCoroutine))
            {
                if (overrideExisting)
                {
                    coroutineRunner.StopCoroutine(existingCoroutine);
                    runningCoroutines[method] = coroutineRunner.StartCoroutine(DelayedExecution(method, delay));
                }
                else
                {
                    Debug.Log("A delayed execution for this method is already running.");
                }
            }
            else
            {
                runningCoroutines[method] = coroutineRunner.StartCoroutine(DelayedExecution(method, delay));
            }
        }

        private static IEnumerator DelayedExecution(Action method, float delay)
        {
            yield return new WaitForSeconds(delay);
            method.Invoke();
            runningCoroutines.Remove(method);
        }

        private class CoroutineRunnerComponent : MonoBehaviour { }
    }
}