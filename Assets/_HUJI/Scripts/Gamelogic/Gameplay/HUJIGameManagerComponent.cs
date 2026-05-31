using TMPro;
using UnityEngine;

namespace HUJI.Gamelogic
{
    public class HUJIGameManagerComponent : HUJIMonoBehaviour
    {
        [SerializeField] private HUJIPlayerPrefabComponent _playerPrefab;
        [SerializeField] private Vector3 _spawnPosition;


        // public HUJIEnemySpawnService EnemySpawnService => _enemySpawnService;

        public HUJIPlayerPrefabComponent Player { get; private set; }

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
            
            Player = Instantiate(_playerPrefab, _spawnPosition, Quaternion.identity);
            Player.Init(this);
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