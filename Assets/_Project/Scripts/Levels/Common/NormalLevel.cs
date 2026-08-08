using Game.Enemies;
using Game.Services;
using Game.Utilities;
using System;
using UnityEngine;

namespace Game.Levels
{
    [Serializable]
    public class NormalLevel : LevelBase
    {
        [Header("Settings")]
        [SerializeField] private int _animalsCount = 20;
        [SerializeField] private Vector2 _animalSpawnDelay = new Vector2(4f, 10f);
        [SerializeField] private Vector2 _starterSpawnDelay = new Vector2(1f, 3f);
        [SerializeField] private bool _spawnAir = true;
        [SerializeField] private bool _spawnGround = true;

        private EnemySpawner _enemySpawner;

        private int _animalsLeftToSpawn = 1;
        private float _animalSpawnTimer;

        private Action _spawnTimerCallback;

        public override bool IsInProcess => !IsLevelFinished();
        public override bool IsFinished => IsLevelFinished();

        public NormalLevel()
        {
            _animalsLeftToSpawn = _animalsCount;
            _spawnTimerCallback = SpawnTimerCallback;
        }

        public override void Start()
        {
            _enemySpawner = ServiceLocator.Get<EnemySpawner>();

            _animalsLeftToSpawn = _animalsCount; // Just to be safe
            Utils.RandomizeValue(ref _animalSpawnTimer, _starterSpawnDelay);
        }

        public override void Update()
        {
            Utils.UpdateTimer(ref _animalSpawnTimer, _spawnTimerCallback);
        }

        public void SpawnTimerCallback()
        {
            _enemySpawner.TrySpawn(_spawnGround, _spawnAir);
            Utils.RandomizeValue(ref _animalSpawnTimer, _animalSpawnDelay);
        }

        private bool IsLevelFinished()
        {
            return _animalsLeftToSpawn <= 0 && _enemySpawner.AliveEnemiesCount == 0;
        }
    }
}