using System;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIBaseAbilityPrefab : HUJIGenericPoolPrefab
    {
        public Vector3 InitLocalPosition { get; private set; }
        public Vector3 InitLocalRotation { get; private set; }

        protected HUJIPlayerPrefabComponent _player;
        protected HUJIAbilityRuntime _currentAbility;

        protected virtual void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerAbility");

            var colliders = GetComponents<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            var rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void Awake()
        {
            InitLocalPosition = transform.localPosition;
            InitLocalRotation = transform.localEulerAngles;
        }

        public virtual void Initialize(HUJIPlayerPrefabComponent player, HUJIAbilityRuntime currentAbility)
        {
            _player = player;
            _currentAbility = currentAbility;

            transform.localPosition = InitLocalPosition;
            transform.localPosition = InitLocalRotation;
        }
    }
}