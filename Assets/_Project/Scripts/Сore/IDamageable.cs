using System;

namespace Game.Core
{
    public interface IDamageable
    {
        public event Action<float> Damaged;

        public void TakeDamage(float amount);
    }
}