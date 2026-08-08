using Alchemy.Inspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Core
{
    public class DamageReciever : MonoBehaviour, IDamageable
    {
        [HelpBox("Optional IHealth component")]
        [SerializeField] private MonoBehaviour _healthMonoBehaviour;

        [SerializeField] private bool _clampDamage = true;
        [SerializeField] private UnityEvent<float> DamagedUnityEvent = new();

        private IHealth _health;

        public event Action<float> Damaged = delegate { };

        private void Awake()
        {
            if (_healthMonoBehaviour != null)
            {
                if (_healthMonoBehaviour is IHealth health)
                {
                    _health = health;
                }
                else
                {
                    Debug.LogError($"The {nameof(_healthMonoBehaviour)} must be an IHealth!", this);
                }
            }
        }

        public void TakeDamage(float amount)
        {
            if (_health == null)
            {
                InvokeDamaged(amount);
                return;
            }

            if (_health.IsDead)
                return;

            if (_clampDamage)
                amount = amount > _health.Current ? _health.Current : amount;

            _health.Damage(amount);

            InvokeDamaged(amount);
        }

        private void InvokeDamaged(float amount)
        {
            Damaged.Invoke(amount);
            DamagedUnityEvent.Invoke(amount);
        }
    }
}