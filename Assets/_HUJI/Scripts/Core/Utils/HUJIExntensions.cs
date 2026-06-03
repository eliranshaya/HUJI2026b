using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HUJI
{
    public static class HUJIExtensions
    {
        public static void SaveData(this IHUJISaveData saveable)
        {
            HUJICoreManager.Instance.SaveManager.Save(saveable);
        }

        public static void StopAndStartCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine, IEnumerator enumerator)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
            }

            coroutine = monoBehaviour.StartCoroutine(enumerator);
        }

        public static void StopWithNullCheckCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistance(this Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static bool GetRandomBool()
        {
            return UnityEngine.Random.Range(0, 2) == 0;
        }

        public static float GetRandomNumberBetween(this Vector2 range)
        {
            return UnityEngine.Random.Range(range.x, range.y);
        }

        public static T GetRandomFromArray<T>(this T[] array)
        {
            if (array.Length == 0) return default;

            int random = Random.Range(0, array.Length);
            return array[random];
        }

        public static GameObject FindInChildrenByName(this Transform parent, string name)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        #region ChangeValueOverTimeEase

        public enum HUJIUpdateMode
        {
            Update,
            EndOfFrame,
            UnscaledUpdate,
            FixedUpdate,
        }

        public static IEnumerator ChangeValueOverTimeEase<T>(T startValue, T endValue, float duration, Action<T> applyValue, Func<T, T, float, T> lerpFunc, AnimationCurve animationCurve = null, HUJIUpdateMode updateMode = HUJIUpdateMode.Update, Action onComplete = null)
        {
            if (animationCurve == null)
            {
                animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
            }

            Func<float> deltaTime;
            YieldInstruction yieldInstruction;

            switch (updateMode)
            {
                case HUJIUpdateMode.EndOfFrame:
                    deltaTime = () => Time.deltaTime;
                    yieldInstruction = new WaitForEndOfFrame();
                    break;
                case HUJIUpdateMode.FixedUpdate:
                    deltaTime = () => Time.fixedDeltaTime;
                    yieldInstruction = new WaitForFixedUpdate();
                    break;
                case HUJIUpdateMode.UnscaledUpdate:
                    deltaTime = () => Time.unscaledDeltaTime;
                    yieldInstruction = null;
                    break;

                default:
                    deltaTime = () => Time.deltaTime;
                    yieldInstruction = null;
                    break;
            }

            applyValue(startValue);
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float easedT = animationCurve.Evaluate(t);

                applyValue(lerpFunc(startValue, endValue, easedT));

                elapsedTime += deltaTime();
                yield return yieldInstruction;
            }

            applyValue(lerpFunc(startValue, endValue, animationCurve.Evaluate(1f)));
            onComplete?.Invoke();
        }

        #region Variants:

        public static IEnumerator ChangeValueOverTimeEase(float startValue, float endValue, float duration, Action<float> applyValue, AnimationCurve animationCurve = null, HUJIUpdateMode updateMode = HUJIUpdateMode.Update, Action onComplete = null)
        {
            return ChangeValueOverTimeEase(startValue, endValue, duration, applyValue, Mathf.LerpUnclamped, animationCurve, updateMode, onComplete);
        }

        public static IEnumerator ChangeValueOverTimeEase(Vector2 startValue, Vector2 endValue, float duration, Action<Vector2> applyValue, AnimationCurve animationCurve = null, HUJIUpdateMode updateMode = HUJIUpdateMode.Update, Action onComplete = null)
        {
            return ChangeValueOverTimeEase(startValue, endValue, duration, applyValue, Vector2.LerpUnclamped, animationCurve, updateMode, onComplete);
        }

        public static IEnumerator ChangeValueOverTimeEase(Vector3 startValue, Vector3 endValue, float duration, Action<Vector3> applyValue, AnimationCurve animationCurve = null, HUJIUpdateMode updateMode = HUJIUpdateMode.Update, Action onComplete = null)
        {
            return ChangeValueOverTimeEase(startValue, endValue, duration, applyValue, Vector3.LerpUnclamped, animationCurve, updateMode, onComplete);
        }

        public static IEnumerator ChangeValueOverTimeEase(Color startValue, Color endValue, float duration, Action<Color> applyValue, AnimationCurve animationCurve = null, HUJIUpdateMode updateMode = HUJIUpdateMode.Update, Action onComplete = null)
        {
            return ChangeValueOverTimeEase(startValue, endValue, duration, applyValue, Color.LerpUnclamped, animationCurve, updateMode, onComplete);
        }

        #endregion

        #endregion
    }
}