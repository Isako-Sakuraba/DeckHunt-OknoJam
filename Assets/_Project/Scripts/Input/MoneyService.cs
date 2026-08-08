using Game.Lifecycle;
using Game.Services;
using System;
using UnityEngine;

namespace Game.Core
{
    public class MoneyService : MonoBehaviour, IBootstrapable
    {
        private int _money;

        public int Money => _money;

        public event Action<float> Changed = delegate { };

        public void Bootstrap()
        {
            ServiceLocator.Register(this);
        }

        public bool TrySpend(int amount)
        {
            if (_money <= 0)
                return false;

            amount = amount > _money ? _money : amount;

            _money -= amount;

            Changed.Invoke(amount);

            return true;
        }

        public void Give(int amount)
        {
            _money += amount;

            Changed.Invoke(amount);
        }
    }
}