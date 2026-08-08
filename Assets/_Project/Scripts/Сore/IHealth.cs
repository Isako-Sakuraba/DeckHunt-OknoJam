using System;

namespace Game.Core
{
    public interface IHealth : IDamageable
    {
        public float Current { get; }
        public float Max { get; set; }

        public bool IsDead { get; }

        public event Action Died;

        public void Heal(float amount);
    }
}