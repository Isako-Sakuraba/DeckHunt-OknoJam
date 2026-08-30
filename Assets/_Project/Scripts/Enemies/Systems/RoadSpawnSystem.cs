using UnityEngine;

namespace Game.Enemies
{
    public class RoadSpawnSystem : MonoBehaviour
    {
        private RoadLayerSystem _roadLayerSystem;

        private void Start()
        {
            if (_roadLayerSystem == null)
                _roadLayerSystem = GetComponent<RoadLayerSystem>();
        }

        /// <summary>
        /// Tries to spawn an enemy on the road. Instantiates the prefab.
        /// </summary>
        /// <param name="enemy">enemy prefab</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public bool TrySpawn(Enemy enemy, out Enemy instance)
        {
            instance = null;

            if (_roadLayerSystem.TryGetUnoccupiedLayer(out var roadLayer))
            {
                instance = Instantiate(enemy);
                instance.Spawn(roadLayer);
                return true;
            }

            return false;
        }
    }
}