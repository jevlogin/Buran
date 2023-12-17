using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class MobileInputTouch : IUserInputTouch, ICleanup
    {
        private readonly Camera _camera;
        private readonly InputAction _onTouch;
        private readonly InputAction _primaryPosition;
        private bool _isPressed = false;
        private bool _isSendingPosition;

        public event Action<Vector2> OnChangeTouchPositionStarted;
        public event Action<Vector2> OnChangeTouchPositionUpdate;
        public event Action<bool> OnInputTouch;
        public event Action<bool> OnInputUnTouch;

        public MobileInputTouch(InputAction primaryContact, InputAction primaryPosition, Camera camera)
        {
            _camera = camera;
            _onTouch = primaryContact;
            _primaryPosition = primaryPosition;
            _onTouch.Enable();
            _primaryPosition.Enable();

            _onTouch.started += OnTouchStart;
            _onTouch.canceled += OnTouchStartCancel;

            _primaryPosition.performed += OnPerformedPrimaryPosition;
        }

        private void OnPerformedPrimaryPosition(InputAction.CallbackContext context)
        {
            var position = context.ReadValue<Vector2>();
            var currentPosition = ScreenToWorldPosition(position);

            if (!_isSendingPosition)
            {
                OnChangeTouchPositionStarted?.Invoke(currentPosition);
                _isSendingPosition = true;
            }
            else
            {
                OnChangeTouchPositionUpdate?.Invoke(currentPosition);
            }
        }

        private void OnTouchStartCancel(InputAction.CallbackContext context)
        {
            Debug.Log($"Отпустили кнопку");
            OnInputUnTouch?.Invoke(false);
        }

        private void OnTouchStart(InputAction.CallbackContext context)
        {
            _isSendingPosition = false;
            Debug.Log($"Нажали на кнопку");
            OnInputTouch?.Invoke(true);
        }

        public void Cleanup()
        {
            _onTouch.started -= OnTouchStart;
            _onTouch.canceled -= OnTouchStartCancel;

            _primaryPosition.performed -= OnPerformedPrimaryPosition;

            _onTouch.Disable();
            _primaryPosition.Disable();
        }

        private Vector3 ScreenToWorldPosition(Vector3 position)
        {
            position.z = _camera.nearClipPlane;
            return _camera.ScreenToWorldPoint(position);
        }
    }
}