using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klak.Motion;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] OrbitCameraController _orbitController;
    [SerializeField] Camera _cam;
    [SerializeField] Transform _selectedObjCamPosition;

    SmoothFollow _follower;

    EventBinding<MapMarkerSelected> MapMarkerSelected;

    void OnEnable()
    {
        MapMarkerSelected = new EventBinding<MapMarkerSelected>(HandleMapMarkerSelected);
        Bus<MapMarkerSelected>.Register(MapMarkerSelected);
    }

    void OnDisable()
    {
        Bus<MapMarkerSelected>.Unregister(MapMarkerSelected);
    }

    protected override void OnAwake()
    {
        _follower = _cam.GetComponent<SmoothFollow>();
        Reset();
    }

    private void HandleMapMarkerSelected(MapMarkerSelected @event)
    {
        var t = @event.Selected?.gameObject.transform;

        var lookOffset = new Vector3(0f, 7f, 0f);

        if (@event.Selected is VpsMarker)
        {
            lookOffset = new Vector3(0f, 7f, 0f);
        }
        else if (@event.Selected is OutpostMarker)
        {
            lookOffset = new Vector3(0f, 1f, 0f);
        }

        SetCameraTarget(t, lookOffset);
    }

    public void SetCameraTarget(Transform target, Vector3 lookOffset = default)
    {
        if (target == null)
        {
            Reset();
            return;
        }

        // Vector3 cameraTargetPos = target.position
        //     + new Vector3(0f, 20f, 0f)
        //     + target.forward * 30f;

        Vector3 cameraTargetPos = target.position
            + new Vector3(0f, 15f, 0f)
            + target.forward * 30f;

        var lookPos = target.position + lookOffset;
        Vector3 distance = (lookPos - cameraTargetPos).normalized;

        _selectedObjCamPosition.SetPositionAndRotation(
            cameraTargetPos,
            Quaternion.LookRotation(distance));

        _orbitController.enabled = false;
        _follower.enabled = true;
        _follower.target = _selectedObjCamPosition;
    }

    void Reset()
    {
        _orbitController.enabled = true;
        _follower.enabled = false;
        _follower.target = null;
    }
}