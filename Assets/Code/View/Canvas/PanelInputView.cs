using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class PanelInputView : PanelView
    {
        [SerializeField] private FloatingJoystick joystick;

        internal FloatingJoystick Joystick => joystick;

        private void Awake()
        {
            joystick = transform.GetComponentInChildren<FloatingJoystick>(true);
        }
    }
}