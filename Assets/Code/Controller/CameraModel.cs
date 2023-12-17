namespace WORLDGAMEDEVELOPMENT
{
    internal sealed class CameraModel
    {
        #region Fields

        public CameraStruct CamStruct;
        public CameraComponents Components;
        public CameraSettings Settings;

        #endregion


        #region ClassLifeCycles

        public CameraModel(CameraStruct cameraStruct, CameraComponents components, CameraSettings settings)
        {
            CamStruct = cameraStruct;
            Components = components;
            Settings = settings;
        }

        #endregion
    }
}