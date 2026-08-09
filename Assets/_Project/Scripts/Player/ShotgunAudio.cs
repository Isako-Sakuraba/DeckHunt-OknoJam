using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class ShotgunAudio : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Shotgun _shotgun;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _shootSource;
        [SerializeField] private AudioSource _cooldownSource;
        [SerializeField] private AudioSource _reloadSource;

        [Header("Clips")]
        [SerializeField] private AudioClip _shootClip;
        [SerializeField] private AudioClip _cooldownClip;
        [SerializeField] private AudioClip _reloadClip;

        [Header("Settings")]
        [SerializeField] private float _cooldownSoundDelay = 0.1f;

        private readonly Dictionary<float, WaitForSeconds> _waitCache = new();

        private Coroutine _cooldownCoroutine;
        private Coroutine _reloadCoroutine;

        private void OnEnable()
        {
            if (_shotgun == null)
                return;

            _shotgun.Shot += OnShot;
            _shotgun.Reload += OnReload;
        }

        private void OnDisable()
        {
            if (_shotgun == null)
                return;

            _shotgun.Shot -= OnShot;
            _shotgun.Reload -= OnReload;

            StopAudio();
        }

        private void OnShot(float damage)
        {
            PlayShootSound();

            if (_cooldownCoroutine != null)
                StopCoroutine(_cooldownCoroutine);

            _cooldownCoroutine = StartCoroutine(PlayCooldownSound());
        }

        private void PlayShootSound()
        {
            if (_shootClip == null)
                return;

            _shootSource.pitch = 1f;
            _shootSource.PlayOneShot(_shootClip);
        }

        private IEnumerator PlayCooldownSound()
        {
            if (_cooldownClip == null)
                yield break;

            float cooldownTime = _shotgun.ShootCooldown;
            float playbackTime = cooldownTime - _cooldownSoundDelay;

            if (playbackTime <= 0f)
                yield break;

            yield return GetWait(_cooldownSoundDelay);

            // Make the cooldown sound finish exactly when the shotgun
            // cooldown finishes.
            float pitch = _cooldownClip.length / playbackTime;

            _cooldownSource.pitch = pitch;
            _cooldownSource.PlayOneShot(_cooldownClip);

            yield return GetWait(playbackTime);

            _cooldownSource.pitch = 1f;
            _cooldownCoroutine = null;
        }

        private void OnReload(float reloadTime)
        {
            if (_reloadCoroutine != null)
                StopCoroutine(_reloadCoroutine);

            _reloadCoroutine = StartCoroutine(PlayReloadSounds(reloadTime));
        }

        private IEnumerator PlayReloadSounds(float reloadTime)
        {
            int ammoCount = _shotgun.MaxAmmo;

            if (_reloadClip == null || ammoCount <= 0 || reloadTime <= 0f)
                yield break;

            float interval = reloadTime / ammoCount;

            // Make each loading sound fit exactly into one ammo interval.
            float pitch = _reloadClip.length / interval;

            _reloadSource.clip = _reloadClip;
            _reloadSource.pitch = pitch;

            WaitForSeconds wait = GetWait(interval);

            for (int i = 0; i < ammoCount; i++)
            {
                _reloadSource.Play();
                yield return wait;
            }

            _reloadSource.Stop();
            _reloadSource.pitch = 1f;

            _reloadCoroutine = null;
        }

        private WaitForSeconds GetWait(float duration)
        {
            // Quantize to milliseconds to avoid floating-point variations
            // creating unnecessary cache entries.
            duration = Mathf.Round(duration * 1000f) / 1000f;

            if (!_waitCache.TryGetValue(duration, out WaitForSeconds wait))
            {
                wait = new WaitForSeconds(duration);
                _waitCache.Add(duration, wait);
            }

            return wait;
        }

        private void StopAudio()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }

            if (_reloadCoroutine != null)
            {
                StopCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
            }

            if (_shootSource != null)
            {
                _shootSource.Stop();
                _shootSource.pitch = 1f;
            }

            if (_cooldownSource != null)
            {
                _cooldownSource.Stop();
                _cooldownSource.pitch = 1f;
            }

            if (_reloadSource != null)
            {
                _reloadSource.Stop();
                _reloadSource.pitch = 1f;
            }
        }
    }
}