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
        [SerializeField] private Transform _target;
        [SerializeField] private float _rotationLerp = 20f;
        [SerializeField] private float _rotationStep = 12.5f;   

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
            //Ray ray = _camera.ScreenPointToRay(_inputService.MousePosition);
            //Quaternion desiredRotation = Quaternion.FromToRotation(Vector3.forward, ray.direction);

            //if (_shotgun.IsReloading || _shotgun.IsInCooldown)
            //    desiredRotation = _initialRotation;

            //_target.rotation = Quaternion.Lerp(_target.rotation, desiredRotation, _rotationLerp * Time.deltaTime);

            Ray ray = _camera.ScreenPointToRay(_inputService.MousePosition);

            Quaternion desiredRotation;

            if (_shotgun.IsReloading || _shotgun.IsInCooldown)
            {
                desiredRotation = _initialRotation;
            }
            else
            {
                desiredRotation = GetQuantizedRotation(ray.direction);
            }

            _target.rotation = desiredRotation;
        }

        private Quaternion GetQuantizedRotation(Vector3 direction)
        {
            Vector3 localDirection = _target.parent
                ? _target.parent.InverseTransformDirection(direction)
                : direction;

            float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

            angle = Mathf.Round(angle / _rotationStep) * _rotationStep;

            return Quaternion.Euler(transform.eulerAngles.x, angle, 0f);
        }
    }
}