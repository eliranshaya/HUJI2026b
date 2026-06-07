using System.Collections.Generic;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIAoeAbilityPrefab : HUJIBaseAbilityPrefab
    {
        private readonly HashSet<string> _onTriggerEnters = new();

        
        public override void Initialize(HUJIPlayerPrefabComponent player, HUJIAbilityRuntime currentAbility)
        {
            base.Initialize(player, currentAbility);
            
            _onTriggerEnters.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_onTriggerEnters.Add(other.name))
            {
                var enemy = _player.GameManager.EnemySpawnService.GetEnemy(other.name);
                if (enemy)
                {
                    HUJIDebug.Log($"enemy = {enemy}");
                    float damage = _currentAbility.GetAbilityStat(HUJIStatType.AttackDamage);
                    // enemy.RemoveHealth(_player, other.hit, damage);
                }
            }
        }
    }
}