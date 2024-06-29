using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Niantic.Lightship.AR;
using Niantic.Lightship.AR.VpsCoverage;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.MapLayers.Components;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class OutpostMakerManager : Singleton<OutpostMakerManager>
{
    [SerializeField] LayerGameObjectPlacement _markerSpawner;

    CoverageClientManager _coverageClientManager;

    List<OutpostMarker> _markers = new();

    public void SpawnOutposts()
    {
        ClearOutposts();

        for (int i = 0; i < 3; i++)
        {
            SpawnOutpost();
        }
    }

    void SpawnOutpost()
    {
        var rndPosition = PlayerLocationController.Instance.RandomPointNearPlayer();

        // Safety
        int i=0;
        while (VpsMarkerManager.Instance.IsPositionNearAnyMaker(rndPosition) || IsPositionNearAnyMaker(rndPosition))
        {
            // Bail!
            if (i >= 40) return;
            i++;

            rndPosition = PlayerLocationController.Instance.RandomPointNearPlayer();
        }

        var pooledObj = _markerSpawner.PlaceInstance(rndPosition, "Outpost");

        var marker = pooledObj.Value.GetComponent<OutpostMarker>();

        _markers.Add(marker);
    }

    public bool IsPositionNearAnyMaker(Vector3 position)
    {
        float declutterRange = 20f;
        return _markers.Any(m => Vector3.Distance(m.transform.position, position) <= declutterRange);
    }

    void ClearOutposts()
    {
        foreach (var item in _markers)
        {
            Destroy(item.gameObject);
        }

        _markers.Clear();
    }
}