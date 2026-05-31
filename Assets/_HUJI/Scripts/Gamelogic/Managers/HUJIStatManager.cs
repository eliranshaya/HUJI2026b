using System;

namespace HUJI.Gamelogic
{
    public class HUJIStatManager
    {
        private HUJIStatConfig _config;

        public HUJIStatManager(Action onComplete)
        {
            HUJICoreManager.Instance.ConfigManager.LoadConfigAsync("stat_config", (HUJIStatConfig config) =>
            {
                _config = config;

                foreach (var baseStat in _config.BaseStats)
                {
                    HUJIDebug.Log($"baseStat = {baseStat.StatType},  value = {baseStat.Value}");
                }

                onComplete?.Invoke();
            });
        }

        public HUJIBaseStat[] GetBaseStats() => _config.BaseStats;
    }
    public class HUJIStatConfig
    {
        public HUJIBaseStat[] BaseStats;
    }
    [Serializable]
    public struct HUJIBaseStat
    {
        public HUJIStatType StatType;
        public float Value;
    }
    public enum HUJIStatType
    {
        None,
        AttackDamage,
        Health,
        MovementSpeed
    }
}