using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using UnityECSTile;

public sealed class GameBootstrap
{
    public static EntityArchetype TileArchetype;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        if(World.Active == null)
        {
            World.DisposeAllWorlds();
            World.Active = new World("ECSTestWorld");
        }

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        TileArchetype = entityManager.CreateArchetype(
            typeof(TileComponent),
            typeof(ColorComponent),
            typeof(HealthComponent),
            typeof(TransformMatrix));    
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeAfterSceneLoad()
    {

    }
}