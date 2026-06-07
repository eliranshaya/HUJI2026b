using System;
using System.Linq;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIAbilityManager
    {
        private HUJIAbilityConfig _config;
        public HUJIAbilityConfig Config => _config;

        public HUJIAbilityManager(Action onComplete)
        {
            HUJICoreManager.Instance.ConfigManager.LoadConfigAsync("ability_config", (HUJIAbilityConfig config) =>
            {
                _config = config;

                SetAbilitySprites();

                onComplete?.Invoke();
            });
        }

        private void SetAbilitySprites()
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Abilities");
            var match = sprites.FirstOrDefault(x => x.name == _config.BasicAttackConfig.AbilitySpriteName);
            if (match)
            {
                _config.BasicAttackConfig.AbilitySprite = match;
                HUJIDebug.Log($"Sprite name AbilitySpriteName = '{match}'");
            }
            else
            {
                HUJIDebug.LogWarning($"Sprite name AbilitySpriteName = '{_config.BasicAttackConfig.AbilitySpriteName}' does not match.");
            }

            foreach (var abilityConfig in _config.AbilityConfigs)
            {
                match = sprites.FirstOrDefault(x => x.name == abilityConfig.AbilitySpriteName);
                if (match)
                {
                    abilityConfig.AbilitySprite = match;
                }
                else
                {
                    HUJIDebug.LogWarning($"Sprite name AbilitySpriteName = '{abilityConfig.AbilitySpriteName}' does not match.");
                }
            }
        }

        #region Config

        public AbilityConfig GetAbilityConfig(int uniqueId)
        {
            var abilityConfig = _config.AbilityConfigs.FirstOrDefault(x => x.AbilityUniqueId == uniqueId);
            if (abilityConfig == null)
            {
                HUJICoreManager.Instance.CrashManager.LogException($"abilityConfig Not Found, UniqueId : {uniqueId}, Fix Firebase Config");
            }

            return abilityConfig;
        }

        #endregion
    }

    public class HUJIAbilityConfig
    {
        public AbilityConfig BasicAttackConfig;
        public AbilityConfig[] AbilityConfigs;
    }
    public class AbilityConfig
    {
        public int AbilityUniqueId;
        public string AbilityName;

        public string AbilitySpriteName;
        public Sprite AbilitySprite;

        public HUJIBodyType BodyType;
        public HUJIPoolName VisualPoolName;
        public string AnimationTriggerString;
        public float StoppingDistance;

        public HUJIBaseStat[] BaseStats;

        public float GetAbilityBaseStat(HUJIStatType statType)
        {
            foreach (var stat in BaseStats)
            {
                if (stat.StatType == statType)
                {
                    return stat.Value;
                }
            }

            return 0;
        }
    }
}