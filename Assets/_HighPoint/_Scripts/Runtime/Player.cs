using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Persistence;
using UnityEngine;

public class Player : SingletonPersistent<Player>, IBind<PlayerData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [SerializeReference] PlayerData _data;

    public Color Color => StringToColor(_data.Color);
    public int Gold => _data.Gold;

    public bool HasAnyUnits => _data.Units.Any(u => u.Count > 0);

    public static Action<UnitCount> OnUnitCountChanged;
    public static Action<int> OnGoldChanged;

    public void Bind(PlayerData data)
    {
        _data = data;
        _data.Id = Id;
    }

    public void SetColor(Color color)
    {
        _data.Color = ColorToString(color);
        SaveLoadSystem.Instance.SaveGame();
    }

    public void CaptureWayspot(string identifier)
    {
        if (_data.CapturedWayspots.Contains(identifier)) return;

        _data.CapturedWayspots.Add(identifier);
        SaveLoadSystem.Instance.SaveGame();
    }

    public bool DoesPlayerControlWayspot(string identifier)
    {
        if (_data.CapturedWayspots != null &&
            _data.CapturedWayspots.Contains(identifier))
        {
            return true;
        }

        return false;
    }

    public int GetUnitCount(string unit)
    {
        return GetUnit(unit).Count;
    }

    public void SetUnitCount(string unit, int count)
    {
        var u = GetUnit(unit);
        u.Count = count;
        OnUnitCountChanged?.Invoke(u);
        SaveLoadSystem.Instance.SaveGame();
    }

    UnitCount GetUnit(string unit)
    {
        var u = _data.Units.Where(u => u.Name == unit).FirstOrDefault();

        if (u == null)
        {
            var newUnit = new UnitCount(unit);
            _data.Units.Add(newUnit);
            return newUnit;
        }
        else
        {
            return u;
        }
    }

    public void SetGoldCount(int count)
    {
        _data.Gold = count;
        OnGoldChanged?.Invoke(_data.Gold);
        SaveLoadSystem.Instance.SaveGame();
    }

    public static Color StringToColor(string color)
    {
        ColorUtility.TryParseHtmlString(color, out Color result);
        return result;
    }

    public static string ColorToString(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }
}
