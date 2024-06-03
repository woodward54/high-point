// using System;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// using DotsTween;
// using DotsTween.Tweens;
// using DotsTween.Math;

// public partial class SpawnUnitSystem : SystemBase
// {
//     public Action<Entity> OnUnitSpawned;

//     public void SpawnUnit(Entity prefab, float3 position, quaternion rotation, float scale)
//     {
//         var adjustedScale = HexGrid.Instance.HexSize * scale;

//         var newUnit = EntityManager.Instantiate(prefab);

//         var unitTransform = LocalTransform.FromPositionRotationScale(position, rotation, adjustedScale);
//         EntityManager.SetComponentData(newUnit, unitTransform);

//         // float duration = 3.0f;
//         // Tween.Scale.FromTo(ref EntityManager, newUnit, 0f, 1f, duration, new TweenParams
//         // {
//         //     EaseType = EaseType.BounceOut,
//         // });

//         OnUnitSpawned?.Invoke(newUnit);
//     }

//     public void SpawnUnit(BuildingData type, float3 position, quaternion rotation, float scale)
//     {
//         var config = SystemAPI.GetSingleton<Config>();
//         Entity prefab;
//         switch (type.Type)
//         {
//             case BuildingCategory.WALL:
//                 prefab = config.WallPrefab;
//                 SpawnUnit(prefab, position, rotation, scale);
//                 break;

//             default:
//                 Debug.LogWarning(type.Type.ToString() + " not implemented.");
//                 break;
//         }
//     }

//     protected override void OnUpdate()
//     {
//         if (Input.GetKeyDown(KeyCode.W))
//         {
//             var worker = SystemAPI.GetSingleton<Config>().WorkerPrefab;
//             SpawnUnit(worker, float3.zero, quaternion.identity, 1f);
//         }
//     }
// }