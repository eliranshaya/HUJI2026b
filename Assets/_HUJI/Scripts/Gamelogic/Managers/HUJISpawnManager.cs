using System;
using System.Linq;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJISpawnManager
    {
        private HUJISpawnConfig _config;
        public HUJISpawnConfig Config => _config;

        public HUJISpawnManager(Action onComplete)
        {
            HUJICoreManager.Instance.ConfigManager.LoadConfigAsync("spawn_config", (HUJISpawnConfig config) =>
            {
                _config = config;

                onComplete?.Invoke();
            });
        }

        public SpawnConfig GetSpawnConfig(int level)
        {
            int loopStart = _config.LoopBetweenLevels;
            int maxLevel = _config.SpawnConfigs.Max(l => l.Level);

            if (level <= maxLevel)
            {
                return GetExactLevelConfig(level);
            }

            int window = maxLevel - loopStart + 1;
            int mapped = loopStart + (level - loopStart) % window;

            return GetExactLevelConfig(mapped);

            SpawnConfig GetExactLevelConfig(int level)
            {
                var levelConfig = _config.SpawnConfigs.FirstOrDefault(x => x.Level == level);
                if (levelConfig == null)
                {
                    HUJICoreManager.Instance.CrashManager.LogException($"SpawnConfig Not Found, level : {level}, Fix Firebase Config");
                }

                return levelConfig;
            }
        }
    }
    public class HUJISpawnConfig
    {
        public int LoopBetweenLevels;
        public SpawnConfig[] SpawnConfigs;
    }
    public class SpawnConfig
    {
        public int Level;
        public HUJIWaveConfig[] WaveConfigs;

        public HUJIWaveConfig GetWaveConfig(int wave)
        {
            int waveIndex = Mathf.Clamp(wave - 1, 0, WaveConfigs.Length);
            return WaveConfigs[waveIndex];
        }
    }
    public class HUJIWaveConfig
    {
        public HUJIWaveEnemyConfig[] WaveEnemyConfigs;
    }
    public class HUJIWaveEnemyConfig
    {
        public HUJIPoolName EnemyPoolName;
        public int EnemyLevel;
        public int Amount;
        public float SpawnDelay;
        public float DelayBeforeSpawn;
    }
}