using System;
using System.Collections;
using Firebase.Extensions;
using UnityEngine;

namespace HUJI
{
    public class HUJICoreManager
    {
        public static HUJICoreManager Instance { get; private set; }

        public HUJIEventManager EventManager;
        public HUJIPoolManager PoolManager;
        public HUJIConfigManager ConfigManager;
        public HUJICrashManager CrashManager;
        public HUJISaveManager SaveManager;
        public HUJIAnalyticsManager AnalyticsManager;

        //save
        //settings
        //sound
        //analytics
        //crashlytics

        public HUJICoreManager()
        {
            Instance = this;
        }

        public void LoadManager(Action onComplete)
        {
            InitFirebase(delegate
            {
                LoadCoreManagers(onComplete);
            });
        }

        private void InitFirebase(Action onComplete)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    var app = Firebase.FirebaseApp.DefaultInstance;
                    HUJIDebug.Log($"Firebase was initialized");
                    onComplete?.Invoke();
                }
                else
                {
                    HUJIDebug.Log($"Could not resolve all Firebase dependencies : {dependencyStatus}");
                }
            });
        }

        public void LoadCoreManagers(Action onComplete)
        {
            CrashManager = new();

            GameObject temp = new GameObject("MonoManager");
            temp.AddComponent<HUJIMonoManagerObject>();

            AnalyticsManager = new();
            SaveManager = new();
            EventManager = new();
            PoolManager = new();

            HUJIMonoManagerObject.Instance.StartCoroutine(LoadingAsyncManagers(() =>
            {
                HUJIDebug.Log($"Core Managers Initialized");
                onComplete?.Invoke();
            }));
        }

        private IEnumerator LoadingAsyncManagers(Action onComplete)
        {
            int numManagers = 1;
            int initializedManagers = 0;

            ConfigManager = new(() => initializedManagers++);

            yield return new WaitUntil(() => initializedManagers >= numManagers);

            onComplete?.Invoke();
        }
    }
}