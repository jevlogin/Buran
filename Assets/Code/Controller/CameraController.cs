using UnityEngine;
using Cinemachine;
using System;

namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class CameraController : IAddedModel, ICleanup, IExecute
    {
        private readonly Camera _camera;
        private CinemachineVirtualCamera _activeCamera;
        private readonly CinemachineBrain _cinemaMashineBrain;
        private readonly Timer _timer;
        private readonly Transform _playerTransform;
        private readonly PlayerController _playerController;
        private readonly SceneController _sceneController;
        private readonly CameraModel _cameraModel;
        private CameraView _cameraView;
        private bool _stopControl;
        private bool _cameraAligned;
        private bool _cameraMoveOff;

        public CameraController(CameraModel cameraModel, Transform playerTransform, SceneController sceneController, PlayerController playerController)
        {
            _cameraModel = cameraModel;
            _cameraView = cameraModel.Components.CameraView;
            _camera = _cameraView.Camera;
            _activeCamera = cameraModel.Components.VirtualCameraDefaulth;
            _cinemaMashineBrain = _camera.GetComponent<CinemachineBrain>();
            _timer = new Timer();
            _timer.OnTimerLeftCountDown += OnTimerLeftCountDown;

            _playerTransform = playerTransform;
            _playerController = playerController;
            _playerController.MoveController.TheShipTookOff += TheShipTookOff;
            _playerController.PlayerInitialization.PlayerModel.PlayerStruct.Player.IsDeadPlayer += IsDeadPlayer;
            _playerController.PlayerInitialization.PlayerModel.PlayerStruct.Player.IsDamageReceived += IsDamageReceived;
            _playerController.IsAlivePlayer += IsAlivePlayer;

            foreach (var camera in cameraModel.CamStruct.VirtualCameras)
            {
                camera.Follow = _playerTransform;
                camera.LookAt = _playerTransform;
            }
            
            _sceneController = sceneController;
            _sceneController.IsStopControl += OnStopControl;
            _sceneController.TakeOffOfTheShip += OnChangeTakeOffOfTheShip;
        }

        private void OnTimerLeftCountDown()
        {
            SwitchVirtualCamera(_cameraModel.Components.VirtualCameraDefaulth);
        }

        private void IsDamageReceived()
        {
            SwitchVirtualCamera(_cameraModel.Components.VirtualCameraDamage);
            _timer.StartTimer(2);
        }

        private void IsAlivePlayer()
        {
            SwitchVirtualCamera(_cameraModel.Components.VirtualCameraDefaulth);
        }

        private void IsDeadPlayer()
        {
            _cameraMoveOff = false;
            SwitchVirtualCamera(_cameraModel.Components.VirtualCameraDeath);
        }

        private void TheShipTookOff(bool value)
        {
            _cameraMoveOff = value;
            SwitchVirtualCamera(_cameraModel.Components.VirtualCameraDefaulth);
        }


        private void OnChangeTakeOffOfTheShip(bool value)
        {
            _stopControl = false;
            SwitchVirtualCamera(_cameraModel.Components.VirtualCameraIntro);
        }

        private void SwitchVirtualCamera(CinemachineVirtualCamera camera)
        {
            camera.Priority = 10;
            _activeCamera = camera;

            foreach (var cam in _cameraModel.CamStruct.VirtualCameras)
            {
                if (cam != camera && cam.Priority != 0)
                {
                    cam.Priority = 0;
                }
            }
        }

        private void OnStopControl(bool value)
        {
            _stopControl = value;
        }

        public void Cleanup()
        {
            _sceneController.IsStopControl -= OnStopControl;
            _sceneController.TakeOffOfTheShip -= OnChangeTakeOffOfTheShip;
            _playerController.MoveController.TheShipTookOff -= TheShipTookOff;
            _playerController.PlayerInitialization.PlayerModel.PlayerStruct.Player.IsDeadPlayer -= IsDeadPlayer;
            _playerController.IsAlivePlayer -= IsAlivePlayer;
            _playerController.PlayerInitialization.PlayerModel.PlayerStruct.Player.IsDamageReceived -= IsDamageReceived;
            _timer.OnTimerLeftCountDown -= OnTimerLeftCountDown;
        }

        public void Execute(float deltatime)
        {
            _timer.Execute(deltatime);
        }
    }
}