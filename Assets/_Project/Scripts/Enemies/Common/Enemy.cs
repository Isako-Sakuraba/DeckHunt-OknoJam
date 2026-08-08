using Alchemy.Inspector;
using Game.Core;
using System;
using UnityEngine;

namespace Game.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] protected Transform Target;
        [SerializeField, Required] protected Health Health;

        protected RoadLayer RoadLayer;

        public event Action<Enemy> Died = delegate { };

        private void Awake()
        {
            if (Target == null)
                Target = transform;

            if (Health == null)
                Health = GetComponent<Health>();

            LateAwake();
        }

        private void OnEnable()
        {
            Health.Died += OnDied;
        }

        private void OnDisable()
        {
            Health.Died -= OnDied;
        }

        private void OnDied()
        {
            Died.Invoke(this);
            Despawn();
        }

        protected virtual void LateAwake() { }

        public void Spawn(RoadLayer layer)
        {
            RoadLayer = layer;
            LateSpawn();
        }

        public void Despawn()
        {
            LateDespawn();
            RoadLayer.Release();
        }

        protected virtual void LateDespawn() { }

        protected virtual void LateSpawn() { }
    }
}