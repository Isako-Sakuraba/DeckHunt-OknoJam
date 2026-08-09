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

        private float _shootTimer;
        private float _reloadTimer;
        private Action _shootCallback;
        private Action _reloadCallback;

        public float Damage => _cachedValue.Damage * _cachedValue.DamageMultiplier;
        public int MaxAmmo => _cachedValue.Ammo;
        public int CurrentAmmo => _currentAmmo;
        public bool IsEmpty => _currentAmmo <= 0;

        public float ReloadTime => _cachedValue.ReloadTime * _cachedValue.ReloadTimeMultiplier;
        public bool IsReloading => _reloadTimer > 0f;

        public float ShootCooldown => _cachedValue.ShootCooldown * _cachedValue.ShootCooldownMultiplier;
        public bool IsInShootCooldown => _shootTimer > 0f;

        public float CooldownTimer => _reloadTimer + _shootTimer;
        public bool IsInCooldown => IsReloading || IsInShootCooldown;

        private Action<float> Shot = delegate { };
        private Action<float> Reload = delegate { };

        private ShotgunValues _cachedValue;
        private Stack<ShotgunValues> _modifierStack = new();

        private void Awake()
        {
            _shootCallback = ShootTimerCallback;
            _reloadCallback = ReloadTimerCallback;
        }

        private void Start()
        {
            RecalculateValues();
            _currentAmmo = MaxAmmo;
        }

        private void ShootTimerCallback()
        {
            if (IsEmpty)
                TryReload();
        }

        private void ReloadTimerCallback()
        {
            _currentAmmo = MaxAmmo;
        }

        public bool TryShoot()
        {
            if (IsInShootCooldown || IsReloading)
                return false;

            Shot.Invoke(Damage);

            _currentAmmo--;

            _shootTimer = ShootCooldown;
            
            return true;
        }

        public bool TryReload()
        {
            if (IsInShootCooldown || IsReloading)
                return false;

            if (CurrentAmmo == MaxAmmo) 
                return false;

            Debug.Log("Reloading!");
            _reloadTimer = ReloadTime;
            Reload.Invoke(ReloadTime);
            return true;
        }

        private void Update()
        {
            UpdateTimer(ref _shootTimer, _shootCallback);
            UpdateTimer(ref _reloadTimer, _reloadCallback);
        }

        private void UpdateTimer(ref float value, Action callback)
        {
            if (value > 0f)
            {
                value -= Time.deltaTime;
                if (value <= 0f)
                {
                    value = 0f;
                    callback?.Invoke();
                }
            }
        }

        private void RecalculateValues()
        {
            _cachedValue = _baseValues;

            foreach (ShotgunValues modifier in _modifierStack)
            {
                _cachedValue.Damage += modifier.Damage;
                _cachedValue.DamageMultiplier *= modifier.DamageMultiplier;
                _cachedValue.Ammo += modifier.Ammo;
                _cachedValue.ReloadTime += modifier.ReloadTime;
                _cachedValue.ReloadTimeMultiplier *= modifier.ReloadTimeMultiplier;
                _cachedValue.ShootCooldown += modifier.ShootCooldown;
                _cachedValue.ShootCooldownMultiplier *= modifier.ReloadTimeMultiplier;
            }

            _cachedValue = ClampValues(_cachedValue, _minValues);

            if (_currentAmmo > MaxAmmo)
                _currentAmmo = MaxAmmo;
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

        public void PushModifier(ShotgunValues values)
        {
            _modifierStack.Push(values);
            RecalculateValues();
        }

        public ShotgunValues PopModifier()
        {
            if (_modifierStack.Count == 0)
                return default;

            var popped = _modifierStack.Pop();
            RecalculateValues();
            return popped;
        }
    }
}