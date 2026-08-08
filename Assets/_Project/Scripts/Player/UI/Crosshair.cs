using Game.Core;
using Game.Services;
using UnityEngine;
using Alchemy.Inspector;

namespace Game.Player
{
    public class Crosshair : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField] private RectTransform _target;

        private IInputService _inputService;

        private void Awake()
        {
            if (_target == null)
                _target = GetComponent<RectTransform>();
        }

        private void Start()
        {
            _inputService = ServiceLocator.Get<IInputService>();
        }

        private void Update()
        {
            var mousePosition = _inputService.MousePosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                mousePosition,
                _canvas.worldCamera,
                out Vector2 localPosition
            );

            _target.localPosition = localPosition;
        }
    }
}