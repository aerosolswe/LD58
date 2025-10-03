using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCG
{
    public static class GCGUtil
    {
        private static Dictionary<float, WaitForSeconds> yielders = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds Yield(float time)
        {
            if (yielders.ContainsKey(time))
            {
                return yielders[time];
            } else
            {
                yielders.Add(time, new WaitForSeconds(time));
                return yielders[time];
            }
        }

        public static void LogError(string message)
        {
            Debug.LogError("<color=#00ccff>GCG:</color> " + message);
        }

        public static void Log(string message)
        {
            Debug.Log("<color=#00ccff>GCG:</color> " + message);
        }

        public static void Wait(MonoBehaviour behaviour, float duration, Action onComplete, float delay = 0.0f)
        {
            behaviour.StartCoroutine(LerpRoutine(0, 1, duration, null, onComplete, delay));
        }

        public static void Lerp(MonoBehaviour behaviour, float from, float to, float duration, Action<float> onUpdate, Action onComplete = null, float delay = 0.0f, bool unscaledTime = false)
        {
            behaviour.StartCoroutine(LerpRoutine(from, to, duration, onUpdate, onComplete, delay, unscaledTime));
        }

        public static IEnumerator LerpRoutine(float from, float to, float duration, Action<float> onUpdate, Action onComplete = null, float delay = 0.0f, bool unscaledTime = false)
        {
            float time = 0.0f;
            float t = 0.0f;
            
            yield return new WaitForSecondsRealtime(delay);

            while (t <= 1.0f)
            {
                time += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                t = time / duration;
                float val = Mathf.Lerp(from, to, t);
                onUpdate?.Invoke(val);

                yield return null;
            }

            onComplete?.Invoke();
        }

        public static void SetLayers(GameObject go, string layer)
        {
            foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }

        public static int GetRandomWeightedIndex(float[] weights)
        {
            if (weights == null || weights.Length == 0)
                return -1;

            float w;
            float t = 0;
            int i;
            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];

                if (float.IsPositiveInfinity(w))
                {
                    return i;
                } else if (w >= 0f && !float.IsNaN(w))
                {
                    t += weights[i];
                }
            }

            float r = UnityEngine.Random.value;
            float s = 0f;

            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0f)
                    continue;

                s += w / t;
                if (s >= r)
                    return i;
            }

            return -1;
        }
    }
}
