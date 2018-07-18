using System;
using UnityEngine;
using Unity.Entities;

namespace UnityECSTile
{
    [Serializable]
    public struct ColorComponent : IComponentData
    {
        public Color Color;

        public ColorComponent(Color color)
        {
            Color = color;
        }
    }

    public class ColorComponentWrapper : ComponentDataWrapper<ColorComponent>
    {

    }
}
