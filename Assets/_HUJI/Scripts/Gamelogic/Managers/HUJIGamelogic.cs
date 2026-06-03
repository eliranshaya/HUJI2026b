using System;
using System.Collections;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIGamelogic
    {
        public static HUJIGamelogic Instance;

        public HUJIStaticManagerScriptableObject StaticManager;

        // public HUJIInventoryManager InventoryManager;
        public HUJIStatManager StatManager;
        public HUJIAbilityManager AbilityManager;
        public HUJIEnemyManager EnemyManager;
        public HUJISpawnManager SpawnManager;
        public HUJILevelManager LevelManager;

        public HUJIGamelogic()
        {
            Instance = this;

            StaticManager = Resources.Load<HUJIStaticManagerScriptableObject>("ScriptableObjects/StaticManager");
            // StaticManager.Init();
        }

        public void LoadManager(Action onComplete)
        {
            HUJIMonoManagerObject.Instance.StartCoroutine(InitializeManagersAsync(() =>
            {
                onComplete?.Invoke();
            }));
        }

        private IEnumerator InitializeManagersAsync(Action onComplete)
        {
            int numManagers = 5;
            int initializedManagers = 0;

            // InventoryManager = new(() => initializedManagers++);
            StatManager = new(() => initializedManagers++);
            AbilityManager = new(() => initializedManagers++);
            EnemyManager = new(() => initializedManagers++);
            SpawnManager = new(() => initializedManagers++);
            LevelManager = new(() => initializedManagers++);

            yield return new WaitUntil(() => initializedManagers >= numManagers);

            HUJIDebug.Log($"GameLogic Managers Initialized");
            onComplete?.Invoke();
        }
    }
}