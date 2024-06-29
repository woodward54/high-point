using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARMeshManager))]
public class MeshManagerController : MonoBehaviour
{
    ARMeshManager _meshManager;

    EventBinding<GameStateChangedEvent> GameStateChanged;

    void OnEnable()
    {
        _meshManager = GetComponent< ARMeshManager>();

        GameStateChanged = new EventBinding<GameStateChangedEvent>(HandleGameStateChanged);
        Bus<GameStateChangedEvent>.Register(GameStateChanged);
    }

    void OnDisable()
    {
        Bus<GameStateChangedEvent>.Unregister(GameStateChanged);
    }

    void HandleGameStateChanged(GameStateChangedEvent @event)
    {
        switch (@event.State)
        {
            case GameState.BuildingBase:
                _meshManager.enabled = false;
                break;
        }
    }
}