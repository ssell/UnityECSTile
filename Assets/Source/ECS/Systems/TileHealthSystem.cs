using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;

namespace UnityECSTile
{
    class TileHealthBarrier : BarrierSystem { }

    public class TileHealthSystem : JobComponentSystem
    {
        public struct TileHealthData
        {
            [ReadOnly] public ComponentDataArray<HealthComponent> Health;
            [ReadOnly] public ComponentDataArray<TileComponent> Tiles;
            [ReadOnly] public EntityArray Entities;
        }

        [Inject] private TileHealthData _healthData;
        [Inject] private TileHealthBarrier _barrier;

        [BurstCompile]
        struct TileHealthJob : IJob
        {
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<HealthComponent> Health;
            public EntityCommandBuffer Commands;

            public void Execute()
            {
                for(int i = 0; i < Entities.Length; ++i)
                {
                    var entity = Entities[i];
                    var health = Health[i];

                    if(health.CurrentHealth <= 0)
                    {
                        Commands.DestroyEntity(entity);
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if(_healthData.Entities.Length == 0)
            {
                return inputDeps;
            }

            return new TileHealthJob
            {
                Entities = _healthData.Entities,
                Health = _healthData.Health,
                Commands = _barrier.CreateCommandBuffer()
            }.Schedule(inputDeps);
        }
    }
}
