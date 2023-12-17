using Cinemachine;
using System;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal sealed class CameraSettings
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private CinemachineVirtualCamera _virtualCameraDefaulth;
        [SerializeField] private CinemachineVirtualCamera _virtualCameraIntro;
        [SerializeField] private CinemachineVirtualCamera _virtualCameraDeath;
        [SerializeField] private CinemachineVirtualCamera _virtualCameraDamage;

        internal CameraView CameraView => cameraView;
        internal CinemachineVirtualCamera VirtualCameraDefaulth => _virtualCameraDefaulth;
        internal CinemachineVirtualCamera VirtualCameraIntro => _virtualCameraIntro;
        internal CinemachineVirtualCamera VirtualCameraDeath => _virtualCameraDeath;
        internal CinemachineVirtualCamera VirtualCameraDamage => _virtualCameraDamage;
    }
}