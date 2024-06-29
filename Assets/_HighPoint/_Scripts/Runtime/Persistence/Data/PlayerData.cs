using System;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

[Serializable]
public class UnitCount
{
    [field: SerializeField] public SerializableGuid Id;
    public string Name;
    public int Count;

    public UnitCount(string name, int count = 0)
    {
        Id = SerializableGuid.NewGuid();
        Name = name; 
        Count = count;
    }
}

[Serializable]
public class PlayerData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; }
    public string Color = "#2597fa";
    public int Gold = 2700;
    public List<string> CapturedWayspots;
    public List<UnitCount> Units = new();

    public PlayerData()
    {
        // Give player some starting units
        Blackboard.Instance.AllUnits.ForEach(u => Units.Add(new UnitCount(u.name, 5)));
    }
}
