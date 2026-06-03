using System;
using System.Collections;
using UnityEngine;

namespace HUJI.Gamelogic
{
    [Serializable]
    public class HUJIPlayerHealth : HUJIBasePlayer
    {
        private static readonly Color _emissionOff = Color.black;
        private static readonly Color _emissionOn = Color.red;
        private static readonly int _emissionColorPropertyId = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private CapsuleCollider _healthCollider;
        public CapsuleCollider HealthCollider => _healthCollider;

        [SerializeField] private Material _playerMaterial;
        [SerializeField] private float _damageColorDuration = 0.3f;
        [SerializeField] private AnimationCurve _animationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

        private Coroutine _colorCoroutine;

        private float _currentHealth;
        private float _maxHealth;

        // private Coroutine _healthRegenCoroutine;

        public bool GetIsAlive() => _currentHealth > 0;

        public override void OnInitialize(HUJIPlayerPrefabComponent player)
        {
            base.OnInitialize(player);

            _maxHealth = _player.PlayerStat.GetStat(HUJIStatType.Health);
            _currentHealth = _maxHealth;

            HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnUpdatePlayerHealth, (_currentHealth, _maxHealth));

            _player.PlayerStat.AddListenerOnUpdateStat(HUJIStatType.Health, UpdateMaxHealth);

            // _playerPrefab.StopAndStartCoroutine(ref _healthRegenCoroutine, HealthRegenCoroutine());
        }

        private void UpdateMaxHealth()
        {
            _maxHealth = _player.PlayerStat.GetStat(HUJIStatType.Health);

            HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnUpdatePlayerHealth, (_currentHealth, _maxHealth));
        }

        public void AddHealth(float value)
        {
            if (_currentHealth <= 0)
            {
                return;
            }

            _currentHealth = Mathf.Min(_currentHealth + value, _maxHealth);

            HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnUpdatePlayerHealth, (_currentHealth, _maxHealth));
        }

        public void ReduceHealth(float value)
        {
            float armor = _player.PlayerStat.GetStat(HUJIStatType.Armor);
            value = Mathf.Max(value - armor, 0);
            if (value == 0)
            {
                //maybe here spawn Ignore text and sound
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - value, 0);

            _player.StopAndStartCoroutine(ref _colorCoroutine, HitColorCoroutine());

            // SummonDamageText(value);

            HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnUpdatePlayerHealth, (_currentHealth, _maxHealth));
            if (_currentHealth <= 0)
            {
                _player.OnDeath();
            }
        }

        private IEnumerator HitColorCoroutine()
        {
            yield return HUJIExtensions.ChangeValueOverTimeEase(_emissionOff, _emissionOn, _damageColorDuration, color =>
            {
                _playerMaterial.SetColor(_emissionColorPropertyId, color);
            }, animationCurve: _animationCurve);
        }
    }
}