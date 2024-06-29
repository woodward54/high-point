using Pathfinding;
using Pathfinding.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial struct WalkToNodeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DestinationPoint>();
        state.RequireForUpdate<Movement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (movement, destPoint) in
                 SystemAPI.Query<RefRO<Movement>, RefRW<DestinationPoint>>())
        {
            destPoint.ValueRW.destination = new float3(3f, 0f, 3f);
        }
    }
}