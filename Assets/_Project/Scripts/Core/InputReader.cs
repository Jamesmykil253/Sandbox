// InputReader.cs (v1.1 - No changes from v1.0)
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Platformer
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/Input Reader")]
    public class InputReader : ScriptableObject, InputSystem_Actions.IPlayerActions
    {
        private InputSystem_Actions _inputActions;

        public event Action<Vector2> MoveEvent;
        public event Action JumpEvent;
        public event Action JumpCancelledEvent;
        public event Action CycleCameraEvent;
        public event Action AttackEvent;
        public event Action ScoreEvent;
        public event Action ScoreCancelledEvent; // **THE FIX**: This was missing.

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputSystem_Actions();
                _inputActions.Player.SetCallbacks(this);
            }
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed) JumpEvent?.Invoke();
            if (context.canceled) JumpCancelledEvent?.Invoke();
        }

        public void OnCycleCamera(InputAction.CallbackContext context)
        {
            if (context.performed) CycleCameraEvent?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed) AttackEvent?.Invoke();
        }

        // **THE FIX**: This method now correctly handles both events.
        public void OnScore(InputAction.CallbackContext context)
        {
            if (context.performed) ScoreEvent?.Invoke();
            if (context.canceled) ScoreCancelledEvent?.Invoke();
        }
    }
}