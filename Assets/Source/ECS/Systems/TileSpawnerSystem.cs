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

            Vector3 randLoc = RandomLocation();
            TransformMatrix transform = new TransformMatrix();
            transform.Value = new Unity.Mathematics.float4x4(1.0f, 0.0f, 0.0f, randLoc.x,
                                                             0.0f, 1.0f, 0.0f, randLoc.y,
                                                             0.0f, 0.0f, 1.0f, randLoc.z,
                                                             0.0f, 0.0f, 0.0f, 1.0f);

            PostUpdateCommands.CreateEntity(GameBootstrap.TileArchetype);
            PostUpdateCommands.SetComponent(transform);
            PostUpdateCommands.SetComponent(new HealthComponent(10));
            PostUpdateCommands.SetComponent(new ColorComponent(Color.white));
            PostUpdateCommands.AddSharedComponent(GameBootstrap.TileSpritePrototype);
        }

        private static Vector3 RandomLocation()
        {
            var randVec = new Vector3(Random.value, Random.value, 1.0f);

            var cameraBounds = CameraUtils.OrthographicBounds(Camera.main);
            var cameraOrigin = new Vector3(cameraBounds.center.x - cameraBounds.extents.x * 0.5f,
                                           cameraBounds.center.y - cameraBounds.extents.y * 0.5f,
                                           cameraBounds.center.z);

            return new Vector3(
                cameraOrigin.x + (cameraBounds.extents.x * randVec.x),
                cameraOrigin.y + (cameraBounds.extents.y * randVec.y),
                0.0f);
        }
    }
}
