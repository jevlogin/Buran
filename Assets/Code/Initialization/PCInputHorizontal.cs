using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class PCInputHorizontal : IUserInputProxy, ICleanup
    {
        private InputAction _moveLeftAndRightInput;
        private float _horizontal;

        public event Action<float> AxisOnChange;

        public PCInputHorizontal(InputAction moveLeftAndRight)
        {
            _moveLeftAndRightInput = moveLeftAndRight;
            _moveLeftAndRightInput.Enable();
            _moveLeftAndRightInput.performed += Performed;
            _moveLeftAndRightInput.canceled += Canceled;
        }

        private void Canceled(InputAction.CallbackContext context)
        {
            _horizontal = context.ReadValue<Vector2>().x;
        }

        private void Performed(InputAction.CallbackContext context)
        {
            _horizontal = context.ReadValue<Vector2>().x;
        }

        public void GetAxis()
        {
            AxisOnChange?.Invoke(_horizontal);
        }

        public void Cleanup()
        {
            _moveLeftAndRightInput.performed -= Performed;
            _moveLeftAndRightInput.canceled -= Canceled;
            _moveLeftAndRightInput.Disable();
        }
    }
}