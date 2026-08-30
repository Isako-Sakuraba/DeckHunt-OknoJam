using UnityEngine;

namespace Game.Enemies
{
    public class RoadLayer
    {
        private readonly RoadLayerSystem _roadSystem;
        private readonly int _index;

        public int Index => _index;

        #region Properties
        // Layer line
        public Vector3 LeftPoint => _roadSystem.GetLeftPoint(_index);
        public Vector3 RightPoint => _roadSystem.GetRightPoint(_index);

        // Spawn positions at the bottom of the spawn area
        public Vector3 LeftSpawnPosition =>
            _roadSystem.GetLeftSpawnPosition(_index);

        public Vector3 RightSpawnPosition =>
            _roadSystem.GetRightSpawnPosition(_index);

        // Vertical spawn areas
        public VerticalBounds LeftSpawnBounds =>
            _roadSystem.GetLeftSpawnBounds(_index);

        public VerticalBounds RightSpawnBounds =>
            _roadSystem.GetRightSpawnBounds(_index);

        // Vertical
        public RectBounds LayerBounds =>
            _roadSystem.GetLayerBounds(_index);

        // Convenience
        public Vector3 LeftSpawnCenter =>
            LeftSpawnBounds.Center;

        public Vector3 RightSpawnCenter =>
            RightSpawnBounds.Center;

        #endregion

        public RoadLayer(RoadLayerSystem roadSystem, int layerIndex)
        {
            _roadSystem = roadSystem;
            _index = layerIndex;
        }

        public void Release()
        {
            _roadSystem.TryRelease(Index);
        }
    }
}