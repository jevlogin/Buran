using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal interface IUserInputTouch
    {
        event Action<Vector2> OnChangeTouchPositionStarted;
        event Action<bool> OnInputTouch;
        event Action<bool> OnInputUnTouch;
    }
}