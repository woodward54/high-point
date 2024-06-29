using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// This could be optimized by having one master VFX instance and sending position each frame with a graphics buffer
namespace Systems.Battle
{
    public class BuildingExplosionVfxController : PoolerBase<ReturnVisualEffectToPool>
    {
        [SerializeField] ReturnVisualEffectToPool _vfxPrefab;

        readonly int _boomId = Shader.PropertyToID("OnBoom");

        EventBinding<UnitDeathEvent> _unitDiedBinding;
        EventBinding<UnitSpawnEvent> _unitSpawnBinding;

        readonly Queue<Vector3> _spawnPositions = new();

        void OnEnable()
        {
            _unitDiedBinding = new EventBinding<UnitDeathEvent>(HandleUnitDied);
            Bus<UnitDeathEvent>.Register(_unitDiedBinding);

            _unitSpawnBinding = new EventBinding<UnitSpawnEvent>(HandleUnitSpawn);
            Bus<UnitSpawnEvent>.Register(_unitSpawnBinding);
        }

        void OnDisable()
        {
            Bus<UnitDeathEvent>.Unregister(_unitDiedBinding);
            Bus<UnitSpawnEvent>.Unregister(_unitSpawnBinding);
        }

        void Start()
        {
            InitPool(_vfxPrefab, 20, 100);
        }

        void Update()
        {
            if (_spawnPositions.Count == 0) return;

            // We should be allowed to get multiple vfx per frame from the pool (separate instances), 
            // but I was seeing a bug where if 30+ instances were triggered in one frame some of them wouldn't trigger.
            // So for now, one SendEvent allowed per frame
            TriggerVfx(_spawnPositions.Dequeue());
        }

        void TriggerVfx(Vector3 pos)
        {
            var vfx = Get();
            vfx.transform.position = pos;

            vfx.VisualEffect.SendEvent(_boomId);
        }

        protected override ReturnVisualEffectToPool CreatePooledItem()
        {
            var go = Instantiate(_vfxPrefab.gameObject);
            go.transform.localScale *= HexGrid.Instance.HexSize;
            go.transform.SetParent(transform);

            var returnToPool = go.GetComponent<ReturnVisualEffectToPool>();
            returnToPool.Setup(Pool);

            return returnToPool;
        }

        void HandleUnitDied(UnitDeathEvent @event)
        {
            if (@event.Unit is BuildingUnit)
            {
                // Arbitrary height based on HexSize to spawn them
                var yOffset = new Vector3(0f, HexGrid.Instance.HexSize, 0f);

                var spawnPos = @event.Unit.transform.position + yOffset;

                _spawnPositions.Enqueue(spawnPos);
            }
        }

        void HandleUnitSpawn(UnitSpawnEvent @event)
        {
            // if (@event.Unit is BuildingUnit)
            // {
            //     // Arbitrary height based on HexSize to spawn them
            //     var yOffset = new Vector3(0f, HexGrid.Instance.HexSize, 0f);

            //     var spawnPos = @event.Unit.transform.position + yOffset;

            //     _spawnPositions.Enqueue(spawnPos);
            // }
        }
    }
}