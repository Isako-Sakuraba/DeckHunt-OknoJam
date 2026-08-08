using Alchemy.Inspector;
using Game.Core;
using Game.Services;
using UnityEngine;

namespace Game.Player
{
    public class ShotgunController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private Shotgun _shotgun;
        [SerializeField, Required] private Camera _camera;

        [Header("Settings")]
        [SerializeField] LayerMask _hitLayers;
        [SerializeField] float _rayDistance = 50f;
        [SerializeField] float _sphereRadius = 0.2f;

        private IInputService _inputService;

        private void Awake()
        {
            if (_shotgun == null)
                _shotgun = GetComponent<Shotgun>();

            if (_camera == null)
                _camera = Camera.main;
        }

        private void Start()
        {
            _inputService = ServiceLocator.Get<IInputService>();
        }

        private void Update()
        {
            if (_shotgun.IsReloading || _shotgun.IsInCooldown)
                return;

            Ray ray = _camera.ScreenPointToRay(_inputService.MousePosition);
        }

        private void ShootWithSpherecast(float damage)
        {
            Ray ray = _camera.ScreenPointToRay(_inputService.MousePosition);
            RaycastHit hit;

            Debug.Log("Shot");

            if (Physics.SphereCast(ray, _sphereRadius, out hit, _rayDistance, _hitLayers))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(damage);
                    Debug.Log($"Hit {hit.collider.gameObject.name}");
                }
            }
        }
    }
}