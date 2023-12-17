using System.Collections.Generic;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    internal class CameraFactory
    {
        #region Fields

        private CameraData _cameraData;
        private CameraModel _model;

        #endregion


        #region ClassLifeCycles

        public CameraFactory(CameraData cameraData)
        {
            _cameraData = cameraData;
        }

        #endregion


        #region Methods

        internal CameraModel CreateCameraModel()
        {
            if (_model == null)
            {
                var cameraStruct = _cameraData.CameraStruct;
                var components = new CameraComponents();
                var settings = _cameraData.CameraSettings;

                components.CameraView = Object.Instantiate(settings.CameraView);
                components.CameraView.name = settings.CameraView.name;

                components.VirtualCameraDefaulth = Object.Instantiate(settings.VirtualCameraDefaulth);
                components.VirtualCameraDefaulth.name = settings.VirtualCameraDefaulth.name;

                components.VirtualCameraIntro = Object.Instantiate(settings.VirtualCameraIntro);
                components.VirtualCameraIntro.name = settings.VirtualCameraIntro.name;

                components.VirtualCameraDeath = Object.Instantiate(settings.VirtualCameraDeath);
                components.VirtualCameraDeath.name = settings.VirtualCameraDeath.name;

                components.VirtualCameraDamage = Object.Instantiate(settings.VirtualCameraDamage);
                components.VirtualCameraDamage.name = settings.VirtualCameraDamage.name;

                cameraStruct.VirtualCameras = new List<Cinemachine.CinemachineVirtualCamera>
                {
                    components.VirtualCameraDefaulth, components.VirtualCameraIntro, components.VirtualCameraDeath, components.VirtualCameraDamage
                };

                foreach (var camera in cameraStruct.VirtualCameras)
                {
                    if (camera != components.VirtualCameraDefaulth)
                    {
                        camera.Priority = 0;
                    }
                }

                _model = new CameraModel(cameraStruct, components, settings);
            }
            return _model;
        }

        #endregion
    }
}