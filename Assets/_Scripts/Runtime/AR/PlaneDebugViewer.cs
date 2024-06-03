using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneDebugViewer : MonoBehaviour
{
    [SerializeField] Material _planeTransparent;
    [SerializeField] Material _planeDebug;

    ARPlaneManager _planeManager;

    EventBinding<DebugModeEvent> _debugModeBinding;

    void OnEnable()
    {
        _planeManager = GetComponent<ARPlaneManager>();

        _debugModeBinding = new EventBinding<DebugModeEvent>(HandleDebugModeChanged);
        Bus<DebugModeEvent>.Register(_debugModeBinding);

        _planeManager.planePrefab.GetComponent<MeshRenderer>().material = _planeTransparent;
    }

    void OnDisable()
    {
        Bus<DebugModeEvent>.Unregister(_debugModeBinding);
    }

    void HandleDebugModeChanged(DebugModeEvent @event)
    {
        var mat = @event.DebugMode ? _planeDebug : _planeTransparent;

        _planeManager.planePrefab.GetComponent<MeshRenderer>().material = mat;

        foreach (var plane in _planeManager.trackables)
        {
            plane.GetComponent<MeshRenderer>().material = mat;
        }
    }
}