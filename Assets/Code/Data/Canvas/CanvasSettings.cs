using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal sealed class CanvasSettings
    {
        [SerializeField] private CanvasView _canvasView;
        [SerializeField] private CanvasView _canvasViewPixelSize;
        [SerializeField] private EventSystem _eventSystem;


        internal EventSystem EventSystem => _eventSystem; 
        internal CanvasView CanvasView => _canvasView;
        internal CanvasView CanvasViewPixelSize => _canvasViewPixelSize;
    }
}