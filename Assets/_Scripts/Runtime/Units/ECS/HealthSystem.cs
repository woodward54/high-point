using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class HealthSystem : SystemBase
{
    public Action<int, int, Entity> OnHealthChanged;
    public Action<Entity> OnUnitDied;
    public Action<int, float3, Entity> OnUnitDamaged;

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // For each character with a damage component...
        foreach (var (hitPoints, damageToUnit, transform, entity) in
                 SystemAPI.Query<RefRW<Health>, DamageToUnit,
                     LocalTransform>().WithEntityAccess())
        {
            hitPoints.ValueRW.CurrentHealth -= damageToUnit.Value;

            OnUnitDamaged?.Invoke(damageToUnit.Value, transform.Position, entity);
            OnHealthChanged?.Invoke(hitPoints.ValueRO.CurrentHealth, hitPoints.ValueRO.MaxHeath, entity);

            ecb.RemoveComponent<DamageToUnit>(entity);

            // If the damaged character is out of health... 
            if (hitPoints.ValueRO.CurrentHealth <= 0)
            {
                OnUnitDied?.Invoke(entity);
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}