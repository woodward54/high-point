using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Operation High Point/Unit", fileName = "Unit")]
public class UnitConfig : ScriptableObject
{
    [field: Header("Basic")]
    [field: SerializeField] public float Health { get; private set; } = 100f;
    [field: SerializeField] public Transform Prefab { get; private set; }
    [field: SerializeField] public Sprite UiIcon { get; private set; }
    [field: SerializeField] public float HealthBarOffset { get; private set; } = 2f;
    [field: SerializeField] public int Price { get; private set; } = 100;

    [field: Header("Attack/Action")]
    [field: SerializeField] public float AttackRange { get; private set; } = 10f;
    [field: SerializeField] public float SecondsPerAttack { get; private set; } = 1f;
    [field: SerializeField] public float DamagePerSecond { get; private set; } = 10f;
    [field: SerializeField] public float DamageDelay { get; private set; } = 0.2f;
}