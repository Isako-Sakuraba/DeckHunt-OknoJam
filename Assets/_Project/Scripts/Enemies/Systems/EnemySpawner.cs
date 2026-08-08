using Alchemy.Inspector;
using Game.Core;
using Game.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private RoadSpawnSystem _spawnSystem;

        [Header("Enemies")]
        [SerializeField, Required] private AirEnemy _airEnemy;
        [SerializeField, Required] private GroundEnemy _groundEnemy;

        [Header("Test")]
        [SerializeField] private Enemy _testEnemy;

        private HashSet<Enemy> _aliveEnemies = new(20);

        public int AliveEnemiesCount => _aliveEnemies.Count;

        private void Awake()
        {
            if (_spawnSystem == null)
                _spawnSystem = GetComponent<RoadSpawnSystem>();

            ServiceLocator.Register(this);
        }

        public void TrySpawn(bool groundAllowed, bool airAllowed)
        {
            if (!groundAllowed && !airAllowed)
                return;

            bool spawnGround = (groundAllowed && airAllowed) ?
                               UnityEngine.Random.Range(0, 2) == 0 :
                               groundAllowed;

            Enemy instance = null;
            if (spawnGround)
            {
                _spawnSystem.TrySpawn(_groundEnemy, out instance);
            }
            else
            {
                _spawnSystem.TrySpawn(_airEnemy, out instance);
            }

            if (instance  != null)
            {
                _aliveEnemies.Add(instance);
                instance.Died += OnEnemyDied;
            }
        }

        private void OnEnemyDied(Enemy enemy)
        {
            enemy.Died -= OnEnemyDied;
            _aliveEnemies.Remove(enemy);
        }

        [Button]
        private void TestSpawnEnemy()
        {
            if (_testEnemy == null)
                return;

            _spawnSystem.TrySpawn(_testEnemy, out _);
        }
    }
}