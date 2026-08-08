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

        private float _cooldownTimer;
        private Action _cooldownCallback;

        public float Damage => _cachedValue.Damage * _cachedValue.DamageMultiplier;
        public int MaxAmmo => _cachedValue.Ammo;
        public int CurrentAmmo => _currentAmmo;
        public bool IsEmpty => _currentAmmo <= 0;

        public float ReloadTime => _cachedValue.Damage * _cachedValue.DamageMultiplier;
        public bool IsReloading => IsEmpty && _cooldownTimer > 0f;

        public float ShootCooldown => _cachedValue.ShootCooldown * _cachedValue.ShootCooldownMultiplier;
        public bool IsInCooldown => !IsEmpty && _cooldownTimer > 0f;

        private Action<float> Shot = delegate { };
        private Action<float> Reload = delegate { };

        private ShotgunValues _cachedValue;
        private Stack<ShotgunValues> _shotgunStack = new();

        private void Awake()
        {
            _cooldownCallback = RestoreAmmo;            
        }

        private void Start()
        {
            RecalculateValues();
        }

        private void RestoreAmmo()
        {
            _currentAmmo = MaxAmmo;
        }

        public bool TryShoot()
        {
            if (IsInCooldown || IsReloading)
                return false;

            Shot.Invoke(Damage);

            _currentAmmo--;

            _cooldownTimer = IsEmpty ? ShootCooldown + ReloadTime : ShootCooldown;

            if (IsEmpty)
            {
                _cooldownTimer = ShootCooldown + ReloadTime;
                Reload.Invoke(ReloadTime);
            }
            else
            {
                _cooldownTimer = ShootCooldown;
            }

            return true;
        }

        public bool TryReload()
        {
            if (IsInCooldown || IsReloading)
                return false;

            if (CurrentAmmo == MaxAmmo) 
                return false;

            _cooldownTimer = ReloadTime;
            Reload.Invoke(ReloadTime);
            return true;
        }

        private void Update()
        {
            UpdateTimer(ref _cooldownTimer, _cooldownCallback);
        }

        private void UpdateTimer(ref float value, Action callback)
        {
            if (value > 0f)
            {
                value -= Time.deltaTime;
                if (value <= 0f)
                {
                    _cooldownTimer = 0f;
                    callback?.Invoke();
                }
            }
        }

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