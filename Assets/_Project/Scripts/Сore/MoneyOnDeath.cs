using Alchemy.Inspector;
using Game.Services;
using System;
using UnityEngine;

namespace Game.Core
{
    public class MoneyOnDeath : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private Health _health;

        [Header("Settings")]
        [SerializeField] private int _amount = 1;

        private MoneyService _moneyService;

        private void Start()
        {
            _moneyService = ServiceLocator.Get<MoneyService>();
        }

        private void OnEnable()
        {
            _health.Died += OnDied;
        }

        private void OnDisable()
        {
            _health.Died -= OnDied;
        }

        private void OnDied()
        {
            _moneyService.Give(_amount);
        }
    }
}