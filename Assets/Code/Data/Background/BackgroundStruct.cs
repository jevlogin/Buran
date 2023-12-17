using System;
using System.Collections.Generic;


namespace WORLDGAMEDEVELOPMENT
{
    [Serializable]
    internal struct BackgroundStruct
    {
        internal Dictionary<SpaceLayerType, List<IBackgroundPool>> PoolsType;
    }
}