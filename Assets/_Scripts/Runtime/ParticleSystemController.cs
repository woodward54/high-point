using System;
using UnityEngine;

public class ParticleSystemController : PoolerBase<ReturnParticlesToPool>
{
    [SerializeField] ReturnParticlesToPool _particleSystem;

    EventBinding<UnitDeathEvent> UnitDiedBinding;

    void OnEnable()
    {
        UnitDiedBinding = new EventBinding<UnitDeathEvent>(HandleUnitDied);
        Bus<UnitDeathEvent>.Register(UnitDiedBinding);
    }

    void OnDisable()
    {
        Bus<UnitDeathEvent>.Unregister(UnitDiedBinding);
    }

    void Start()
    {
        InitPool(_particleSystem, 20, 100);
    }

    protected override ReturnParticlesToPool CreatePooledItem()
    {
        var go = Instantiate(_particleSystem.gameObject);
        go.transform.localScale *= HexGrid.Instance.HexSize;
        go.transform.SetParent(transform);

        var basePs = go.GetComponent<ParticleSystem>();
        basePs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var returnToPool = go.GetComponent<ReturnParticlesToPool>();
        returnToPool.Setup(Pool);

        return returnToPool;
    }

    void HandleUnitDied(UnitDeathEvent @event)
    {
        if (@event.Unit is BuildingUnit)
        {
            var psGO = Get();

            // Arbitrary height to spawn them
            var yOffset = new Vector3(0f, HexGrid.Instance.HexSize, 0f);

            psGO.transform.position = @event.Unit.transform.position + yOffset;

            psGO.System.Play();
        }
    }
}