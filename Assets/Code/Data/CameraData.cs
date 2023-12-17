using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [CreateAssetMenu(fileName = "CameraData", menuName = "CameraData/CameraData", order = 51)]
    internal sealed class CameraData : ScriptableObject
    {
        [SerializeField] private CameraStruct _cameraStruct;
        [SerializeField] private CameraComponents _cameraComponents;
        [SerializeField] private CameraSettings _cameraSettings;

        public CameraStruct CameraStruct => _cameraStruct;
        internal CameraComponents CameraComponents => _cameraComponents;
        internal CameraSettings CameraSettings => _cameraSettings;
    }
}