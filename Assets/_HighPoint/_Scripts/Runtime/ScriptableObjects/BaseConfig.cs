using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingCountConfig
{
    [field: SerializeField] public BuildingConfig Building;
    [field: SerializeField][Range(0f, 1f)] public float Frequency = 0.95f;
    [field: SerializeField] public int Min = 0;
    [field: SerializeField] public int Max = 5;
    [field: SerializeField] public bool SpawnOutsideWalls = false;
}

[CreateAssetMenu(fileName = "Base Config", menuName = "Operation High Point/Base Config")]
public class BaseConfig : ScriptableObject
{
    [field: Header("Base Engine Components")]
    [field: SerializeField] public List<BuildingCountConfig> Buildings { get; private set; }
    [field: SerializeField] public BuildingConfig Wall { get; private set; }
}