using System;
using UnityEngine;

namespace HUJI.Gamelogic
{
    [Serializable]
    public class HUJIPlayerAnimation : HUJIBasePlayer
    {
        [SerializeField] private Animator _animator;
        public Animator Animator => _animator;

        private readonly int _movementHash = Animator.StringToHash("Movement");

        public void UpdateMovement(float movement)
        {
            _animator.SetFloat(_movementHash, movement);
        }

        public void SetTrigger(int trigger)
        {
            _animator.SetTrigger(trigger);
        }

        public void ResetTrigger(int trigger)
        {
            _animator.ResetTrigger(trigger);
        }
    }
}