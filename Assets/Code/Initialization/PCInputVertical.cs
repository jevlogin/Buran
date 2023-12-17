using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class PCInputVertical : IUserInputProxy, ICleanup
    {
        private InputAction _inputActions;
        private float _vertical;
        public event Action<float> AxisOnChange;

        public PCInputVertical(InputAction moveLeftAndRight)
        {
            _inputActions = moveLeftAndRight;
            _inputActions.Enable();
            _inputActions.performed += Performed;
            _inputActions.canceled += Canceled;
        }

        private void Canceled(InputAction.CallbackContext context)
        {
            _vertical = context.ReadValue<Vector2>().y;
        }

        private void Performed(InputAction.CallbackContext context)
        {
            _vertical = context.ReadValue<Vector2>().y;
        }

        public void GetAxis()
        {
            AxisOnChange?.Invoke(_vertical);
        }

        public void Cleanup()
        {
            _inputActions.performed -= Performed;
            _inputActions.canceled -= Canceled;
            _inputActions.Disable();
        }
    }
}