using Game.Services;
using TMPro;
using UnityEngine;

namespace Game.Core
{
    public class MoneyHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;

        private MoneyService _moneyService;

        private void Start()
        {
            _moneyService = ServiceLocator.Get<MoneyService>();
            _moneyService.Changed += MoneyChanged;
            MoneyChanged(_moneyService.Money);
        }

        private void OnDestroy()
        {
            _moneyService.Changed -= MoneyChanged;
        }

        private void MoneyChanged(int obj)
        {
            _moneyText.text = _moneyService.Money.ToString();
        }
    }
}