using Alchemy.Inspector;
using Game.Core;
using Game.Services;
using TMPro;
using UnityEngine;

namespace Game.Player
{
    public class DebugHud : MonoBehaviour
    {
        [SerializeField, Required] private Shotgun _shotgun;
        [SerializeField, Required] private TMP_Text _ammoText;
        [SerializeField, Required] private TMP_Text _cooldownText;
        [SerializeField, Required] private TMP_Text _moneyText;

        private MoneyService _moneyService;

        private void Start()
        {
            _moneyService = ServiceLocator.Get<MoneyService>();
        }

        private void Update()
        {
            _ammoText.text = $"{_shotgun.CurrentAmmo}/{_shotgun.MaxAmmo}";
            _cooldownText.text = _shotgun.CooldownTimer.ToString("F2");
            _moneyText.text = _moneyService.Money.ToString();
        }
    }
}