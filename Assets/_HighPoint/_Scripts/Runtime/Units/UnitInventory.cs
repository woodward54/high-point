using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

// TODO: Refactor this to use Player.OnUnitCountChanged
public class UnitInventory : Singleton<UnitInventory>
{
    public void RegisterUnitType(AgentConfig cfg)
    {
        var count = Player.Instance.GetUnitCount(cfg.name);

        UnitSelector.Instance.UpdateUnitButtonCount(cfg, count);
    }

    public Unit SpawnUnit(AgentConfig cfg, Vector3 position, Quaternion rotation, float scale = 1.5f)
    {
        var count = Player.Instance.GetUnitCount(cfg.name);
        if (count <= 0) return null;

        count--;

        UnitSelector.Instance.UpdateUnitButtonCount(cfg, count);

        Player.Instance.SetUnitCount(cfg.name, count);

        return UnitManager.Instance.SpawnUnit(cfg, position, rotation, scale);
    }

    public void IncreaseUnitCount(AgentConfig cfg, int amount)
    {
        var count = Player.Instance.GetUnitCount(cfg.name);

        count += amount;

        UnitSelector.Instance.UpdateUnitButtonCount(cfg, count);

        Player.Instance.SetUnitCount(cfg.name, count);
    }
}