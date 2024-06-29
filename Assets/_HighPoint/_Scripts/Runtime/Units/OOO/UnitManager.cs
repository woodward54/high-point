using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Unity.Collections;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] MMF_Player _floatingTextPlayer;

    public List<Unit> Units => _units.Where(u => u.gameObject.activeInHierarchy && !u.IsDead).ToList();
    readonly List<Unit> _units = new();

    public Unit SpawnUnit(UnitConfig cfg, Vector3 position, Quaternion rotation, float scale = 1f)
    {
        // TODO replace this with object pool
        var newUnitGO = Instantiate(cfg.Prefab.transform, position, rotation, transform);
        var newUnit = newUnitGO.GetComponent<Unit>();

        newUnit.Setup(_floatingTextPlayer);

        newUnitGO.transform.localScale *= scale;
        newUnitGO.transform.localScale *= HexGrid.Instance.HexSize;

        _units.Add(newUnit);

        if (newUnit is BuildingUnit unit)
        {
            var cell = HexGrid.Instance.GetNearest(position);
            cell.Building = unit;
        }

        return newUnit;
    }
}