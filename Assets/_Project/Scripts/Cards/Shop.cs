using Alchemy.Inspector;
using Game.Core;
using Game.Levels;
using Game.Services;
using Game.Utilities;
using LitMotion;
using LitMotion.Extensions;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Cards
{
    public class Shop : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private ShopCard _cardPrefab;
        [SerializeField, Required] private RectTransform _cardSpawnParent;
        [SerializeField, Required] private GameObject _target;

        [Header("Animation Dependencies")]
        [SerializeField] private CanvasGroup _blackoutGroup;
        [SerializeField] private GameObject _backgroundGameObject;
        [SerializeField, Required] private GameObject _continueButton;

        [Header("Animation Settings")]
        [SerializeField] private float _blackoutDuration = 1.2f;

        private CardDatabase _database;
        private MoneyService _moneyService;
        private CardService _cardService;

        private ShopLevel _shopLevel;

        private MotionHandle _blackoutMotion;

        private void Start()
        {
            ServiceLocator.Register(this);

            _database = ServiceLocator.Get<CardDatabase>();
            _cardService = ServiceLocator.Get<CardService>();
            _moneyService = ServiceLocator.Get<MoneyService>();

            _blackoutGroup.alpha = 0f;
            _blackoutGroup.blocksRaycasts = false;
            ShowStuff(false);
            _cardSpawnParent.DestroyAllChildren();
            _target.SetActive(false);
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

        public async void OpenShop(ShopLevel level)
        {
            _target.SetActive(true);
            _blackoutGroup.alpha = 0f;
            _backgroundGameObject.SetActive(false);

            _shopLevel = level;

            await AnimateShop(true, () => SpawnCards(level.CardAmount));
        }

        public async void CloseShop()
        {
            _shopLevel.Complete();

            _blackoutGroup.alpha = 0f;
            _backgroundGameObject.SetActive(true);

            await AnimateShop(false, () => _cardSpawnParent.DestroyAllChildren());

            _target.SetActive(false);
        }

        private async Awaitable AnimateShop(bool showBackground, Action middleCallback)
        {
            if (_blackoutMotion.IsValid() && _blackoutMotion.IsPlaying())
                _blackoutMotion.Cancel();

            _blackoutMotion = LMotion.Create(0f, 1f, _blackoutDuration/2f)
                .BindToAlpha(_blackoutGroup);
            _blackoutMotion.AddTo(this);

            _blackoutGroup.blocksRaycasts = true;

            await _blackoutMotion;
            await Awaitable.WaitForSecondsAsync(1f, destroyCancellationToken);

            ShowStuff(showBackground);
            middleCallback?.Invoke();

            _blackoutMotion = LMotion.Create(1f, 0f, _blackoutDuration/2f)
                .BindToAlpha(_blackoutGroup);

            _blackoutMotion.AddTo(this);


            await _blackoutMotion;

            _blackoutGroup.blocksRaycasts = false;
        }

        [Button]
        private void TestSpawnThreeCards()
        {
            SpawnCards(3);
        }

        private void ShowStuff(bool visible)
        {
            _backgroundGameObject.SetActive(visible);
            _continueButton.SetActive(visible);
        }
    }
}