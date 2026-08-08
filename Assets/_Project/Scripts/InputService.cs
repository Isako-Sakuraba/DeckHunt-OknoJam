using UnityEngine;
using UnityEngine.InputSystem;

using DefaultInputActions = Game.Generated.DefaultInputActions;

namespace Game.Core
{
    public class InputService : MonoBehaviour, IInputService
    {
        private DefaultInputActions _actions;

        public ButtonState Shoot => GetButtonState(_actions.Gameplay.Shoot);
        public ButtonState Reload => GetButtonState(_actions.Gameplay.Reload);
        public ButtonState Cancel => GetButtonState(_actions.Gameplay.Cancel);

        public Vector2 MousePosition => GetValue<Vector2>(_actions.Gameplay.MousePosition);
        public Vector2 MouseDelta => GetValue<Vector2>(_actions.Gameplay.MouseDelta);

        private void Awake()
        {
            _actions = new DefaultInputActions();
            _actions.Gameplay.Enable();
        }

        public ButtonState GetButtonState(InputAction action)
        {
            return new ButtonState(
                action.WasPressedThisFrame(),
                action.IsPressed(),
                action.WasReleasedThisFrame());
        }

        public T GetValue<T>(InputAction action) where T : struct
        {
            return action.ReadValue<T>();
        }
    }
}