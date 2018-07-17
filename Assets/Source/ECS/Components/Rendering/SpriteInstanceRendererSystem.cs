using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.PlayerLoop;

namespace UnityECSTile
{
    [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
    [ExecuteInEditMode]
    public class SpriteInstanceRendererSystem : ComponentSystem
    {
        private readonly Dictionary<SpriteInstanceRendererComponent, Material> materialCache = new Dictionary<SpriteInstanceRendererComponent, Material>();
        private readonly Dictionary<SpriteInstanceRendererComponent, Mesh> meshCache = new Dictionary<SpriteInstanceRendererComponent, Mesh>();

        private readonly Matrix4x4[] matrices = new Matrix4x4[1023];
        private readonly Vector4[] colors = new Vector4[1023];
        private readonly List<SpriteInstanceRendererComponent> cachedUniqueRenderers = new List<SpriteInstanceRendererComponent>(16);
        private ComponentGroup instanceRendererGroup;

        protected override void OnCreateManager(int capacity)
        {
            instanceRendererGroup = GetComponentGroup(typeof(SpriteInstanceRendererComponent), typeof(TransformMatrix), typeof(ColorComponent));
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponentDatas(cachedUniqueRenderers);

            for(int i = 0; i < cachedUniqueRenderers.Count; ++i)
            {
                var renderer = cachedUniqueRenderers[i];

                if (renderer.Sprite == null)
                    continue;

                instanceRendererGroup.SetFilter(renderer);
                var transformComponents = instanceRendererGroup.GetComponentDataArray<TransformMatrix>();
                var colorComponents = instanceRendererGroup.GetComponentDataArray<ColorComponent>();

                Mesh mesh;
                Material material;
                Sprite sprite = renderer.Sprite;

                if(!meshCache.TryGetValue(renderer, out mesh))
                {
                    float2 meshSize = new float2((sprite.rect.width / sprite.pixelsPerUnit), (sprite.rect.height / sprite.pixelsPerUnit));
                    float2 meshPivot = new float2((sprite.pivot.x / sprite.rect.width * meshSize.x), (sprite.pivot.y / sprite.rect.height * meshSize.y));

                    mesh = CreateQuad(meshSize, meshPivot);
                    meshCache.Add(renderer, mesh);
                }

                if(!materialCache.TryGetValue(renderer, out material))
                {
                    material = new Material(Shader.Find("ECS/Sprite"))
                    {
                        enableInstancing = true,
                        mainTexture = sprite.texture
                    };

                    float4 rect = new float4((sprite.textureRect.x / sprite.texture.width),
                                             (sprite.textureRect.y / sprite.texture.height),
                                             (sprite.textureRect.width / sprite.texture.width),
                                             (sprite.textureRect.height / sprite.texture.height));

                    material.SetVector("_Rect", rect);
                    materialCache.Add(renderer, material);
                }

                int index = 0;

                while(index < transformComponents.Length)
                {
                    int length = math.min(matrices.Length, transformComponents.Length - index);

                    Unity.Rendering.MeshInstanceRendererSystem.CopyMatrices(transformComponents, index, length, matrices);
                    CopyColors(colorComponents, index, length, colors);

                    MaterialPropertyBlock properties = new MaterialPropertyBlock();
                    properties.SetVectorArray("_Color", colors);

                    Graphics.DrawMeshInstanced(mesh, 0, material, matrices, length, properties);
                    index += length;
                }
            }

            cachedUniqueRenderers.Clear();
        }

        private Mesh CreateQuad(float2 size, float2 pivot)
        {
            return new Mesh
            {
                vertices = new Vector3[4]
                {
                    new Vector3(0.0f, 0.0f, 0.0f),
                    new Vector3(1.0f, 0.0f, 0.0f),
                    new Vector3(1.0f, 1.0f, 0.0f),
                    new Vector3(0.0f, 1.0f, 0.0f)
                },

                uv = new Vector2[4]
                {
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.0f, 1.0f)
                },

                triangles = new int[6]
                {
                    0, 1, 2,
                    2, 3, 0
                }
            };
        }

        /**
         * Based on MeshInstanceRendererSystem.CopyMatrices
         */
        public unsafe static void CopyColors(ComponentDataArray<ColorComponent> colors, int beginIndex, int length, Vector4[] outColors)
        {
            fixed(Vector4* colorsPtr = outColors)
            {
                Assert.AreEqual(sizeof(ColorComponent), sizeof(Vector4));
                var colorsSlice = NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<ColorComponent>(colorsPtr, sizeof(Vector4), length);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeSliceUnsafeUtility.SetAtomicSafetyHandle(ref colorsSlice, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
#endif

                colors.CopyTo(colorsSlice, beginIndex);
            }
        }
    }
}
