using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemies
{
    public class RoadLayerSystem : MonoBehaviour
    {
        private struct RoadLayerData
        {
            public RoadLayer Layer;
            public bool Occupied;
        }

        [Title("Settings")]
        [Header("Origin")]
        [SerializeField] private Transform _origin;

        [Header("Sides")]
        [SerializeField] private float _sideLength = 8f;
        [SerializeField] private float _sideCenterOffset = 4f;
        [SerializeField] private float _sideForwardOffset = 4f;
        [SerializeField] private float _spawnOffsetFromSide = 1f;

        [Header("Layers")]
        [SerializeField] private int _layerCount = 6;

        [Header("Height")]
        [SerializeField] private float _height = 8f;
        [SerializeField] private float _lowBounds = 2f;

        [Header("Debug")]
        [SerializeField] private bool _drawGizmos = true;

        private RoadLayerData[] _layers;

        private List<RoadLayer> _unoccupiedLayers = new(16);

        private void Awake()
        {
            InitializeRoadLayers();
        }

        #region Layer logic
        private void InitializeRoadLayers()
        {
            _layers = new RoadLayerData[_layerCount];

            for (int i = 0; i < _layerCount; i++)
            {
                RoadLayer layer = new RoadLayer(this, i);
                _layers[i] = new RoadLayerData 
                { 
                    Layer = layer,
                    Occupied = false
                };
                _unoccupiedLayers.Add(layer);
            }
        }

        public bool TryGetUnoccupiedLayer(out RoadLayer layer)
        {
            layer = null;

            if (_unoccupiedLayers.Count == 0)
                return false;

            RoadLayer unoccupied = _unoccupiedLayers[_unoccupiedLayers.Count - 1];
            _unoccupiedLayers.RemoveAt(_unoccupiedLayers.Count - 1);

            _layers[unoccupied.Index] = new RoadLayerData
            {
                Layer = unoccupied,
                Occupied = true
            };

            layer = unoccupied;
            return true;
        }

        public bool TryRelease(int index)
        {
            if (_layers[index].Occupied == false)
            {
                Debug.LogWarning("Tried to release unoccupied road layer!");
                return false;
            }

            var layer = _layers[index].Layer;
            _layers[index] = new RoadLayerData
            {
                Layer = layer,
                Occupied = false
            };

            _unoccupiedLayers.Add(layer);
            return true;
        }
        #endregion

        #region Position calculations
        public Vector3 GetLeftPoint(int index)
        {
            return GetLayerPoint(index, -_sideCenterOffset);
        }

        public Vector3 GetRightPoint(int index)
        {
            return GetLayerPoint(index, _sideCenterOffset);
        }

        public Vector3 GetLeftSpawnPosition(int index)
        {
            return GetLayerPoint(
                index,
                -_sideCenterOffset - _spawnOffsetFromSide
            );
        }

        public Vector3 GetRightSpawnPosition(int index)
        {
            return GetLayerPoint(
                index,
                _sideCenterOffset + _spawnOffsetFromSide
            );
        }

        public VerticalBounds GetLeftSpawnBounds(int index)
        {
            Vector3 position = GetLeftSpawnPosition(index);

            return new VerticalBounds(
                position + Vector3.up * _lowBounds,
                position + Vector3.up * _height
            );
        }

        public VerticalBounds GetRightSpawnBounds(int index)
        {
            Vector3 position = GetRightSpawnPosition(index);

            return new VerticalBounds(
                position + Vector3.up * _lowBounds,
                position + Vector3.up * _height
            );
        }

        private Vector3 GetLayerPoint(int index, float horizontalOffset)
        {
            float layerSpacing = _sideLength / (_layerCount - 1);
            float distanceFromBack = index * layerSpacing;

            float forwardOffset =
                -(_sideLength - _sideForwardOffset - distanceFromBack);

            return _origin.position
                + _origin.right * horizontalOffset
                + _origin.forward * forwardOffset;
        }

        public RectBounds GetLayerBounds(int index)
        {
            Vector3 topLeft = GetLeftPoint(index) + Vector3.up * _height;
            Vector3 topRight = GetRightPoint(index) + Vector3.up * _height;
            Vector3 bottomLeft = GetLeftPoint(index) + Vector3.up * _lowBounds;
            Vector3 bottomRight = GetRightPoint(index) + Vector3.up * _lowBounds;

            return new RectBounds(topLeft, topRight, bottomLeft, bottomRight);
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
                return;

            Gizmos.color = Color.yellow;

            // Get the origin position
            Vector3 origin = _origin.position;

            // Calculate the four corners of the playable region
            // Forward direction (Z-axis) and Right direction (X-axis)
            Vector3 forward = _origin.forward;
            Vector3 right = _origin.right;

            // Calculate corner points
            // Front-left corner
            Vector3 frontLeft = origin
                - right * _sideCenterOffset
                + forward * _sideForwardOffset;

            // Front-right corner
            Vector3 frontRight = origin
                + right * _sideCenterOffset
                + forward * _sideForwardOffset;

            // Back-left corner
            Vector3 backLeft = origin
                - right * _sideCenterOffset
                - forward * (_sideLength - _sideForwardOffset);

            // Back-right corner
            Vector3 backRight = origin
                + right * _sideCenterOffset
                - forward * (_sideLength - _sideForwardOffset);

            // Draw the playable region box (yellow)
            // Bottom edges (on the floor)
            Gizmos.DrawLine(frontLeft, frontRight);  // Front edge
            Gizmos.DrawLine(backLeft, backRight);    // Back edge
            Gizmos.DrawLine(frontLeft, backLeft);    // Left edge
            Gizmos.DrawLine(frontRight, backRight);  // Right edge

            // Draw layer lines (red)
            Gizmos.color = Color.red;

            // Calculate positions for layer lines going from left to right
            float layerSpacing = _sideLength / (_layerCount - 1);

            for (int i = 0; i < _layerCount; i++)
            {
                // Calculate distance from back of the play area
                float distanceFromBack = i * layerSpacing;

                // Calculate the Z position relative to the play area
                Vector3 leftPoint = origin
                    - right * _sideCenterOffset
                    - forward * (_sideLength - _sideForwardOffset - distanceFromBack);

                Vector3 rightPoint = origin
                    + right * _sideCenterOffset
                    - forward * (_sideLength - _sideForwardOffset - distanceFromBack);

                Gizmos.DrawLine(leftPoint, rightPoint);

                // Draw spawn locations
                Gizmos.color = Color.blue;

                Vector3 leftSpawn = leftPoint + right * -_spawnOffsetFromSide;
                Vector3 rightSpawn = rightPoint + right * _spawnOffsetFromSide;

                Gizmos.DrawWireSphere(leftSpawn, 0.2f);
                Gizmos.DrawWireSphere(rightSpawn, 0.2f);

                Vector3 leftSpawnBound = leftSpawn + Vector3.up * _lowBounds;
                Vector3 leftSpawnTop = leftSpawn + Vector3.up * _height;
                Vector3 rightSpawnBound = rightSpawn + Vector3.up * _lowBounds;
                Vector3 rightSpawnTop = rightSpawn + Vector3.up * _height;

                Gizmos.DrawWireSphere(leftSpawnBound, 0.2f);
                Gizmos.DrawWireSphere(rightSpawnBound, 0.2f);

                Gizmos.DrawWireSphere(leftSpawnTop, 0.2f);
                Gizmos.DrawWireSphere(rightSpawnTop, 0.2f);

                Gizmos.DrawLine(leftSpawnBound, leftSpawnTop);
                Gizmos.DrawLine(rightSpawnBound, rightSpawnTop);

                Gizmos.color = Color.red;
            }

            // Draw vertical lines at corners to show height
            Gizmos.color = Color.yellow;
            float height = _height;

            var topFrontLeft = frontLeft + Vector3.up * height;
            var topFrontRight = frontRight + Vector3.up * height;
            var topBackLeft = backLeft + Vector3.up * height;
            var topBackRight = backRight + Vector3.up * height;

            // Draw height
            Gizmos.DrawLine(frontLeft, topFrontLeft);
            Gizmos.DrawLine(frontRight, topFrontRight);
            Gizmos.DrawLine(backLeft, topBackLeft);
            Gizmos.DrawLine(backRight, topBackRight);

            // Draw the playable region box (yellow)
            // Top edges (on the ceiling)
            Gizmos.DrawLine(topFrontLeft, topFrontRight);  // Front edge
            Gizmos.DrawLine(topBackLeft, topBackRight);    // Back edge
            Gizmos.DrawLine(topFrontLeft, topBackLeft);    // Left edge
            Gizmos.DrawLine(topFrontRight, topBackRight);  // Right edge

            var boundsFrontLeft = frontLeft + Vector3.up * _lowBounds;
            var boundsFrontRight = frontRight + Vector3.up * _lowBounds;
            var boundsBackLeft = backLeft + Vector3.up * _lowBounds;
            var boundsBackRight = backRight + Vector3.up * _lowBounds;

            // Draw bottom bounds
            // Top edges (for bottom bounds)
            Gizmos.color = Color.magenta;

            Gizmos.DrawLine(boundsFrontLeft, boundsFrontRight);  // Front edge
            Gizmos.DrawLine(boundsBackLeft, boundsBackRight);    // Back edge
            Gizmos.DrawLine(boundsFrontLeft, boundsBackLeft);    // Left edge
            Gizmos.DrawLine(boundsFrontRight, boundsBackRight);  // Right edge
        }
        #endregion
    }
}