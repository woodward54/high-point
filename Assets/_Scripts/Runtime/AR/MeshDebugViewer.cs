using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARMeshManager))]
public class MeshDebugViewer : MonoBehaviour
{
    [SerializeField] Material _meshReceiveShadows;
    [SerializeField] Material _meshDebug;

    ARMeshManager _meshManager;

    EventBinding<DebugModeEvent> _debugModeBinding;

    void OnEnable()
    {
        _meshManager = GetComponent<ARMeshManager>();

        _debugModeBinding = new EventBinding<DebugModeEvent>(HandleDebugModeChanged);
        Bus<DebugModeEvent>.Register(_debugModeBinding);

        _meshManager.meshPrefab.GetComponent<MeshRenderer>().material = _meshReceiveShadows;
        _meshManager.meshPrefab.GetComponent<MeshRenderer>().enabled = false;
    }

    void OnDisable()
    {
        Bus<DebugModeEvent>.Unregister(_debugModeBinding);
    }

    void HandleDebugModeChanged(DebugModeEvent @event)
    {
        var mat = @event.DebugMode ? _meshDebug : _meshReceiveShadows;
        _meshManager.meshes.ToList().ForEach(m => m.GetComponent<MeshRenderer>().material = mat);
        _meshManager.meshPrefab.GetComponent<MeshRenderer>().material = mat;

        // Disable all mesh rendering for now
        _meshManager.meshes.ToList().ForEach(m => m.GetComponent<MeshRenderer>().enabled = @event.DebugMode);
        _meshManager.meshPrefab.GetComponent<MeshRenderer>().enabled = @event.DebugMode;
    }
}