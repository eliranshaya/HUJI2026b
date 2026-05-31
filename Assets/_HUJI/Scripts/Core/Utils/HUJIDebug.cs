using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace HUJI
{
    public class HUJIDebug
    {
        [Conditional("LOGS_ENABLE")]
        public static void Log(object message)
        {
            Debug.Log("HUJI Log: " + message);
        }
        [Conditional("LOGS_ENABLE")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning("HUJI LogWarning: " + message);
        }
        [Conditional("LOGS_ENABLE")]
        public static void LogException(object message)
        {
            Debug.LogException(new Exception("HUJI LogException: " + message.ToString()));
        }

        [Conditional("LOGS_ENABLE")]
        public static void LogError(object message)
        {
            Debug.LogError("HUJI LogError: " + message);
        }
    }
}