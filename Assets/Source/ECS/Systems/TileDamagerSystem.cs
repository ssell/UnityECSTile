using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;

namespace UnityECSTile
{
    public class TileDamagerBarrier : BarrierSystem { }
    /**
     * Deals damage to all tiles every second.
     */
    public class TileDamagerSystem : JobComponentSystem
    {
        public struct TileData
        {
            public readonly int Length;
            public EntityArray Entities;
            public ComponentDataArray<TileComponent> Tiles;
            public SubtractiveComponent<DamageComponent> Damage;
        }

        [Inject] private TileData _tileData;
        [Inject] private TileDamagerBarrier _barrier;

        private float _lastTime = 0.0f;
        private float _damageRate = 1.0f;
        
        // There is apparently a bug with [BurstCompile] and most functionality
        // of the EntityCommandBuffer, so we must disable it for now.
        //[BurstCompile]
        struct TileDamagerJob : IJob
        {
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public int DamageCount;
            public EntityCommandBuffer Commands;

            public void Execute()
            {
                for (int i = 0; i < Entities.Length; ++i)
                {
                    var damage = new DamageComponent();
                    damage.Amount = DamageCount;

                    Commands.AddComponent(Entities[i], damage);
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float currTime = Time.time;
            float delta = currTime - _lastTime;

            if(delta < _damageRate)
            {
                return inputDeps;
            }

            _lastTime = currTime;
            int damageCount = (int)(delta / _damageRate);

            return new TileDamagerJob
            {
                Entities = _tileData.Entities,
                DamageCount = (int)(delta / _damageRate),
                Commands = _barrier.CreateCommandBuffer()
            }.Schedule(inputDeps);
        }
    }
}
