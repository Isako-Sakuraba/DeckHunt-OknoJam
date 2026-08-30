using UnityEngine;

namespace Game.Enemies
{
    public class GroundEnemy : Enemy
    {
        [Header("Settings")]
        [SerializeField] private float _speed = 2f;

        private bool _spawnedLeft = false;

        public Vector3 MovementTarget => 
            _spawnedLeft ? 
            RoadLayer.RightSpawnPosition : 
            RoadLayer.LeftSpawnPosition;

        protected override void LateSpawn()
        {
            Vector3 spawnPoint = GetRandomSpawnPoint();

            transform.position = spawnPoint;
        }

        private void Update()
        {
            Target.position = Vector3.MoveTowards(
                Target.position, 
                MovementTarget, 
                Time.deltaTime * _speed);

            if ((MovementTarget - Target.position).sqrMagnitude < 0.01f)
            {
                Despawn();
            }
        }

        protected override void LateDespawn()
        {
            Destroy(gameObject);
        }

        private Vector3 GetRandomSpawnPoint()
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                _spawnedLeft = true;
                return RoadLayer.LeftSpawnPosition;
            } 
            else
            {
                return RoadLayer.RightSpawnPosition;
            }
        }
    }
}