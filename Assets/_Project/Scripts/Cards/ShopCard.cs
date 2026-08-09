using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Cards
{
    public class ShopCard : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;

        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _priceText;

        [SerializeField] private GameObject _purchasedTextGameObject;

        private Shop _shop;
        private CardConfig _config;

        private bool _bought = false;

        public void Initialize(Shop shop, CardConfig config)
        {
            _shop = shop;
            _config = config;
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            _icon.sprite = _config.Icon;
            _title.text = _config.Title;
            _priceText.text = $"Buy for {_config.Price.ToString()}";
            _description.text = _config.CustomBehaviour == null ? 
                "No description" : 
                _config.CustomBehaviour.GetDescription();

            _buyButton.gameObject.SetActive(!_bought);
            _purchasedTextGameObject.SetActive(_bought);
        }

        public void Buy()
        {
            if (_bought)
                return;

            if (_shop.TryBuy(_config))
            {
                _bought = true;
            }

            UpdateVisuals();
        }
    }
}