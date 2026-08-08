using UnityEngine;

namespace Game.Enemies
{
    public class AirEnemy : Enemy
    {
        private enum AirEnemyState
        {
            Entering,
            Flying,
            Leaving
        }

        [Header("Settings")]
        [SerializeField] private Vector2Int _directionChangeCount = new Vector2Int(4, 8);
        [SerializeField] private Vector2 _speedRange = new Vector2(4f, 8f);
        [SerializeField] private float _minNextPointRadius = 10f;
        [SerializeField] private float _nextPointAngle = 240f;

        private bool _spawnedLeft = false;
        private AirEnemyState _state = AirEnemyState.Entering;

        private Vector3 _movementTarget = Vector3.zero;

        private int _directionChangesLeft = 2;
        private float _currentSpeed = 2f;

        private Vector3 _velocity = Vector3.zero;

        private bool HasReachedMovementTarget => (_movementTarget - transform.position).sqrMagnitude <= 0.01f;

        protected override void LateSpawn()
        {
            Vector3 spawnPoint = GetRandomSpawnPoint();

            transform.position = spawnPoint;

            _state = AirEnemyState.Entering;
            _movementTarget = GetRandomPointInBounds();
            
            RandomizeValue(ref _directionChangesLeft, _directionChangeCount);
            RandomizeValue(ref _currentSpeed, _speedRange);
        }

        protected override void LateDespawn()
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            Vector3 oldPosition = transform.position;

            if (_state == AirEnemyState.Entering)
                UpdateEnteringState();
            else if (_state == AirEnemyState.Flying)
                UpdateFlyingState();
            else if (_state == AirEnemyState.Leaving)
                UpdateLeavingState();

            Vector3 newPosition = transform.position;

            _velocity = newPosition - oldPosition;
        }

        private void UpdateLeavingState()
        {
            MoveTowardsTarget();

            if (HasReachedMovementTarget)
            {
                Despawn();
            }
        }

        private void UpdateFlyingState()
        {
            MoveTowardsTarget();

            if (HasReachedMovementTarget)
            {
                _directionChangesLeft--;
                if (_directionChangesLeft <= 0)
                {
                    _state = AirEnemyState.Leaving;
                    RandomizeValue(ref _currentSpeed, _speedRange);
                    _movementTarget = GetRandomSpawnPoint(_spawnedLeft ? RoadLayer.RightSpawnBounds : RoadLayer.LeftSpawnBounds);
                    return;
                }

                RandomizeValue(ref _currentSpeed, _speedRange);
                _movementTarget = GetRandomPointInBoundsComplex();
            }
        }

        private void UpdateEnteringState()
        {
            MoveTowardsTarget();

            if (HasReachedMovementTarget)
            {
                _state = AirEnemyState.Flying;
                RandomizeValue(ref _currentSpeed, _speedRange);
                _movementTarget = GetRandomPointInBounds();
            }
        }

        private void MoveTowardsTarget()
        {
            transform.position = Vector3.MoveTowards(transform.position, _movementTarget, _currentSpeed * Time.deltaTime);
            //transform.position = Vector3.Slerp(transform.position, _movementTarget, _currentSpeed * Time.deltaTime);
        }

        private Vector3 GetRandomSpawnPoint(in VerticalBounds bounds)
        {
            float uniform = UnityEngine.Random.Range(0f, 1f);

            return bounds.GetPosition(uniform);
        }

        private Vector3 GetRandomSpawnPoint()
        {
            float uniform = UnityEngine.Random.Range(0f, 1f);
            VerticalBounds bounds;
            if (uniform < 0.5f)
            {
                _spawnedLeft = true;
                bounds = RoadLayer.LeftSpawnBounds;
            }
            else
            {
                bounds = RoadLayer.RightSpawnBounds;
            }

            return Vector3.Lerp(bounds.Bottom, bounds.Top, uniform);
        }

        private Vector3 GetRandomPointInBounds()
        {
            float horizontal = UnityEngine.Random.Range(0f, 1f);
            float vertical = UnityEngine.Random.Range(0f, 1f);

            RectBounds bounds = RoadLayer.LayerBounds;

            return bounds.GetPosition(horizontal, vertical);
        }

        private static void RandomizeValue(ref float value, in Vector2 range)
        {
            value = UnityEngine.Random.Range(range.x, range.y);
        }

        private static void RandomizeValue(ref int value, in Vector2Int range)
        {
            value = UnityEngine.Random.Range(range.x, range.y);
        }

        private Vector3 GetRandomPointInBoundsComplex()
        {
            RectBounds bounds = RoadLayer.LayerBounds;

            float forbiddenAngle = (360f - _nextPointAngle) / 2f;
            float minRadiusSqr = _minNextPointRadius * _minNextPointRadius;

            const int maxAttempts = 16;

            for (int i = 0; i < maxAttempts; i++)
            {
                float horizontal = UnityEngine.Random.Range(0f, 1f);
                float vertical = UnityEngine.Random.Range(0f, 1f);

                Vector3 point = bounds.GetPosition(horizontal, vertical);

                Vector3 toPoint = point - transform.position;

                // Too close to the current position.
                if (toPoint.sqrMagnitude < minRadiusSqr)
                    continue;

                // Reject points too close to the current movement direction.
                if (_velocity.sqrMagnitude > 0.001f)
                {
                    float angle = Vector3.Angle(_velocity, toPoint);

                    if (angle < forbiddenAngle)
                        continue;
                }

                return point;
            }

            // No valid point found.
            return bounds.GetPosition(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
            );
        }

        private bool IsInsideBounds(Vector3 point, RectBounds bounds)
        {
            return point.x >= Mathf.Min(bounds.BottomLeft.x, bounds.TopRight.x) &&
                   point.x <= Mathf.Max(bounds.BottomLeft.x, bounds.TopRight.x) &&
                   point.y >= Mathf.Min(bounds.BottomLeft.y, bounds.TopLeft.y) &&
                   point.y <= Mathf.Max(bounds.BottomLeft.y, bounds.TopLeft.y) &&
                   point.z >= Mathf.Min(bounds.BottomLeft.z, bounds.BottomRight.z) &&
                   point.z <= Mathf.Max(bounds.BottomLeft.z, bounds.BottomRight.z);
        }
    }
}