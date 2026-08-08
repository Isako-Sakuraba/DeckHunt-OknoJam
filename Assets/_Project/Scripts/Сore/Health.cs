using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Core
{
    public class Health : MonoBehaviour, IHealth
    {
        [SerializeField]
        private float _current = 20f;

        [field: SerializeField]
        public float Max { get; set; } = 20f;

        [SerializeField] private UnityEvent DiedUnityEvent = new();

        public float Current => _current;
        public bool IsDead => Current <= 0f;

        public event Action Died = delegate { };
        public event Action<float> Damaged = delegate { };

        private void Awake()
        {
            if (IsDead)
                _current = Max;
        }

        public void Heal(float amount)
        {
            _current += amount;
            ClampHealth();
        }

        public void TakeDamage(float amount)
        {
            if (IsDead)
                return;

            _current -= amount;
            Damaged.Invoke(amount);

            if (_current < 0f)
            {
                Died.Invoke();
                DiedUnityEvent.Invoke();
            }

            ClampHealth();
        }

        private void ClampHealth()
        {
            _current = Mathf.Clamp(_current, 0f, Max);
        }
    }
}