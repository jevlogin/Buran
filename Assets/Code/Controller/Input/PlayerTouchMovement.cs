using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class PlayerTouchMovement : MonoBehaviour
    {
        [SerializeField] private Vector2 JoystickSize = new Vector2(300.0f, 300.0f);
        [SerializeField] private FloatingJoystick Joystick;

        private Finger MovementFinger;
        private Vector2 MovementAmount;

        private void OnEnable()
        {
            Debug.Log($"События работают");

            EnhancedTouchSupport.Enable();
            ETouch.Touch.onFingerDown += Touch_onFingerDown;
            ETouch.Touch.onFingerMove += OnFingerMove;
            ETouch.Touch.onFingerUp += OnFingerUp;
        }


        private void OnDisable()
        {
            ETouch.Touch.onFingerDown -= Touch_onFingerDown;
            ETouch.Touch.onFingerMove -= OnFingerMove;
            ETouch.Touch.onFingerUp -= OnFingerUp;
            EnhancedTouchSupport.Disable();
        }

        private void OnFingerUp(Finger upFinger)
        {
            if (upFinger == MovementFinger)
            {
                MovementFinger = null;
                Joystick.Knob.anchoredPosition = Vector2.zero;
                Joystick.gameObject.SetActive(false);
                MovementAmount = Vector2.zero;
            }
        }

        private void OnFingerMove(Finger movedFinger)
        {
            if (movedFinger == MovementFinger)
            {
                Vector2 knobPosition;
                float maxMovement = JoystickSize.x / 2.0f;
                ETouch.Touch currentTouch = movedFinger.currentTouch;

                if (Vector2.Distance(currentTouch.screenPosition, Joystick.RectTransform.anchoredPosition) > maxMovement)
                {
                    knobPosition = (currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition).normalized * maxMovement;

                }
                else
                {
                    knobPosition = currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;
                }

                Joystick.Knob.anchoredPosition = knobPosition;
                MovementAmount = knobPosition / maxMovement;
            }
        }

        private void Touch_onFingerDown(Finger downFinger)
        {
            if (MovementFinger == null && downFinger.screenPosition.x <= Screen.width / 2.0f)
            {
                MovementFinger = downFinger;
                MovementAmount = Vector2.zero;
                Joystick.gameObject.SetActive(true);
                Joystick.RectTransform.sizeDelta = JoystickSize;
                Joystick.RectTransform.anchoredPosition = ClampPosition(downFinger.screenPosition);
            }
        }

        private Vector2 ClampPosition(Vector2 screenPosition)
        {
            if (screenPosition.x < JoystickSize.x / 2)
            {
                screenPosition.x = JoystickSize.x / 2;
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