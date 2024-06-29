using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.XR.ARFoundation;
using Drawing;

public class Ceiling : Singleton<Ceiling>
{
    // Default ceiling value of 5
    public Vector3 CeilingPos { get; private set; } = Vector3.up * 5f;

    ARPlaneManager _planeManager;

    void OnEnable()
    {
        _planeManager = GetComponent<ARPlaneManager>();

        _planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        _planeManager.planesChanged -= OnPlanesChanged;
    }

    void Update()
    {
        if (BattleUiController.Instance.DebugMode)
        {
            using (Draw.ingame.WithColor(Color.red))
            {
                Draw.ingame.SolidPlane(CeilingPos, Vector3.down, Vector2.one);
            }
        }
    }

    public void OnPlanesChanged(ARPlanesChangedEventArgs changes)
    {
        foreach (var plane in changes.added)
        {
            if (plane.classification != UnityEngine.XR.ARSubsystems.PlaneClassification.Ceiling) continue;

            if (plane.center.y < CeilingPos.y)
            {
                CeilingPos = plane.center;
            }
        }

        foreach (var plane in changes.updated)
        {
            if (plane.classification != UnityEngine.XR.ARSubsystems.PlaneClassification.Ceiling) continue;

            if (plane.center.y < CeilingPos.y)
            {
                CeilingPos = plane.center;
            }
        }

        // foreach (var plane in changes.removed)
        // {
        //     if (plane.classification != UnityEngine.XR.ARSubsystems.PlaneClassification.Floor) continue;
        // }
    }

}