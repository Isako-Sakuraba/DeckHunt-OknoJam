using Alchemy.Inspector;
using Game.Core;
using Game.Services;
using UnityEngine;

namespace Game.Player
{
    public class ShotgunView : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private Shotgun _shotgun;
        [SerializeField, Required] private Camera _camera;

        [Header("Settings")]
        [SerializeField] private float _rotationLerp = 20f;
        [SerializeField] private Transform _target;

        private IInputService _inputService;

        private Quaternion _initialRotation;

        private void Awake()
        {
            if (_shotgun == null)
                _shotgun = GetComponent<Shotgun>();

            if (_camera == null)
                _camera = Camera.main;

            if (_target == null)
                _target = transform;

            _initialRotation = _target.rotation;
        }

        private void Start()
        {
            _inputService = ServiceLocator.Get<IInputService>();
        }

        private void Update()
        {
            Ray ray = _camera.ScreenPointToRay(_inputService.MousePosition);
            Quaternion desiredRotation = Quaternion.FromToRotation(Vector3.forward, ray.direction);

            if (_shotgun.IsReloading || _shotgun.IsInCooldown)
                desiredRotation = _initialRotation;
            
            _target.rotation = Quaternion.Lerp(_target.rotation, desiredRotation, _rotationLerp * Time.deltaTime);
        }
    }
}