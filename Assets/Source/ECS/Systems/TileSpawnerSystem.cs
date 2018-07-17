using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;

namespace UnityECSTile
{
    public class TileSpawnerSystem : ComponentSystem
    {
        private float _lastSpawnTime = 0.0f;
        private float _spawnRate = 0.1f;

        protected override void OnUpdate()
        {
            float currTime = Time.time;
            float delta = currTime - _lastSpawnTime;

            if(delta < _spawnRate)
            {
                return;
            }

            _lastSpawnTime = currTime;

            TransformMatrix transform = new TransformMatrix();
            transform.Value = new Unity.Mathematics.float4x4(1.0f, 0.0f, 0.0f, 0.0f,
                                                             0.0f, 1.0f, 0.0f, 0.0f,
                                                             0.0f, 0.0f, 1.0f, 0.0f,
                                                             0.0f, 0.0f, 0.0f, 1.0f);

            PostUpdateCommands.CreateEntity(GameBootstrap.TileArchetype);
            PostUpdateCommands.SetComponent(transform);
            PostUpdateCommands.SetComponent(new HealthComponent(10));
            // Add Color and Sprite ...
        }
    }
}
