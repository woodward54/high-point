using System;
using System.Collections;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core.Coordinates;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class PlayerLocationController : Singleton<PlayerLocationController>
{
    public const float BATTLE_RANGE = 35f;

    [SerializeField] LightshipMapView _lightshipMapView;
    [SerializeField] float _editorMovementSpeed;
    [SerializeField] Camera _camera;
    [SerializeField] Transform _rangeRing;

    double _lastGpsUpdateTime;
    Vector3 _targetMapPosition;
    Vector3 _currentMapPosition;
    float _lastMapViewUpdateTime;

    public static Action<string> OnGpsError;
    public static Action<LatLng> OnNewGpsPosition;

    const float WalkThreshold = 0.5f;
    const float TeleportThreshold = 200f;

    static bool IsLocationServiceInitializing
       => Input.location.status == LocationServiceStatus.Initializing;

    void Start()
    {
        _lightshipMapView.MapOriginChanged += OnMapViewOriginChanged;
        _currentMapPosition = _targetMapPosition = transform.position;

        // Maps doesnt start if the default location is not set in the editor
        // bug in niantic maps code
#if UNITY_EDITOR
        var location = new LatLng(37.795663229391415, -122.39360531754329);
        _lightshipMapView.SetMapCenter(location);
        UpdatePlayerLocation(location);
        OnNewGpsPosition?.Invoke(location);
#endif

        var rangeRingSize = (BATTLE_RANGE - 5f) / 10f;
        _rangeRing.transform.localScale = Vector3.one * rangeRingSize;

        StartCoroutine(UpdateGpsLocation());
    }

    void OnMapViewOriginChanged(LatLng center)
    {
        var offset = _targetMapPosition - _currentMapPosition;
        _currentMapPosition = _lightshipMapView.LatLngToScene(center);
        _targetMapPosition = _currentMapPosition + offset;
    }

    IEnumerator UpdateGpsLocation()
    {
        yield return null;

        if (Application.isEditor)
        {
            while (isActiveAndEnabled)
            {
                UpdateEditorInput();
                yield return null;
            }
        }
        else
        {
#if UNITY_ANDROID
                // Request location permission for Android
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Permission.RequestUserPermission(Permission.FineLocation);
                    while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                    {
                        // Wait until permission is enabled
                        yield return new WaitForSeconds(1.0f);
                    }
                }
#endif
            // Check if the user has location service enabled.
            if (!Input.location.isEnabledByUser)
            {
                OnGpsError?.Invoke("Location permission not enabled");
                yield break;
            }

            // Starts the location service.
            Input.location.Start();

            // Waits until the location service initializes
            int maxWait = 20;
            while (IsLocationServiceInitializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // If the service didn't initialize in 20
            // seconds, this cancels location service use.
            if (maxWait < 1)
            {
                OnGpsError?.Invoke("GPS initialization timed out");
                yield break;
            }

            // If the connection failed this cancels location service use.
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                OnGpsError?.Invoke("Unable to determine device location");
                yield break;
            }

            while (isActiveAndEnabled)
            {
                var gpsInfo = Input.location.lastData;
                if (gpsInfo.timestamp > _lastGpsUpdateTime)
                {
                    _lastGpsUpdateTime = gpsInfo.timestamp;
                    var location = new LatLng(gpsInfo.latitude, gpsInfo.longitude);
                    UpdatePlayerLocation(location);
                    OnNewGpsPosition?.Invoke(location);
                }

                yield return null;
            }

            // Stops the location service if there is no
            // need to query location updates continuously.
            Input.location.Stop();
        }
    }

    void UpdatePlayerLocation(in LatLng location)
    {
        // New GPS location data available, will lerp the player's
        // position to this new coordinate, or jump if it is far.
        _targetMapPosition = _lightshipMapView.LatLngToScene(location);
    }

    public void Update()
    {
        // Update the map view position based on where our player is.
        // This will actually be last frame's position, but the map
        // update needs to happen first as the player is positioned
        // on the map relative to the offset to the tile parent node.
        UpdateMapViewPosition();

        // Maintain the player's position on the map, and interpolate
        // to new coordinates as they come in.  Interpolate player's
        // map position without the camera offset, so that camera
        // movements don't result in lerps.  Jump rather than
        // interpolate if the coordinates are really far.

        var movementVector = _targetMapPosition - _currentMapPosition;
        var movementDistance = movementVector.magnitude;

        switch (movementDistance)
        {
            case > TeleportThreshold:
                _currentMapPosition = _targetMapPosition;
                break;

            case > WalkThreshold:
                {
                    // If the player is not stationary,
                    // rotate to face their movement vector
                    var forward = movementVector.normalized;
                    var rotation = Quaternion.LookRotation(forward, Vector3.up);
                    transform.rotation = rotation;
                    break;
                }
        }

        _currentMapPosition = Vector3.Lerp(
            _currentMapPosition,
            _targetMapPosition,
            Time.deltaTime);

        transform.position = _currentMapPosition;
    }

    void UpdateEditorInput()
    {
        // In the Editor, move the character around
        // with the keyboard rather than GPS.

        var movementVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movementVector += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector -= Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector += Vector3.right;
        }

        // Make Editor movement relative to the camera's forward direction
        var cameraForward = _camera.transform.forward;
        float yRotation = Vector3.SignedAngle(Vector3.forward, cameraForward, Vector3.up);
        movementVector = Quaternion.AngleAxis(yRotation, Vector3.up) * movementVector;

        _targetMapPosition += movementVector * (_editorMovementSpeed * Time.deltaTime);
    }

    void UpdateMapViewPosition()
    {
        // Only update the map tile view periodically so as not to spam tile fetches
        if (Time.time < _lastMapViewUpdateTime + 1.0f)
        {
            return;
        }

        _lastMapViewUpdateTime = Time.time;

        // Update the map's view based on where our player is
        _lightshipMapView.SetMapCenter(transform.position);
    }

    public Vector3 RandomPointNearPlayer(float radius = BATTLE_RANGE)
    {
        var randPointInDist = UnityEngine.Random.insideUnitCircle * radius;
        return new Vector3(randPointInDist.x, 0f, randPointInDist.y) + transform.position;
    }
}

