using Game.Cards;
using Game.Services;
using System;
using UnityEngine;

namespace Game.Levels
{
    [Serializable]
    public class ShopLevel : LevelBase
    {
        public override bool IsInProcess => !_hasPurchased;
        public override bool IsFinished => _hasPurchased;

        [SerializeField] private int _cardAmount = 3;
        private bool _hasPurchased = false;

        public int CardAmount => _cardAmount;

        public override void Start()
        {
            _hasPurchased = false;
            Shop shop = ServiceLocator.Get<Shop>();
            shop.OpenShop(this);
        }

        public override void Update() { }

        public void Complete()
        {
            _hasPurchased = true;
        }
    }
}