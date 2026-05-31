using System.Collections.Generic;
using UnityEngine;

namespace HUJI
{
    [CreateAssetMenu(fileName = "AllPoolPrefabs", menuName = "HUJI/AllPoolPrefabs")]
    public class HUJIPoolNamesScriptableObject : ScriptableObject
    {
        [Header("References")]
        [SerializeField] private HUJIPoolPrefab[] _allPoolPrefabs;

        public Dictionary<HUJIPoolName, HUJIPoolPrefab> GetAllPoolPrefabsDictionary()
        {
            Dictionary<HUJIPoolName, HUJIPoolPrefab> allPoolPrefabs = new();
            foreach (var poolPrefab in _allPoolPrefabs)
            {
                if (poolPrefab == null)
                {
                    HUJIDebug.LogWarning($"PoolPrefab {poolPrefab.name} is null");
                    continue;
                }

                if (!allPoolPrefabs.TryAdd(poolPrefab.PoolName, poolPrefab))
                {
                    HUJIDebug.LogWarning($"Duplicate pool name found: {poolPrefab.PoolName}");
                }
            }

            return allPoolPrefabs;
        }

        public void UpdatePoolPrefabs(HUJIPoolPrefab[] poolPrefabs)
        {
            _allPoolPrefabs = poolPrefabs;
        }
    }
}