using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HUJI.Gamelogic
{
    [Serializable]
    public class HUJIEnemySpawnService : HUJIBaseService
    {
        private HUJIEnemyManager _enemyManager;

        private int _uniqueId;
        public int GetUniqueId() => _uniqueId++;
        public readonly Dictionary<string, HUJIEnemyPrefabComponent> ActiveEnemies = new();
        public HUJIEnemyPrefabComponent GetEnemy(string name) => ActiveEnemies.GetValueOrDefault(name);

        private List<Coroutine> _spawnCoroutines = new();

        private Coroutine _delaySpawnCoroutine;

        public override void OnAwake(HUJIGameManagerComponent gameManager)
        {
            base.OnAwake(gameManager);

            _enemyManager = HUJIGamelogic.Instance.EnemyManager;
        }

        public override void OnStart()
        {
            base.OnStart();

            StarSpawn();
        }

        public void StopSpawn()
        {
            foreach (var spawnCoroutine in _spawnCoroutines)
            {
                if (spawnCoroutine != null)
                {
                    _gameManager.StopCoroutine(spawnCoroutine);
                }
            }

            _spawnCoroutines.Clear();

            _gameManager.StopWithNullCheckCoroutine(ref _delaySpawnCoroutine);
        }

        public void StarSpawn()
        {
            var levelData = HUJIGamelogic.Instance.LevelManager.Data;
            int levelNumber = levelData.CurrentLevelNumber;
            var spawnConfig = HUJIGamelogic.Instance.SpawnManager.GetSpawnConfig(levelNumber);

            if (spawnConfig == null)
            {
                HUJIDebug.Log($"spawnConfig == null");
                return;
            }

            int waveNumber = levelData.CurrentWave;
            var waveConfig = spawnConfig.GetWaveConfig(waveNumber);
            if (waveConfig == null)
            {
                HUJIDebug.Log($"waveConfig == null");
                return;
            }

            SpawnWave(levelNumber, waveNumber, waveConfig);
        }

        private void SpawnWave(int levelNumber, int waveNumber, HUJIWaveConfig waveConfig)
        {
            // HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnStartSpawnWave, (levelNumber, waveNumber));

            foreach (var wave in waveConfig.WaveEnemyConfigs)
            {
                Coroutine coroutine = null;
                coroutine = _gameManager.StartCoroutine(SpawnCoroutine(levelNumber, wave, () =>
                {
                    _spawnCoroutines.Remove(coroutine);

                    if (_spawnCoroutines.Count == 0 && ActiveEnemies.Count == 0)
                    {
                        OnFinishWave();
                    }
                }));

                _spawnCoroutines.Add(coroutine);
            }
        }

        private IEnumerator SpawnCoroutine(int level, HUJIWaveEnemyConfig wave, Action onComplete)
        {
            yield return new WaitForSeconds(wave.DelayBeforeSpawn);

            var waitForSecondSpawnDelay = new WaitForSeconds(wave.SpawnDelay);

            var enemyConfig = HUJIGamelogic.Instance.EnemyManager.GetEnemyConfig(wave.EnemyPoolName);
            for (int i = 0; i < wave.Amount; i++)
            {
                SpawnEnemy(level, enemyConfig);
                yield return waitForSecondSpawnDelay;
            }

            onComplete?.Invoke();
        }

        private void SpawnEnemy(int level, EnemyConfig enemyConfig)
        {
            var enemyInstantiated = (HUJIEnemyPrefabComponent)HUJICoreManager.Instance.PoolManager.GetPoolPrefab(enemyConfig.PrefabPoolName);
            if (enemyInstantiated)
            {
                string uniqueId = GetUniqueId().ToString();
                enemyInstantiated.gameObject.name = uniqueId;

                ActiveEnemies[uniqueId] = enemyInstantiated;

                enemyInstantiated.transform.position = GetSpawnPosition(_gameManager.Player.transform.position);
                enemyInstantiated.SetEnemy(level, enemyConfig);
            }
        }

        private Vector3 GetSpawnPosition(Vector3 playerPos)
        {
            const int pointCount = 12;
            float angleStep = 360f / pointCount;

            int randomIndex = UnityEngine.Random.Range(0, pointCount);
            float angle = randomIndex * angleStep;

            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));

            float radius = UnityEngine.Random.Range(_enemyManager.Config.MinMaxRadius.x, _enemyManager.Config.MinMaxRadius.y);
            Vector3 spawnPos = playerPos + dir * radius;

            return spawnPos;
        }

        public HUJIEnemyPrefabComponent FindClosestEnemy(Vector3 position)
        {
            HUJIEnemyPrefabComponent closestEnemy = null;
            float bestDistSq = 1000;

            foreach (var enemyKvp in ActiveEnemies)
            {
                var enemyPrefab = enemyKvp.Value;
                if (enemyPrefab == null) // || !enemyPrefab.EnemyHealth.GetIsAlive())
                {
                    continue;
                }

                float distSq = enemyPrefab.transform.position.SqrDistance(position);
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    closestEnemy = enemyPrefab;
                }
            }

            return closestEnemy;
        }

        public void RemoveEnemy(string enemyName)
        {
            ActiveEnemies.Remove(enemyName);

            if (_spawnCoroutines.Count == 0 && ActiveEnemies.Count == 0)
            {
                OnFinishWave();
            }
        }

        private void OnFinishWave()
        {
            bool isLoop = HUJIGamelogic.Instance.LevelManager.Data.IsLoop;
            if (isLoop)
            {
                StarSpawn();
                return;
            }

            HUJIGamelogic.Instance.LevelManager.AddLevelProgress();
            HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnFinishWave, null);
            _gameManager.StopAndStartWaitForSeconds(ref _delaySpawnCoroutine, 1f, () =>
            {
                StarSpawn();
            });
        }
    }
}