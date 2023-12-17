using System;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    public sealed class EventObjectModel
    {
        public EventSystem _eventSystem;
        public InputSystemUIInputModule _inputSystem;
    }
}