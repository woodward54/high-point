using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    public List<Unit> Units { get; private set; } = new();

    public Unit SpawnUnit(UnitConfig cfg, Vector3 position, Quaternion rotation, float scale = 1f)
    {
        // TODO replace this with object pool
        var newUnitGO = Instantiate(cfg.Prefab.transform, position, rotation, transform);
        var newUnit = newUnitGO.GetComponent<Unit>();

        newUnitGO.transform.localScale *= scale;
        newUnitGO.transform.localScale *= HexGrid.Instance.HexSize;

        Units.Add(newUnit);

        if (newUnit is BuildingUnit unit)
        {
            var cell = HexGrid.Instance.GetNearest(position);
            cell.Building = unit;
        }

        return newUnit;
    }
}