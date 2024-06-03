using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Agent", menuName = "Operation High Point/Agent")]
public class AgentConfig : UnitConfig
{
    [field: Header("Movement")]
    [field: SerializeField] public float Speed { get; private set; }
}