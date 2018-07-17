using System;
using Unity.Entities;

namespace UnityECSTile
{ 
    [Serializable]
    public struct TileComponent : IComponentData
    {
        public int Type;
    }

    public class TileComponentWrapper : ComponentDataWrapper<TileComponent>
    {

    }
}