using Alchemy.Inspector;
using UnityEngine;

namespace Game.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private RoadSpawnSystem _spawnSystem;

        [Header("Test enemy prefab")]
        [SerializeField] private Enemy _testEnemy;

        private void Awake()
        {
            if (_spawnSystem == null)
                _spawnSystem = GetComponent<RoadSpawnSystem>();
        }

        [Button]
        private void TestSpawnEnemy()
        {
            if (_testEnemy == null)
                return;

            _spawnSystem.TrySpawn(_testEnemy);
        }
    }
}