using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    [Serializable]
    public struct ShotgunValues
    {
        public float Damage;
        public float DamageMultiplier;
        public int Ammo;
        public float ReloadTime;
        public float ReloadTimeMultiplier;
        public float ShootCooldown;
        public float ShootCooldownMultiplier;
    }

    public class Shotgun : MonoBehaviour
    {
        [SerializeField] private ShotgunValues _baseValues;
        [SerializeField] private ShotgunValues _minValues;

        private int _currentAmmo;

        private float _shootCooldownTimer;
        private float _reloadTimer;

        public float Damage => _cachedValue.Damage * _cachedValue.DamageMultiplier;
        public int MaxAmmo => _cachedValue.Ammo;
        public int CurrentAmmo => _currentAmmo;

        public float ReloadTime => _cachedValue.Damage * _cachedValue.DamageMultiplier;
        public float ReloadTimer => _reloadTimer;

        public float ShootCooldown => _cachedValue.ShootCooldown * _cachedValue.ShootCooldownMultiplier;
        public float ShootCooldownTimer => _shootCooldownTimer;

        private Action<ShotgunValues> Shot = delegate { };
        private Action<ShotgunValues> Reload = delegate { };

        private ShotgunValues _cachedValue;
        private Stack<ShotgunValues> _shotgunStack = new();

        private void RecalculateValues()
        {
            _cachedValue = _baseValues;

            foreach (ShotgunValues modifier in _shotgunStack)
            {
                _cachedValue.Damage += modifier.Damage;
                _cachedValue.DamageMultiplier += modifier.DamageMultiplier;
                _cachedValue.Ammo += modifier.Ammo;
                _cachedValue.ReloadTime += modifier.ReloadTime;
                _cachedValue.ReloadTimeMultiplier += modifier.ReloadTimeMultiplier;
                _cachedValue.ShootCooldown += modifier.ShootCooldown;
                _cachedValue.ShootCooldownMultiplier += modifier.ReloadTimeMultiplier;
            }

            _cachedValue = ClampValues(_cachedValue, _minValues);
        }

        private ShotgunValues ClampValues(ShotgunValues target, ShotgunValues min)
        {
            var values = new ShotgunValues();

            values.Damage = Mathf.Max(target.Damage, min.Damage);
            values.DamageMultiplier = Mathf.Max(target.DamageMultiplier, min.DamageMultiplier);
            values.Ammo = Mathf.Max(target.Ammo, min.Ammo);
            values.ReloadTime = Mathf.Max(target.ReloadTime, min.ReloadTime);
            values.ReloadTimeMultiplier = Mathf.Max(target.ReloadTimeMultiplier, min.ReloadTimeMultiplier);
            values.ShootCooldown = Mathf.Max(target.ShootCooldown, min.ShootCooldown);
            values.ShootCooldownMultiplier = Mathf.Max(target.ShootCooldownMultiplier, min.ShootCooldownMultiplier);

            return values;
        }
    }
}