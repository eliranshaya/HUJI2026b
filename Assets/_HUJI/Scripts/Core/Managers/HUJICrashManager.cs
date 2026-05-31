using System;
using Firebase.Crashlytics;

namespace HUJI
{
    public class HUJICrashManager
    {
        public HUJICrashManager()
        {
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
        }

        public void LogException(string message)
        {
            Crashlytics.LogException(new Exception("HUJI LogException:" + message));
            HUJIDebug.LogException(message);
        }

        public void Log(string message)
        {
            Crashlytics.Log("HUJI Log: " + message);
            HUJIDebug.Log(message);
        }
    }
}