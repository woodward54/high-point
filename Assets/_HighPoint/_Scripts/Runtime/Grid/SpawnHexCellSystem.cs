// using System;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// using DotsTween;
// using DotsTween.Tweens;
// using DotsTween.Math;

// public partial class SpawnHexCellSystem : SystemBase
// {
//     public void CreateCell()
//     {

//     }

//     public void CreateTerrain(Entity prefab, float3 position, quaternion rotation, float scale)
//     {
//         var newUnit = EntityManager.Instantiate(prefab);

//         var unitTransform = LocalTransform.FromPositionRotationScale(position, rotation, scale);
//         EntityManager.SetComponentData(newUnit, unitTransform);

//         // float duration = 3.0f;
//         // Tween.Scale.FromTo(ref EntityManager, newUnit, 0f, 1f, duration, new TweenParams
//         // {
//         //     EaseType = EaseType.BounceOut,
//         // });
//     }

//     public void SpawnUnit(BuildingType type, float3 position, quaternion rotation, float scale)
//     {
//         var config = SystemAPI.GetSingleton<GlobalUnitConfig>();
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
//     }
// }