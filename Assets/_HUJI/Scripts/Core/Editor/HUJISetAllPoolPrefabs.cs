#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HUJI
{
    public class HUJISetAllPoolPrefabs
    {
        private const string PREFABS_FOLDER_PATH = "Assets/_HUJI/Prefabs/Resources";
        private const string SCRIPTABLE_OBJECT_PATH = "Assets/_HUJI/Prefabs/Resources/Pools/AllPoolPrefabs.asset";

        [MenuItem("Tools/HUJI/SetAllPoolPrefabs")]
        public static void SetAllPoolPrefabs()
        {
            var poolNamesSO = AssetDatabase.LoadAssetAtPath<HUJIPoolNamesScriptableObject>(SCRIPTABLE_OBJECT_PATH);
            if (poolNamesSO == null)
            {
                Debug.LogError("Failed to load HUJIPoolNamesScriptableObject.");
                return;
            }

            string[] prefabPaths = Directory.GetFiles(PREFABS_FOLDER_PATH, "*.prefab", SearchOption.AllDirectories);
            List<HUJIPoolPrefab> allPoolPrefabs = new List<HUJIPoolPrefab>();

            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    HUJIPoolPrefab poolPrefab = prefab.GetComponent<HUJIPoolPrefab>();
                    if (poolPrefab != null)
                    {
                        SetPoolName(poolPrefab, prefab.name, allPoolPrefabs);
                    }
                }
            }

            poolNamesSO.UpdatePoolPrefabs(allPoolPrefabs.ToArray());
            EditorUtility.SetDirty(poolNamesSO);

            AssetDatabase.SaveAssets();
            HUJIDebug.Log("Set All Poolable Names and updated scriptable object completed!");
        }

        private static void SetPoolName(HUJIPoolPrefab poolPrefab, string prefabName, List<HUJIPoolPrefab> allPoolables)
        {
            try
            {
                var poolName = (HUJIPoolName)Enum.Parse(typeof(HUJIPoolName), prefabName);
                poolPrefab.SetPoolName(poolName);

                EditorUtility.SetDirty(poolPrefab);

                allPoolables.Add(poolPrefab);
            }
            catch (Exception ex)
            {
                HUJIDebug.LogError($"Failed to set PoolName for prefab '{prefabName}': {ex.Message}");
            }
        }
    }
}
#endif