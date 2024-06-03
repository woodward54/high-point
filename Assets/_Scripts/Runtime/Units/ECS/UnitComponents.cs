using Unity.Entities;
using UnityEngine.UIElements;

public struct AttackStrength : IComponentData
{
    public int Value;
}

public struct Health : IComponentData
{
    public int CurrentHealth;
    public int MaxHeath;
    public float HealthBarYOffset;
}

public struct Movement : IComponentData
{
    public float Speed;
}

public struct DamageToUnit : IComponentData
{
    public int Value;
    public Entity Attacker;
}

public struct UnitPrefab : IComponentData
{
    public Entity Value;
}

public struct MoveToPosition : IComponentData
{
    public Position Target;
}

public struct EnemyTeamTag : IComponentData { }

public struct PlayerTeamTag : IComponentData { }