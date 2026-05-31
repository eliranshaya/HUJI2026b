using System;
using System.Collections;
using UnityEngine;

namespace HUJI
{
    public class HUJIMonoBehaviour : MonoBehaviour
    {
        protected HUJICoreManager Manager => HUJICoreManager.Instance;
#if UNITY_EDITOR
        public void SetDirty()
        {
            UnityEditor.EditorUtility.SetDirty(this);

            UnityEditor.SerializedObject serializedObj = new UnityEditor.SerializedObject(this);
            serializedObj.Update();
            serializedObj.ApplyModifiedProperties();
        }
#endif

        #region Events:

        protected void AddListener(HUJIEventName eventName, Action<object> eventCallback)
        {
            Manager?.EventManager.AddListener(eventName, eventCallback);
        }

        protected void RemoveListener(HUJIEventName eventName, Action<object> eventCallback)
        {
            Manager?.EventManager.RemoveListener(eventName, eventCallback);
        }

        protected void InvokeEvent(HUJIEventName eventName, object obj)
        {
            Manager?.EventManager.InvokeEvent(eventName, obj);
        }

        #endregion

        #region Debugging

        protected void Log(object msg)
        {
            HUJIDebug.Log($"[{name}] {msg}");
        }

        protected void LogWarning(object msg)
        {
            HUJIDebug.LogWarning($"[{name}] {msg}");
        }

        protected void LogException(object msg)
        {
            HUJIDebug.LogException($"[{name}] {msg}");
        }

        protected void LogError(object msg)
        {
            HUJIDebug.LogError($"[{name}] {msg}");
        }

        #endregion

        #region Coroutines:

        public void StopAndStartCoroutine(ref Coroutine coroutine, IEnumerator routine)
        {
            HUJIExtensions.StopAndStartCoroutine(this, ref coroutine, routine);
        }

        public void StopWithNullCheckCoroutine(ref Coroutine coroutine)
        {
            HUJIExtensions.StopWithNullCheckCoroutine(this, ref coroutine);
        }

        #endregion

        #region WaitFor:

        public Coroutine WaitForSeconds(float time, Action onComplete)
        {
            return StartCoroutine(WaitForSecondsCoroutine(time, onComplete));
        }

        public void StopAndStartWaitForSeconds(ref Coroutine coroutine, float time, Action onComplete)
        {
            StopAndStartCoroutine(ref coroutine, WaitForSecondsCoroutine(time, onComplete));
        }

        public void StopAndStartWaitForSecondsRealtime(ref Coroutine coroutine, float time, Action onComplete)
        {
            StopAndStartCoroutine(ref coroutine, WaitForSecondsRealtimeCoroutine(time, onComplete));
        }

        private IEnumerator WaitForSecondsCoroutine(float time, Action onComplete)
        {
            yield return new WaitForSeconds(time);
            onComplete?.Invoke();
        }

        public Coroutine WaitForSecondsRealtime(float time, Action onComplete)
        {
            return StartCoroutine(WaitForSecondsRealtimeCoroutine(time, onComplete));
        }

        private IEnumerator WaitForSecondsRealtimeCoroutine(float time, Action onComplete)
        {
            yield return new WaitForSecondsRealtime(time);
            onComplete?.Invoke();
        }

        public Coroutine WaitForFrame(Action onComplete)
        {
            return StartCoroutine(WaitForFramesCoroutine(onComplete, 1));
        }

        public void StopAndStartWaitForFrame(ref Coroutine coroutine, Action onComplete)
        {
            StopAndStartCoroutine(ref coroutine, WaitForFramesCoroutine(onComplete, 1));
        }

        public Coroutine WaitForAmountFrames(int amount, Action onComplete)
        {
            return StartCoroutine(WaitForFramesCoroutine(onComplete, amount));
        }

        public IEnumerator WaitForFramesCoroutine(Action onComplete, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return null;
            }

            onComplete?.Invoke();
        }

        #endregion
    }
}