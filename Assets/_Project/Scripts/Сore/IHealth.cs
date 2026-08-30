using System;

namespace Game.Core
{
    public interface IHealth
    {
        public float Current { get; }
        public float Max { get; set; }

        public bool IsDead { get; }

        public event Action Died;
        public event Action<float> Changed;

        public void Heal(float amount);
        public void Damage(float amount);
    }
}