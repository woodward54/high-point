using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// public enum Team
// {
//     PLAYER,
//     ENEMY
// }

public class UnitAuthoring : MonoBehaviour
{
    [SerializeField] UnitConfig _data;
    [SerializeField] Team _team;

    private class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            if (authoring._team == Team.Player)
            {
                AddComponent<PlayerTeamTag>(entity);
            }
            else
            {
                AddComponent<EnemyTeamTag>(entity);
            }

            // AddComponent(entity, new Health
            // {
            //     MaxHeath = authoring._data.Health,
            //     CurrentHealth = authoring._data.Health,
            //     HealthBarYOffset = authoring._data.HealthBarOffset
            // });
        }
    }
}