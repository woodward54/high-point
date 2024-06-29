using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// Singleton Class
public class ConfigAuthoring : MonoBehaviour
{
    [Header("Building Prefabs")]
    [SerializeField] BuildingConfig Wall;

    [Header("Agent Prefabs")]
    [SerializeField] AgentConfig Worker;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);

            AddComponent(entity, new Config
            {
                WallPrefab = GetEntity(authoring.Wall.Prefab, TransformUsageFlags.Dynamic),
                WorkerPrefab = GetEntity(authoring.Worker.Prefab, TransformUsageFlags.Dynamic)
            });

            // var config = SystemAPI.GetSingleton<Config>();

            // var configManaged = new ConfigManaged();
            // configManaged.BotAnimatedPrefabGO = authoring.BotAnimatedPrefabGO;
            // AddComponentObject(entity, configManaged);
        }
    }
}

public struct Config : IComponentData
{
    // Buildings
    public Entity WallPrefab;

    // Agents
    public Entity WorkerPrefab;
}

// public class ConfigManaged : IComponentData
// {
//     public GameObject BotAnimatedPrefabGO;
//     public UIController UIController;
// }
