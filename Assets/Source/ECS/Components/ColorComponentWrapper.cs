using System;
using UnityEngine;
using Unity.Entities;

namespace UnityECSTile
{
    [Serializable]
    public struct ColorComponent : IComponentData
    {
        public Color Color;
    }

    public class ColorComponentWrapper : ComponentDataWrapper<ColorComponent>
    {

    }
}
