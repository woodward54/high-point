// using System;
// using System.Collections.Generic;
// using TMPro;
// using Unity.Mathematics;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Linq;
// using Unity.Entities;
// using Unity.Transforms;

// public class EntityHealthBar
// {
//     readonly Entity _unit;
//     readonly CanvasGroup _healthBar;
//     readonly Image _barImage;
//     float3 _offset;

//     float _lastUpdateTime;

//     public EntityHealthBar(Entity unit, CanvasGroup healthBar, Image barImage, float yOffset)
//     {
//         _unit = unit;
//         _healthBar = healthBar;
//         _barImage = barImage;
//         _offset = new Vector3(0f, yOffset, 0f);

//         _lastUpdateTime = Time.time;
//     }

//     public void SetHealthPercent(float percent)
//     {
//         var s = _barImage.transform.localScale;
//         s.x = math.clamp(percent, 0f, 1f);
//         _barImage.transform.localScale = s;

//         _lastUpdateTime = Time.time;
//     }

//     public void UpdatePosition(Transform camera, EntityManager entityManager)
//     {
//         // _unitOwner
//         var transData = entityManager.GetComponentData<LocalTransform>(_unit);

//         _healthBar.transform.position = transData.Position + _offset;

//         var target = camera.position;
//         target.y = _healthBar.transform.position.y;
//         _healthBar.transform.LookAt(target);
//     }

//     public void UpdateFadeOut(float currentTime, float fadeOutTime)
//     {
//         var t = currentTime - (_lastUpdateTime + fadeOutTime);
//         _healthBar.alpha = Mathf.Clamp(Mathf.SmoothStep(1f, 0f, t), 0, 1);
//     }

//     public void Destroy()
//     {
//         UnityEngine.Object.Destroy(_healthBar.gameObject);
//     }
// }

// public class EntityHealthBarManager : Singleton<EntityHealthBarManager>
// {
//     [SerializeField] Transform _healthBarPrefab;
//     [SerializeField] Transform _camera;
//     [SerializeField] float _fadeOutTime = 1.0f;

//     EntityManager _entityManager;

//     Dictionary<Entity, EntityHealthBar> _healthBars = new();

//     void OnEnable()
//     {
//         _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

//         var healthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HealthSystem>();
//         healthSystem.OnHealthChanged += UpdateUnitHealthBar;
//         healthSystem.OnUnitDied += DestroyHealthBar;

//         var spawnUnitSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpawnUnitSystem>();
//         spawnUnitSystem.OnUnitSpawned += CreateHealthBar;

//         // MapGenerator.Instance.OnNewMapGenerationStated += ClearHealthBars;
//     }

//     void OnDisable()
//     {
//         if (World.DefaultGameObjectInjectionWorld == null) return;

//         var healthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HealthSystem>();
//         healthSystem.OnHealthChanged -= UpdateUnitHealthBar;
//         healthSystem.OnUnitDied += DestroyHealthBar;

//         var spawnUnitSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpawnUnitSystem>();
//         spawnUnitSystem.OnUnitSpawned -= CreateHealthBar;

//         // MapGenerator.Instance.OnNewMapGenerationStated -= ClearHealthBars;
//     }

//     public void CreateHealthBar(Entity owner)
//     {
//         var healthBar = Instantiate(_healthBarPrefab, Vector3.zero, Quaternion.identity, transform);
//         var healthBarCanvas = healthBar.GetComponent<CanvasGroup>();
//         healthBar.localScale *= HexGrid.Instance.HexTerrainSize;
//         var healthBarChildImg = healthBar.GetComponentsInChildren<Image>().First(c => c.transform != healthBar);

//         var health = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Health>(owner);
//         float yOffset = HexGrid.Instance.HexTerrainSize * health.HealthBarYOffset;
        
//         var bar = new EntityHealthBar(owner, healthBarCanvas, healthBarChildImg, yOffset);

//         _healthBars.Add(owner, bar);
//     }

//     public void UpdateUnitHealthBar(int currentHealth, int maxHealth, Entity unit)
//     {
//         _healthBars[unit].SetHealthPercent(currentHealth / (float)maxHealth);
//     }

//     private void DestroyHealthBar(Entity entity)
//     {
//         _healthBars[entity].Destroy();
//         _healthBars.Remove(entity);
//     }

//     void Update()
//     {
//         var curTime = Time.time;
//         foreach (var bar in _healthBars.Values)
//         {
//             bar.UpdatePosition(_camera, _entityManager);
//             bar.UpdateFadeOut(curTime, _fadeOutTime);
//         }
//     }

//     void ClearHealthBars()
//     {
//         foreach (var bar in _healthBars.Values)
//         {
//             bar.Destroy();
//         }

//         _healthBars.Clear();
//     }
// }