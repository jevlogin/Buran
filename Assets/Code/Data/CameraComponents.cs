using Cinemachine;


namespace WORLDGAMEDEVELOPMENT
{
    [System.Serializable ]
    internal sealed class CameraComponents
    {
        internal CameraView CameraView;
        internal CinemachineVirtualCamera VirtualCameraDefaulth;
        internal CinemachineVirtualCamera VirtualCameraIntro;
        internal CinemachineVirtualCamera VirtualCameraDeath;
        internal CinemachineVirtualCamera VirtualCameraDamage;
        internal CinemachineVirtualCamera ActiveCamera;
    }
}