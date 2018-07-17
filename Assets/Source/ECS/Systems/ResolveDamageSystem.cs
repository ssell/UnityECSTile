using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

namespace UnityECSTile
{
    class ResolveDamageBarrier : BarrierSystem { }

    /**
     * System which resolves all damage components on entities.
     * Entities must have the following components to be worked on by this system:
     * 
     *     - DamageComponent
     *     - HealthComponent
     */
    public class ResolveDamageSystem : JobComponentSystem
    {
        public struct DamageData
        {
            [ReadOnly] public ComponentDataArray<DamageComponent> Damage;
            [ReadOnly] public ComponentDataArray<HealthComponent> Health;
            [ReadOnly] public EntityArray Entities;
        }

        [Inject] private DamageData _damageData;
        [Inject] private ResolveDamageBarrier _barrier;
        
        struct ResolveDamageJob : IJob
        {
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<DamageComponent> Damage;
            [ReadOnly] public ComponentDataArray<HealthComponent> Health;
            public EntityCommandBuffer Commands;

            public void Execute()
            {
                for(int i = 0; i < Entities.Length; ++i)
                {
                    HealthComponent health = new HealthComponent(Health[i].MaxHealth);
                    health.CurrentHealth = Health[i].CurrentHealth - Damage[i].Amount;

                    Commands.SetComponent(Entities[i], health);
                    Commands.RemoveComponent<DamageComponent>(Entities[i]);
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if(_damageData.Entities.Length == 0)
            {
                return inputDeps;
            }

            return new ResolveDamageJob
            {
                Entities = _damageData.Entities,
                Damage = _damageData.Damage,
                Health = _damageData.Health,
                Commands = _barrier.CreateCommandBuffer()
            }.Schedule(inputDeps);
        }
    }
}
