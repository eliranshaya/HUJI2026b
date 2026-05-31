using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HUJI
{
    public class HUJIPoolManager
    {
        private const int DEFAULT_INITIAL_AMOUNT = 2;
        private const int DEFAULT_MAX_POOLABLES = 50;

        private readonly Dictionary<HUJIPoolName, HUJIPoolPrefab> _allPrefabs = new();
        private readonly Dictionary<HUJIPoolName, HUJIPool> _pools = new();
        private readonly Transform _rootPools;

        public HUJIPoolManager()
        {
            _rootPools = new GameObject("Pools").transform;
            Object.DontDestroyOnLoad(_rootPools);

            var poolNamesSO = Resources.Load<HUJIPoolNamesScriptableObject>("Pools/AllPoolPrefabs");
            if (poolNamesSO == null)
            {
                HUJIDebug.LogError("AllPoolPrefabs ScriptableObject not found in Resources/ScriptableObjects.");
                return;
            }

            _allPrefabs = poolNamesSO.GetAllPoolPrefabsDictionary();
        }

        // ─────────────────────────────────────────────
        // INIT
        // ─────────────────────────────────────────────

        public void InitPool(HUJIPoolName poolName, int amount, int maxPoolPrefabs = DEFAULT_MAX_POOLABLES)
        {
            if (!TryGetPrefab(poolName, out var prefab))
            {
                HUJIDebug.LogWarning($"Pool for {poolName} not found.");
                return;
            }

            InitPool(prefab, amount, maxPoolPrefabs);
        }

        private void InitPool(HUJIPoolPrefab prefab, int amount, int maxPoolPrefabs = DEFAULT_MAX_POOLABLES)
        {
            if (prefab == null)
            {
                HUJIDebug.LogError("InitPool: prefab is null.");
                return;
            }

            if (_pools.ContainsKey(prefab.PoolName))
            {
                HUJIDebug.LogWarning($"Pool for {prefab.PoolName} already initialized.");
                return;
            }

            var holder = new GameObject($"{prefab.name} Holder").transform;
            holder.SetParent(_rootPools);
            holder.localPosition = Vector3.zero;

            var pool = new HUJIPool { Prefab = prefab, Holder = holder, MaxPoolPrefabs = maxPoolPrefabs };

            _pools.Add(prefab.PoolName, pool);
            CreatePoolPrefabs(pool, amount);
        }

        // ─────────────────────────────────────────────
        // GET / RETURN
        // ─────────────────────────────────────────────

        public HUJIPoolPrefab GetPoolPrefab(HUJIPoolName poolName)
        {
            if (poolName == HUJIPoolName.None)
            {
                HUJIDebug.LogWarning("GetPoolPrefab called with HUJIPoolName.None");
                return null;
            }

            if (!_pools.TryGetValue(poolName, out var pool))
            {
                InitPool(poolName, DEFAULT_INITIAL_AMOUNT);
                if (!_pools.TryGetValue(poolName, out pool))
                {
                    return null;
                }
            }

            if (pool.AvailablePoolPrefabs.Count == 0)
            {
                AddPoolPrefabs(pool, 1);
            }

            if (pool.AvailablePoolPrefabs.Count == 0)
            {
                return null;
            }

            var poolPrefab = pool.AvailablePoolPrefabs.First();
            pool.AvailablePoolPrefabs.Remove(poolPrefab);
            pool.ActivePoolPrefabs.Add(poolPrefab);
            poolPrefab.OnTakenFromPool();
            return poolPrefab;
        }

        public void ReturnPoolPrefab(HUJIPoolPrefab poolPrefab, bool returnToHolderParent = false)
        {
            if (poolPrefab == null)
                return;

            if (!_pools.TryGetValue(poolPrefab.PoolName, out var pool))
                return;

            if (!pool.ActivePoolPrefabs.Remove(poolPrefab))
                return;

            pool.AvailablePoolPrefabs.Add(poolPrefab);
            poolPrefab.OnReturnedToPool();

            if (returnToHolderParent && pool.Holder != null)
                poolPrefab.transform.SetParent(pool.Holder);
        }

        public void ReturnAllPoolPrefabs(HUJIPoolName poolName, bool returnToHolderParent = false)
        {
            if (_pools.TryGetValue(poolName, out var pool))
                ReturnAllInPool(pool, returnToHolderParent);
        }

        public void ReturnAllPoolPrefabs(bool returnToHolderParent = false)
        {
            foreach (var pool in _pools.Values)
                ReturnAllInPool(pool, returnToHolderParent);
        }

        // ─────────────────────────────────────────────
        // DESTROY
        // ─────────────────────────────────────────────

        public void DestroyPool(HUJIPoolName poolName)
        {
            if (!_pools.TryGetValue(poolName, out var pool))
                return;

            DestroyPoolInternal(pool);
            _pools.Remove(poolName);
        }

        public void DestroyAllPools()
        {
            foreach (var pool in _pools.Values)
                DestroyPoolInternal(pool);

            _pools.Clear();
        }

        // ─────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────

        private bool TryGetPrefab(HUJIPoolName poolName, out HUJIPoolPrefab prefab)
        {
            if (poolName == HUJIPoolName.None)
            {
                HUJIDebug.LogError("PoolName is None.");
                prefab = null;
                return false;
            }

            if (!_allPrefabs.TryGetValue(poolName, out prefab))
            {
                HUJIDebug.LogError($"Prefab for {poolName} not found.");
                return false;
            }

            return true;
        }

        private void AddPoolPrefabs(HUJIPool pool, int amount)
        {
            int totalAfterAdd = pool.AllPoolPrefabs.Count + amount;
            if (totalAfterAdd > pool.MaxPoolPrefabs)
            {
                HUJIDebug.LogWarning($"Cannot add {amount} poolPrefabs to {pool.Prefab.PoolName}. " + $"Would exceed max of {pool.MaxPoolPrefabs}.");
                return;
            }

            CreatePoolPrefabs(pool, amount);
        }

        private void CreatePoolPrefabs(HUJIPool pool, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var instance = Object.Instantiate(pool.Prefab, pool.Holder);
                instance.gameObject.SetActive(false);
                instance.name = $"{pool.Prefab.PoolName} {pool.AllPoolPrefabs.Count}";

                pool.AllPoolPrefabs.Add(instance);
                pool.AvailablePoolPrefabs.Add(instance);
            }
        }

        private void ReturnAllInPool(HUJIPool pool, bool returnToHolderParent)
        {
            foreach (var poolPrefab in pool.ActivePoolPrefabs)
            {
                pool.AvailablePoolPrefabs.Add(poolPrefab);
                poolPrefab.OnReturnedToPool();

                if (returnToHolderParent && pool.Holder != null)
                {
                    poolPrefab.transform.SetParent(pool.Holder);
                }
            }

            pool.ActivePoolPrefabs.Clear();
        }

        private void DestroyPoolInternal(HUJIPool pool)
        {
            foreach (var poolPrefab in pool.AllPoolPrefabs)
            {
                if (poolPrefab == null || poolPrefab.gameObject == null)
                    continue;

                poolPrefab.PreDestroy();
                Object.Destroy(poolPrefab.gameObject);
            }

            pool.AllPoolPrefabs.Clear();
            pool.AvailablePoolPrefabs.Clear();
            pool.ActivePoolPrefabs.Clear();

            if (pool.Holder != null)
                Object.Destroy(pool.Holder.gameObject);
        }

        // ─────────────────────────────────────────────
        // POOL DATA
        // ─────────────────────────────────────────────

        private class HUJIPool
        {
            public HUJIPoolPrefab Prefab;
            public Transform Holder;
            public int MaxPoolPrefabs = DEFAULT_MAX_POOLABLES;

            public readonly HashSet<HUJIPoolPrefab> AllPoolPrefabs = new();
            public readonly HashSet<HUJIPoolPrefab> AvailablePoolPrefabs = new();
            public readonly HashSet<HUJIPoolPrefab> ActivePoolPrefabs = new();
        }
    }
    public enum HUJIPoolName
    {
        None,
        SoundPrefab,
        ExperiencePrefab,
        DamageTextPrefab,
        Enemy1Prefab,
    }
}