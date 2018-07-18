using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using UnityECSTile;

public sealed class GameBootstrap
{
    public static EntityArchetype TileArchetype;
    public static SpriteInstanceRendererComponent TileSpritePrototype;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
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
        TileSpritePrototype = GetComponentFromPrototype<SpriteInstanceRendererWrapper>("TilePrototype", true).Value;
    }

    private static TWrapper GetComponentFromPrototype<TWrapper>(string name, bool destroy = false)
    {
        var prototype = GameObject.Find(name);
        var result = prototype.GetComponent<TWrapper>();

        if (destroy)
        {
            Object.Destroy(prototype);
        }

        return result;
    }
}