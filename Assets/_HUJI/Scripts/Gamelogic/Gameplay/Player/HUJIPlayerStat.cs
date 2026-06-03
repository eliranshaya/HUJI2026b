using System;
using System.Collections.Generic;

namespace HUJI.Gamelogic
{
    [Serializable]
    public class HUJIPlayerStat : HUJIBasePlayer
    {
        private Dictionary<HUJIStatType, float> _stats;
        private Dictionary<HUJIStatType, Action> _onUpdateStats;

        public override void OnInitialize(HUJIPlayerPrefabComponent player)
        {
            base.OnInitialize(player);

            _stats = new();
            _onUpdateStats = new();

            var baseStats = Gamelogic.StatManager.GetBaseStats();
            foreach (var playerBaseStat in baseStats)
            {
                _stats.Add(playerBaseStat.StatType, playerBaseStat.Value);
            }

            //here will be more calculation from data too
        }

        public float GetStat(HUJIStatType statType) => _stats.GetValueOrDefault(statType, 0);

        public void SetStat(HUJIStatType statType, float value)
        {
            if (!_stats.TryAdd(statType, value))
            {
                _stats[statType] = value;

                if (_onUpdateStats.TryGetValue(statType, out Action onUpdateStat))
                {
                    onUpdateStat?.Invoke();
                }
            }
        }

        public void AddListenerOnUpdateStat(HUJIStatType statType, Action onUpdateStat)
        {
            if (!_onUpdateStats.TryAdd(statType, onUpdateStat))
            {
                _onUpdateStats[statType] += onUpdateStat;
            }
        }

        public void RemoveListenerOnUpdateStat(HUJIStatType statType, Action onUpdateStat)
        {
            if (_onUpdateStats.TryGetValue(statType, out Action callback))
            {
                callback -= onUpdateStat;
                _onUpdateStats[statType] = callback;
            }
        }
    }
}