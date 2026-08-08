using Alchemy.Inspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Core
{
    public class DamageReciever : MonoBehaviour, IDamageable
    {
        [HelpBox("Optional IHealth component")]
        [SerializeReference] private IHealth _health;

        [SerializeField] private bool _clampDamage = true;
        [SerializeField] private UnityEvent<float> DamagedUnityEvent = new();

        public event Action<float> Damaged = delegate { };

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

            _health.TakeDamage(amount);

            InvokeDamaged(amount);
        }

        private void InvokeDamaged(float amount)
        {
            Damaged.Invoke(amount);
            DamagedUnityEvent.Invoke(amount);
        }
    }
}