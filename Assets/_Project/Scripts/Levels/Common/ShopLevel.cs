using System;

namespace Game.Levels
{
    [Serializable]
    public class ShopLevel : LevelBase
    {
        public override bool IsInProcess => !_hasPurchased;
        public override bool IsFinished => _hasPurchased;

        private bool _hasPurchased = false;

        public override void Start()
        {
            _hasPurchased = false;
        }

        public override void Update() { }

        public void CompletePurchase()
        {
            _hasPurchased = true;
        }
    }
}