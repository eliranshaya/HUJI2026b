using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace HUJI.Gamelogic
{
    [Serializable]
    public class HUJIPlayerMovement : HUJIBasePlayer
    {
        [SerializeField] private NavMeshAgent _agent;
        public NavMeshAgent Agent => _agent;

        private Coroutine _moveCoroutine;

        public override void OnInitialize(HUJIPlayerPrefabComponent player)
        {
            base.OnInitialize(player);

            _agent.speed = _player.PlayerStat.GetStat(HUJIStatType.MovementSpeed);

            _player.PlayerStat.AddListenerOnUpdateStat(HUJIStatType.MovementSpeed, UpdateMovementSpeed);

            EnableDisableMovement(true);
        }

        public override void OnDeath()
        {
            base.OnDeath();

            _player.StopWithNullCheckCoroutine(ref _moveCoroutine);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void UpdateMovementSpeed()
        {
            _agent.speed = _player.PlayerStat.GetStat(HUJIStatType.MovementSpeed);
        }

        public void EnableDisableMovement(bool enable)
        {
            if (enable)
            {
                _player.StopAndStartCoroutine(ref _moveCoroutine, MoveCoroutine());
            }
            else
            {
                _player.PlayerAnimation.UpdateMovement(0);

                _player.StopWithNullCheckCoroutine(ref _moveCoroutine);
            }
        }

        private IEnumerator MoveCoroutine()
        {
            if (_player.PlayerAttack.CurrentAbility == null)
            {
                _player.PlayerAttack.SetCurrentAbility();
            }

            Transform playerTransform = _player.transform;
            while (true)
            {
                var enemy = _gameManager.EnemySpawnService.FindClosestEnemy(_player.transform.position);
                if (!enemy)
                {
                    yield return null;
                    continue;
                }

                var enemyPosition = enemy.transform.position;

                if (_player.PlayerAttack.CurrentAbility != null)
                {
                    float distSq = playerTransform.position.SqrDistance(enemyPosition);
                    if (distSq <= _player.PlayerAttack.CurrentAbility.AttackRange)
                    {
                        _agent.isStopped = true;
                        _agent.velocity = Vector3.zero;
                        _agent.ResetPath();
                        
                        _player.PlayerAnimation.UpdateMovement(0);

                        yield return _player.PlayerAttack.DoAttack(enemyPosition);

                        _player.PlayerAttack.SetCurrentAbility();

                        _agent.isStopped = false;

                        continue;
                    }
                }

                _agent.SetDestination(enemyPosition);
                _player.PlayerAnimation.UpdateMovement(1);

                yield return null;
            }
        }
    }
}