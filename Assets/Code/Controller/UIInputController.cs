﻿using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal class UIInputController : IInitialization, ICleanup
    {
        private readonly InputInitialization _inputInitialization;
        //private readonly IUserInputTouch _inputTouch;
        private readonly MobileInputTouch _inputTouch;
        private readonly CanvasModel _canvasModel;
        private readonly PanelInputView _panelInputView;
        private Vector2 JoystickSize;
        private FloatingJoystick Joystick;


        #region ClassLifeCycles

        public UIInputController(InputInitialization inputInitialization, CanvasModel canvasModel)
        {
            _inputInitialization = inputInitialization;
            _inputTouch = _inputInitialization.GetTouchMobile();

            _canvasModel = canvasModel;

            foreach (var panel in _canvasModel.CanvasStruct.CanvasViewPixelSize.panelViews)
            {
                if (panel is PanelInputView panelInputView)
                {
                    _panelInputView = panelInputView;
                    Joystick = _panelInputView.Joystick;
                }
            }

            JoystickSize = Joystick.gameObject.GetComponent<RectTransform>().sizeDelta;
        }

        #endregion

        public void Initialization()
        {
            _inputTouch.OnInputTouch += OnTouch;
            _inputTouch.OnInputUnTouch += OnUnTouch;
            _inputTouch.OnChangeTouchPositionStarted += OnChangeTouchPositionStarted;
            _inputTouch.OnChangeTouchPositionUpdate += OnChangeTouchPositionUpdate;
        }

        private void OnChangeTouchPositionUpdate(Vector2 position)
        {
            Debug.Log($"Позиция джойстика при Update - {position}");
            Joystick.gameObject.transform.position = ClampPosition(position);
        }

        private void OnChangeTouchPositionStarted(Vector2 position)
        {
            Debug.Log($"Позиция джойстика при старте - {position}");
            Joystick.gameObject.transform.position = ClampPosition(position);
        }

        private void OnUnTouch(bool value)
        {
            Debug.Log($"Выключаем джойстик.");
            if (Joystick != null)
            {
                Joystick.gameObject.SetActive(value); 
            }
        }

        private void OnTouch(bool value)
        {
            Debug.Log($"Включаем джойстик.");
            Joystick.gameObject.SetActive(value);
        }

        public void Cleanup()
        {
            _inputTouch.OnInputTouch -= OnTouch;
            _inputTouch.OnInputUnTouch -= OnUnTouch;
            _inputTouch.OnChangeTouchPositionStarted -= OnChangeTouchPositionStarted;
            _inputTouch.OnChangeTouchPositionUpdate -= OnChangeTouchPositionUpdate;
        }

        private Vector2 ClampPosition(Vector2 screenPosition)
        {
            if (screenPosition.x < JoystickSize.x / 2)
            {
                screenPosition.x = JoystickSize.x / 2;
            }
            else if (screenPosition.x > Screen.width - JoystickSize.x / 2)
            {
                screenPosition.x = Screen.width - JoystickSize.x / 2;
            }
            if (screenPosition.y < JoystickSize.y / 2)
            {
                screenPosition.y = JoystickSize.y / 2;
            }
            else if (screenPosition.y > Screen.height - JoystickSize.y / 2)
            {
                screenPosition.y = Screen.height - JoystickSize.y / 2;
            }
            return screenPosition;
        }
    }
}