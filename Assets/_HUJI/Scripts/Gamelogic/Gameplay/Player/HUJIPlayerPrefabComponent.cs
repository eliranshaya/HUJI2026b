using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIPlayerPrefabComponent : HUJIMonoBehaviour
    {
        public HUJIGameManagerComponent GameManager { get; private set; }

        [HUJIBoxGroup("BodyMapping")] [SerializeField] private HUJIBodyMapping _bodyMapping;
        public HUJIBodyMapping BodyMapping => _bodyMapping;

        [HUJIBoxGroup("Movement")] [SerializeField] private HUJIPlayerMovement _playerMovement;
        public HUJIPlayerMovement PlayerMovement => _playerMovement;

        [HUJIBoxGroup("Animation")] [SerializeField] private HUJIPlayerAnimation _playerAnimation;
        public HUJIPlayerAnimation PlayerAnimation => _playerAnimation;

        [HUJIBoxGroup("Attack")] [SerializeField] private HUJIPlayerAttack _playerAttack;
        public HUJIPlayerAttack PlayerAttack => _playerAttack;

        [HUJIBoxGroup("Stat")] [SerializeField] private HUJIPlayerStat _playerStat;
        public HUJIPlayerStat PlayerStat => _playerStat;

        [HUJIBoxGroup("Health")] [SerializeField] private HUJIPlayerHealth _playerHealth;
        public HUJIPlayerHealth PlayerHealth => _playerHealth;

        private HUJIBasePlayer[] _services;

        #region Editor:

#if UNITY_EDITOR
        [HUJIButton("SetBodyMapping")]
        private void SetBodyMapping()
        {
            _bodyMapping.SetBodyMapping(transform);
            SetDirty();
        }

        [HUJIButton("OnUpdatePlayerHealth")]
        private void OnUpdatePlayerHealth()
        {
            HUJICoreManager.Instance.EventManager.InvokeEvent(HUJIEventName.OnUpdatePlayerHealth, (Random.Range(10,50), 50));
        }
#endif

        #endregion

        public void Init(HUJIGameManagerComponent gameManager)
        {
            GameManager = gameManager;

            _services = new HUJIBasePlayer[]
            {
                _playerStat,
                _playerAnimation,
                _playerAttack,
                _playerHealth,
                _playerMovement,
            };

            foreach (var service in _services)
            {
                service.OnInitialize(this);
            }
        }

        public void OnDeath()
        {
            foreach (var service in _services)
            {
                // service.OnDeath();
            }
        }

        private void OnDestroy()
        {
            foreach (var service in _services)
            {
                // service.OnDestroy();
            }
        }
//
//         #region AnimationEvents:
//
//         private void StartAttackTrigger(HUJIAbilityType abilityType) => _playerAttack.ActiveAbility(abilityType);
//         private void EndAttackTrigger(HUJIAbilityType abilityType) => _playerAttack.EndAbilityAnimation(abilityType);
//         public void InstantiateAbilityCollider(HUJIAbilityType abilityType) => _playerAttack.InstantiateAbilityCollider(abilityType);
//         private void PlayAttackSound(HUJISoundConfig soundConfig) => _playerAttack.PlayAttackSound(soundConfig);
//         private void StopAttackSound() => _playerAttack.StopAttackSound();
//
//         private void SetCanRotate(HUJIBooleanType booleanType) => _playerCamera.SetCanRotate(booleanType);
//
//         private void ExecutePush(HUJIPushConfig pushConfig) => _playerMovement.PushMovementCameraDirection(pushConfig);
//         private void EnableDisableMovement(HUJIBooleanType booleanType) => _playerMovement.EnableDisableMovement(booleanType == HUJIBooleanType.True);
//
//         private void PlayFootstep(AnimationEvent evt) => _playerSound.PlayFootstep(evt);
//         private void PlaySound(HUJISoundConfig soundConfig) => _playerSound.PlaySound(soundConfig);
//
//         #endregion
//
//         public bool CanBeDamagedBy(HUJIWeaponType weaponType) => true;
//
//         public void ReduceHealth(float damage, Collider other) => _playerHealth.ReduceHealth(damage);
    }
}