using System;
using UnityEngine;

namespace HUJI
{
    public class HUJICoreManager
    {
        public static HUJICoreManager Instance { get; private set; }

        public HUJIEventManager EventManager;
        public HUJIPoolManager PoolManager;
        public HUJIConfigManager ConfigManager;

        //save
        //settings
        //sound
        //analytics
        //crashlytics

        public HUJICoreManager()
        {
            Instance = this;
        }

        public void LoadCoreManagers(Action onComplete)
        {
            GameObject temp = new GameObject("MonoManager");
            temp.AddComponent<HUJIMonoManagerObject>();

            EventManager = new();
            PoolManager = new();
            ConfigManager = new();//++

            HUJIDebug.Log($"Core Managers Initialized");
            onComplete?.Invoke();
        }
    }
}