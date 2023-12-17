using Cinemachine;
using System.Collections.Generic;
using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    [System.Serializable ]
    public struct CameraStruct
    {
        public List<CinemachineVirtualCamera> VirtualCameras;
        public float DefaultBlendTime;
    }
}