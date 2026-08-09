using Alchemy.Inspector;
using Game.Core;
using Game.Player;
using Game.Services;
using Game.Utilities;
using UnityEngine;

namespace Game.Cards
{
    public class Shop : MonoBehaviour
    {
        [SerializeField, Required] private ShopCard _cardPrefab;
        [SerializeField, Required] private RectTransform _cardSpawnParent;

        private CardDatabase _database;
        private MoneyService _moneyService;
        private CardService _cardService;

        private void Start()
        {
            ServiceLocator.Register(this);

            _database = ServiceLocator.Get<CardDatabase>();
            _cardService = ServiceLocator.Get<CardService>();
            _moneyService = ServiceLocator.Get<MoneyService>();
        }

        public void SpawnCards(int amount)
        {
            _cardSpawnParent.DestroyAllChildren();

            var configs = _database.GetRandomCards(amount);

            if (configs.Length == 0)
                return;

            foreach (var config in configs)
            {
                ShopCard instance = Instantiate(_cardPrefab, _cardSpawnParent);
                instance.Initialize(this, config);
            }
        }

        public bool TryBuy(CardConfig card)
        {
            if (_moneyService.TrySpend(card.Price))
            {
                _cardService.PushCard(card);
                return true;
            }

            return false;
        }

        [Button]
        private void TestSpawnThreeCards()
        {
            SpawnCards(3);
        }
    }
}