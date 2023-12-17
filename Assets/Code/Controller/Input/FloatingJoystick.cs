using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class FloatingJoystick : MonoBehaviour
    {
        internal RectTransform RectTransform;
        internal RectTransform Knob;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            Knob = transform.GetChild(0).GetComponent<RectTransform>();
        }
    }
}