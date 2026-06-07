using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIAbilityRuntime
    {
        public AbilityConfig AbilityConfig;

        public int AnimationTrigger { get; private set; }
        [NonSerialized] public bool CanTrigger;
        [NonSerialized] public float AttackRange;

        private HUJIPlayerPrefabComponent _player;
        private Coroutine _coroutine;

        public void Initialize(HUJIPlayerPrefabComponent player, AbilityConfig abilityConfig)
        {
            _player = player;
            AbilityConfig = abilityConfig;
            AnimationTrigger = Animator.StringToHash(abilityConfig.AnimationTriggerString);
            CanTrigger = true;
        }

        public float GetAbilityStat(HUJIStatType statType)
        {
            var abilityStat = AbilityConfig.GetAbilityBaseStat(statType);
            if (abilityStat == 0)
            {
                return 0;
            }

            float basePlayerStat = _player.PlayerStat.GetStat(statType);
            return abilityStat * (1 + basePlayerStat);
        }

        public void StartCdCoroutine()
        {
            _player.StopAndStartCoroutine(ref _coroutine, SkillCdCoroutine());
        }

        private IEnumerator SkillCdCoroutine()
        {
            float cd = GetAbilityStat(HUJIStatType.AttackSpeed);
            if (cd == 0)
            {
                CanTrigger = true;
                yield break;
            }

            // HUJIManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnAbilityCd, (this, cd));
            CanTrigger = false;

            yield return new WaitForSeconds(cd);

            CanTrigger = true;
        }
    }
    [Serializable]
    public class HUJIPlayerAttack : HUJIBasePlayer
    {
        private const float ROTATE_SPEED = 10f;
        private const float ALIGNMENT_THRESHOLD = 0.98f;

        private HUJIAbilityRuntime _basicAbility;
        private List<HUJIAbilityRuntime> _abilities = new();

        private HUJIAbilityRuntime _currentAbility;
        public HUJIAbilityRuntime CurrentAbility => _currentAbility;
        private HUJISoundPrefabComponent _currentAttackSound;

        public bool IsAttacking => _currentAbility != null;

        private Coroutine _coroutine;

        public override void OnInitialize(HUJIPlayerPrefabComponent player)
        {
            base.OnInitialize(player);

            var basicAbilityConfig = Gamelogic.AbilityManager.Config.BasicAttackConfig;
            _basicAbility = new();
            _basicAbility.Initialize(_player, basicAbilityConfig);

            //here i will have more abilities that i will assign

            SetCurrentAbility();
        }

        public void SetCurrentAbility()
        {
            _currentAbility = GetNewCurrentAbility();
            _currentAbility.AttackRange = _currentAbility.GetAbilityStat(HUJIStatType.AttackRange);
            HUJIDebug.Log($"_currentAbility.AbilityConfig.StoppingDistance = {_currentAbility.AbilityConfig.StoppingDistance}");
            _player.PlayerMovement.Agent.stoppingDistance = _currentAbility.AbilityConfig.StoppingDistance; //???;

            HUJIAbilityRuntime GetNewCurrentAbility()
            {
                foreach (var ability in _abilities)
                {
                    if (ability.CanTrigger)
                    {
                        return ability;
                    }
                }

                return _basicAbility;
            }
        }

        public IEnumerator DoAttack(Vector3 enemyPosition)
        {
            Vector3 direction = _player.transform.position - enemyPosition;
            direction.y = 0f;
            float dirSqrMag = direction.sqrMagnitude;
            Vector3 forward;
            float dot;
            Transform t = _player.transform;
            Quaternion targetRot = Quaternion.LookRotation(-direction);

            if (dirSqrMag > 0.1f)
            {
                direction /= Mathf.Sqrt(dirSqrMag);

                forward = t.forward;
                dot = Vector3.Dot(forward, -direction);
                while (dot < ALIGNMENT_THRESHOLD)
                {
                    t.rotation = Quaternion.Slerp(t.rotation, targetRot, ROTATE_SPEED * Time.deltaTime);
                    forward = t.forward;
                    dot = Vector3.Dot(forward, -direction);

                    yield return null;
                }
            }

            _currentAbility.StartCdCoroutine();
            _player.PlayerAnimation.SetTrigger(_currentAbility.AnimationTrigger);

            yield return new WaitUntil(() => _currentAbility == null);
        }

        public void OnFinishAttack()
        {
            _currentAbility = null;
        }

        public void InstantiateAbility(int abilityUniqueId)
        {
            if (CurrentAbility == null || _currentAbility.AbilityConfig.AbilityUniqueId != abilityUniqueId)
            {
                return;
            }

            var abilityConfig = _currentAbility.AbilityConfig;
            var bodyHolder = _player.BodyMapping.GetBodyTransform(abilityConfig.BodyType);

            var abilityVisualInstance = (HUJIBaseAbilityPrefab)CoreManager.PoolManager.GetPoolPrefab(_currentAbility.AbilityConfig.VisualPoolName);
            if (abilityVisualInstance)
            {
                abilityVisualInstance.transform.SetParent(bodyHolder.transform);
                abilityVisualInstance.transform.localScale = Vector3.one;
                abilityVisualInstance.Initialize(_player, _currentAbility);
            }
        }
    }
}