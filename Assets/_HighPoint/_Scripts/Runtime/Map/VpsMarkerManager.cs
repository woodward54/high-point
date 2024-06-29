using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Niantic.Lightship.AR;
using Niantic.Lightship.AR.VpsCoverage;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.MapLayers.Components;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CoverageClientManager))]
public class VpsMarkerManager : Singleton<VpsMarkerManager>
{
    [SerializeField] LayerGameObjectPlacement _vpsMarkerSpawner;

    CoverageClientManager _coverageClientManager;
    LightshipMapView _mapView;
    LightshipMapManager _mapManager;

    List<VpsMarker> _markers = new();

    void OnEnable()
    {
        PlayerLocationController.OnNewGpsPosition += HandleNewGpsPosition;
    }

    void OnDisable()
    {
        PlayerLocationController.OnNewGpsPosition -= HandleNewGpsPosition;
    }

    protected override void OnAwake()
    {
        _coverageClientManager = GetComponent<CoverageClientManager>();
        _mapView = FindAnyObjectByType<LightshipMapView>();
    }

    /// <summary>
    /// Setup listeners and callbacks to Change UI and set updated values coverage API Manager.
    /// </summary>
    void Start()
    {
        //_latitudeTextField.text = _coverageClientManager.QueryLatitude.ToString(CultureInfo.CurrentCulture);
        //_latitudeTextField.onValueChanged.AddListener(OnLatitudeChanged);
        // OnLatitudeChanged(_coverageClientManager.QueryLatitude.ToString(CultureInfo.CurrentCulture));

        //_longitudeTextField.text = _coverageClientManager.QueryLongitude.ToString(CultureInfo.CurrentCulture);
        //_longitudeTextField.onValueChanged.AddListener(OnLongitudeChanged);
        // OnLongitudeChanged(_coverageClientManager.QueryLongitude.ToString(CultureInfo.CurrentCulture));

        // RequestAreas();
    }

    public void RequestAreas()
    {
        Debug.Log("Requesting coverage from server...");
        _coverageClientManager.TryGetCoverage(OnTryGetCoverage);
    }

    void HandleNewGpsPosition(Niantic.Lightship.Maps.Core.Coordinates.LatLng lng)
    {
        Debug.Log("New GPS Position: " + lng.ToString());

        _coverageClientManager.QueryLatitude = (float)lng.Latitude;
        _coverageClientManager.QueryLongitude = (float)lng.Longitude;

        RequestAreas();
    }

    /// <summary>
    /// Clears list, Gets result from coverage around the selected location and sorts it to be presentable.
    /// </summary>
    void OnTryGetCoverage(AreaTargetsResult areaTargetsResult)
    {
        var responseText = string.Empty;

        if (areaTargetsResult.Status == ResponseStatus.Success)
        {
            areaTargetsResult.AreaTargets.Sort((a, b) =>
                a.Area.Centroid.Distance(areaTargetsResult.QueryLocation).CompareTo(
                    b.Area.Centroid.Distance(areaTargetsResult.QueryLocation)));

            Debug.Log("VPS Locations Found: " + areaTargetsResult.AreaTargets.Count);

            foreach (var areaTarget in areaTargetsResult.AreaTargets)
            {
                // Don't add duplicates
                // TODO: need a way to despawn them if out of range
                if (_markers.Any(m => m.VpsTarget.Identifier == areaTarget.Target.Identifier)) continue;

                var latLng = new Niantic.Lightship.Maps.Core.Coordinates.LatLng(
                                    areaTarget.Target.Center.Latitude, areaTarget.Target.Center.Longitude);

                var randNum = HexUtils.SideAngle(UnityEngine.Random.Range(0, 6));
                // var angle = Quaternion.Euler(0f, randNum, 0f);

                var pooledObj = _vpsMarkerSpawner.PlaceInstance(latLng, Quaternion.identity, areaTarget.Target.Name);
                var marker = pooledObj.Value.GetComponent<VpsMarker>();
                marker.Setup(areaTarget.Target);

                _markers.Add(marker);
            }
        }
        else
        {
            responseText = "Response : " + areaTargetsResult.Status;
        }

        OutpostMakerManager.Instance.SpawnOutposts();
    }

    public bool IsPositionNearAnyMaker(Vector3 position)
    {
        float declutterRange = 20f;
        return _markers.Any(m => Vector3.Distance(m.transform.position, position) <= declutterRange);
    }

    void ClearMarkers()
    {
        foreach (var item in _markers)
        {
            Destroy(item.gameObject);
        }

        _markers.Clear();
    }

    void OnLatitudeChanged(string newLatitude)
    {
        // if (!float.TryParse(_latitudeTextField.text, out float latValue))
        // {
        //     _coverageClientManager.QueryLatitude = 0;
        // }
        // else
        // {
        //     _coverageClientManager.QueryLatitude = latValue;
        // }
    }

    void OnLongitudeChanged(string newLongitude)
    {
        // if (!float.TryParse(_longitudeTextField.text, out float longValue))
        // {
        //     _coverageClientManager.QueryLongitude = 0;
        // }
        // else
        // {
        //     _coverageClientManager.QueryLongitude = longValue;
        // }
    }
}