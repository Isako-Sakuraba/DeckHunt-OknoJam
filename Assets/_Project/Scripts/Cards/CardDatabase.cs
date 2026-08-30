using Game.Lifecycle;
using Game.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Cards
{
    public class CardDatabase : MonoBehaviour, IBootstrapable
    {
        private const string CardsPath = "Cards";

        private List<CardConfig> _allowedCards = new();

        private void Awake()
        {
            _allowedCards.Clear();

            var loadedObjects = Resources.LoadAll(CardsPath);

            foreach (var loaded in loadedObjects)
            {
                if (loaded is CardConfig card)
                    _allowedCards.Add(card);
            }
        }

        public void Bootstrap()
        {
            ServiceLocator.Register(this);
        }

        public CardConfig[] GetRandomCards(int amount)
        {
            if (amount <= 0)
                return System.Array.Empty<CardConfig>();

            if (amount > _allowedCards.Count)
                return System.Array.Empty<CardConfig>();

            // Shallow copy to avoid mutating the serialized source list
            var pool = new List<CardConfig>(_allowedCards);
            var result = new CardConfig[amount];

            // Partial Fisher-Yates shuffle using UnityEngine.Random
            for (int i = 0; i < amount; i++)
            {
                int swapIndex = UnityEngine.Random.Range(i, pool.Count);
                (pool[i], pool[swapIndex]) = (pool[swapIndex], pool[i]);
                result[i] = pool[i];
            }

            return result;
        }

        public CardConfig GetRandomCard()
        {
            if (_allowedCards == null || _allowedCards.Count == 0)
                return null;

            int index = UnityEngine.Random.Range(0, _allowedCards.Count);
            return _allowedCards[index];
        }
    }
}