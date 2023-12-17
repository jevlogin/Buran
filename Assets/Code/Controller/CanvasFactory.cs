using UnityEngine;
using UnityEngine.InputSystem.UI;


namespace WORLDGAMEDEVELOPMENT
{
    internal class CanvasFactory
    {
        private CanvasData _canvasData;

        public CanvasFactory(CanvasData canvasData)
        {
            _canvasData = canvasData;
        }

        internal CanvasModel CreateCanvasModel()
        {
            var canvasStruct = _canvasData.CanvasStruct;
            var canvasSettings = new CanvasSettings();

            canvasStruct.CanvasView = Object.Instantiate(_canvasData.CanvasSettings.CanvasView);
            canvasStruct.CanvasView.name = _canvasData.CanvasSettings.CanvasView.name;

            for (int i = 0; i < canvasStruct.CanvasView.transform.childCount; i++)
            {
                if (canvasStruct.CanvasView.transform.GetChild(i).TryGetComponent<PanelView>(out var panel))
                {
                    canvasStruct.CanvasView.panelViews.Add(panel);
                }
            }

            canvasStruct.CanvasViewPixelSize = Object.Instantiate(_canvasData.CanvasSettings.CanvasViewPixelSize);
            canvasStruct.CanvasViewPixelSize.name = _canvasData.CanvasSettings.CanvasView.name;

            for (int i = 0; i < canvasStruct.CanvasViewPixelSize.transform.childCount; i++)
            {
                if (canvasStruct.CanvasViewPixelSize.transform.GetChild(i).TryGetComponent<PanelView>(out var panel))
                {
                    canvasStruct.CanvasViewPixelSize.panelViews.Add(panel);
                }
            }

            var eventSystem = Object.Instantiate(_canvasData.CanvasSettings.EventSystem);
            eventSystem.name = _canvasData.CanvasSettings.EventSystem.name;

            canvasStruct.EventObjectModel = new EventObjectModel 
            {
               _eventSystem = eventSystem,
               _inputSystem = eventSystem.GetComponent<InputSystemUIInputModule>(),
            };

            var canvasModel = new CanvasModel(canvasStruct, canvasSettings);

            return canvasModel;
        }
    }
}