using System;
using Unity.Entities;

namespace UnityECSTile
{
    [Serializable]
    public struct HealthComponent : IComponentData
    {
        public int CurrentHealth;
        public int MaxHealth;

        public HealthComponent(int max)
        {
            CurrentHealth = max;
            MaxHealth = max;
        }
    }

    public class HealthComponentWrapper : ComponentDataWrapper<HealthComponent>
    {

    }
}