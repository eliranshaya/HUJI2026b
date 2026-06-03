using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIGameManagerComponent : HUJIMonoBehaviour
    {
        [SerializeField] private HUJIPlayerPrefabComponent _playerPrefab;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private Transform _defaultTargetPosition;

        [SerializeField] private Vector3 _spawnPosition;

        [SerializeField] private HUJIEnemySpawnService _enemySpawnService;
        public HUJIEnemySpawnService EnemySpawnService => _enemySpawnService;

        public HUJIPlayerPrefabComponent Player { get; private set; }

        private HUJIBaseService[] _services;

        private void Awake()
        {
            _services = new HUJIBaseService[] { _enemySpawnService };

            foreach (var service in _services)
            {
                service.OnAwake(this);
            }

            Player = Instantiate(_playerPrefab, _spawnPosition, Quaternion.identity);
            Player.Init(this);

            _cinemachineCamera.Follow = _defaultTargetPosition;
        }

        private void Start()
        {
            foreach (var service in _services)
            {
                service.OnStart();
            }
        }

        private void Update()
        {
            _defaultTargetPosition.transform.position = Player.transform.position;
        }

        private void OnDestroy()
        {
            foreach (var service in _services)
            {
                service.OnDestroy();
            }
        }
    }
}