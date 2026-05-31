using TMPro;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIGameManagerComponent : HUJIMonoBehaviour
    {
        [SerializeField] private HUJIPlayerPrefabComponent _playerPrefab;

        // public HUJIEnemySpawnService EnemySpawnService => _enemySpawnService;

        // public HUJIPlayerPrefabComponent Player { get; private set; }

        // private HUJIBaseService[] _services;

        private void Awake()
        {
            // _services = new HUJIBaseService[]
            // {
            //     _dayNightService, _enemySpawnService
            // };
            //
            // foreach (var service in _services)
            // {
            //     service.OnAwake(this);
            // }
            
            // Vector3 spawnPos = GetGroundPosition(35f, 35f);
            // Player = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
            // Player.Init(this);
        }

        // private void Start()
        // {
        //     foreach (var service in _services)
        //     {
        //         service.OnStart();
        //     }
        // }
        //
        // private void OnDestroy()
        // {
        //     foreach (var service in _services)
        //     {
        //         service.OnDestroy();
        //     }
        // }
    }
}