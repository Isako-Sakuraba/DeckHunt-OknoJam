using Alchemy.Inspector;
using TMPro;
using UnityEngine;

namespace Game.Player
{
    public class DebugShotgunView : MonoBehaviour
    {
        [SerializeField, Required] private Shotgun _shotgun;
        [SerializeField, Required] private TMP_Text _ammoText;
        [SerializeField, Required] private TMP_Text _cooldownText;

        private void Update()
        {
            _ammoText.text = $"{_shotgun.CurrentAmmo}/{_shotgun.MaxAmmo}";
            _cooldownText.text = $"{_shotgun.ShootCooldown}";
        }

    }
}