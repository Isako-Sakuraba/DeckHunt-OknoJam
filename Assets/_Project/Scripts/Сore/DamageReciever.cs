using Alchemy.Inspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Core
{
    public class DamageReciever : MonoBehaviour, IDamageable
    {
        [SerializeField] private bool _clampDamage = true;
        [SerializeField] private UnityEvent<float> DamagedUnityEvent = new();

        private IHealth _health;

        public event Action<float> Damaged = delegate { };


        public void TakeDamage(float amount)
        {
            InvokeDamaged(amount);
        }

        private void InvokeDamaged(float amount)
        {
            Damaged.Invoke(amount);
            DamagedUnityEvent.Invoke(amount);
        }
    }
}