using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal struct CanvasStruct
    {
        [SerializeField] internal CanvasView CanvasView;
        internal CanvasView CanvasViewPixelSize;
        internal EventObjectModel EventObjectModel;
    }
}