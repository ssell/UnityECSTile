# UnityECSTile

Experimental project for learning the new Unity ECS framework as well as implementing instanced sprite components.

## References

### [EntityComponentSystemSamples](https://github.com/Unity-Technologies/EntityComponentSystemSamples)

Extensively referenced the documentation and samples provided in the repository.

### [ECS-Sandbox](https://github.com/Necromantic/ECS-Sandbox)

Referenced their implementation of a sprite renderer component and system. Their implementation is based on the official `MeshInstanceRenderer` family of objects which can be found under `Packages/com.unity.entities/Unity.Rendering.Hybrid`.

The `ECS/Sprite` shader also served as inspiration, but ultimately ended up using my own custom shader.

### [EntityCommandBuffer Strange Behaviour](https://forum.unity.com/threads/entitycommandbuffer-removecomponent-t-strange-behaviour.535424/)

Thread that enlightened me about the fact that `[BurstCompile]` generally can not be used on `IJob` implementations that make use of an `EntityCommandBuffer`. This is apparently a bug and it is expected that we can burst compile with the command buffer in future releases.

Was lead to that thread by [CommandBuffer.AddComponent throws IndexOutOfRangeException](https://forum.unity.com/threads/commandbuffer-addcomponent-throws-indexoutofrangeexception-from-unity-entities-typemanager-gettype.539359/).

### [gametorrahod](https://gametorrahod.com/unity-ecs-an-injected-group-is-implicitly-readonly-17996163dc32)

Lots of useful little snippets and gotchas. 

One in particular that helped was [An injected group is implicity `[ReadOnly]](https://gametorrahod.com/unity-ecs-an-injected-group-is-implicitly-readonly-17996163dc32), though admittedly in the end I did not end up using that solution.