using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityECSTile
{
    class TileHealthColorBarrier : BarrierSystem { }

    public class TileHealthColorSystem : JobComponentSystem
    {
        struct TileHealthColorData
        {
            [ReadOnly] public ComponentDataArray<HealthComponent> Health;
            [ReadOnly] public ComponentDataArray<TileComponent> Tile;
            [ReadOnly] public ComponentDataArray<ColorComponent> Color;
            [ReadOnly] public EntityArray Entities;
        }

        [Inject] TileHealthColorData _data;
        [Inject] TileHealthColorBarrier _barrier;

        struct TileHealthColorJob : IJob
        {
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<HealthComponent> Healths;
            [ReadOnly] public ComponentDataArray<ColorComponent> Colors;
            public EntityCommandBuffer Commands;

            public void Execute()
            {
                for(int i = 0; i < Entities.Length; ++i)
                {
                    float healthPercent = math.clamp((float)Healths[i].CurrentHealth / (float)Healths[i].MaxHealth, 0.0f, 1.0f);
                    Color color = Color.Lerp(Color.red, Color.white, healthPercent);

                    Commands.SetComponent(Entities[i], new ColorComponent(color));
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if(_data.Entities.Length == 0)
            {
                return inputDeps;
            }

            return new TileHealthColorJob
            {
                Entities = _data.Entities,
                Healths = _data.Health,
                Colors = _data.Color,
                Commands = _barrier.CreateCommandBuffer()
            }.Schedule(inputDeps);
        }
    }
}
