namespace WORLDGAMEDEVELOPMENT
{
    internal class CameraInitialization
    {
        internal readonly CameraModel CameraModel;

        public CameraInitialization(CameraFactory cameraFactory)
        {
            CameraModel = cameraFactory.CreateCameraModel();
        }
    }
}