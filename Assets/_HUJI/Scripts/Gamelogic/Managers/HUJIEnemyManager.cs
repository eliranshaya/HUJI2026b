using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIEnemyManager
    {
        private HUJIEnemyConfig _config;
        public HUJIEnemyConfig Config => _config;

        public HUJIEnemyManager(Action onComplete)
        {
            HUJICoreManager.Instance.ConfigManager.LoadConfigAsync("enemy_config", (HUJIEnemyConfig config) =>
            {
                _config = config;

                onComplete?.Invoke();
            });
        }

        public EnemyConfig GetEnemyConfig(HUJIPoolName prefabPoolName)
        {
            EnemyConfig enemyConfig = _config.EnemyConfigs.FirstOrDefault(x => x.PrefabPoolName == prefabPoolName);
            if (enemyConfig == null)
            {
                HUJICoreManager.Instance.CrashManager.LogException($"Enemy Config Not Found, prefabPoolName : {prefabPoolName}, Fix Firebase Config");
            }

            return enemyConfig;
        }
    }
    public class HUJIEnemyConfig
    {
        public Vector2 MinMaxRadius;
        public EnemyConfig[] EnemyConfigs;
    }
    public class EnemyConfig
    {
        public HUJIPoolName PrefabPoolName;
        public HUJIEnemyType EnemyType;

        public HUJIEnemyStat[] Stats;
        public HUJIEnemyAbility[] Abilities;
    }
    public class HUJIEnemyStat
    {
        public HUJIBaseStat BaseStat;
        public float IncreasePerLevel;//not good

        public float GetStat(int level)
        {
            return BaseStat.Value + level * IncreasePerLevel;
        }
    }
    public class HUJIEnemyAbility
    {
        public HUJIPoolName VisualPoolName;
        public HUJIBodyType BodyType;

        public string AnimationTriggerString;

        public HUJIBaseStat[] BaseStats;
    }
}