using System;
using Unity.Entities;
using UnityEngine;

namespace UnityECSTile
{
    [Serializable]
    public struct SpriteInstanceRendererComponent : ISharedComponentData
    {
        public Sprite Sprite;
    }

    public class SpriteInstanceRendererWrapper : SharedComponentDataWrapper<SpriteInstanceRendererComponent>
    {

    }
}
