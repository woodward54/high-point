using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class UnitsDisplay : MonoBehaviour
{
    TMP_Text _unitsText;

    EventBinding<UnitSpawnEvent> SpawnedUnitBinding;
    EventBinding<UnitDeathEvent> DeadUnitBinding;

    int _unitCount = 0;

    void Awake()
    {
        _unitsText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        SpawnedUnitBinding = new EventBinding<UnitSpawnEvent>(HandleUnitSpawn);
        Bus<UnitSpawnEvent>.Register(SpawnedUnitBinding);

        DeadUnitBinding = new EventBinding<UnitDeathEvent>(HandleUnitDead);
        Bus<UnitDeathEvent>.Register(DeadUnitBinding);
    }

    void OnDisable()
    {
        Bus<UnitSpawnEvent>.Unregister(SpawnedUnitBinding);
        Bus<UnitDeathEvent>.Unregister(DeadUnitBinding);
    }

    void HandleUnitDead(UnitDeathEvent @event)
    {
        _unitCount--;

        UpdateUnitCount();
    }

    void HandleUnitSpawn(UnitSpawnEvent @event)
    {
        _unitCount++;

        UpdateUnitCount();
    }

    void UpdateUnitCount()
    {
        _unitsText.text = "Units: " + _unitCount;
    }
}