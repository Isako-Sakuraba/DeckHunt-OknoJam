using Alchemy.Inspector;
using Game.Player;
using Game.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Cards
{
    public class CardService : MonoBehaviour
    {
        [SerializeField, Required] private Shotgun _shotgun;

        private Stack<CardConfig> _cardStack = new();

        public Shotgun Shotgun => _shotgun;

        private void Awake()
        {
            ServiceLocator.Register(this);

            if (_shotgun == null)
                _shotgun = GetComponent<Shotgun>();
        }

        public void PushCard(CardConfig card)
        {
            _cardStack.Push(card);
            if (card.CustomBehaviour != null)
                card.CustomBehaviour.OnCardPushed(this);
        }

        public CardConfig PopCard()
        {
            if (_cardStack.Count == 0)
                return null;

            _shotgun.PopModifier();
            return _cardStack.Pop();
        }
    }
}