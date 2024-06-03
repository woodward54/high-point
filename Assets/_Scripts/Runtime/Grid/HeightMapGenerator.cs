using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Drawing;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HeightMapGenerator : Singleton<HeightMapGenerator>
{
    [SerializeField] bool _raycastPlanes;
    [SerializeField] bool _raycastMesh;
    [SerializeField] float _scanHeight = 2f;
    [SerializeField] float _scanTime = 1f;

    public float[,] HeightMap { get; private set; }
    public int[,] HeightMapFiltered { get; private set; }
    public const float RAYCAST_MISS = -1000f;
    public const float WALL = 1000f;

    int Width;
    int Height;

    ARPlaneManager _planeManager;
    Coroutine _heightScanCoroutine;
    readonly List<Bounds> _wallBounds = new();

    EventBinding<GameStateChangedEvent> GameStateChanged;

    public Action<int[,]> OnNewHeightMapData;

    void OnEnable()
    {
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
            case GameState.BuildingTerrain:
                _heightScanCoroutine = StartCoroutine(GenerateHeightMapCoroutine());
                break;

            case GameState.Battle:
                StopScanning();
                break;

            default:
                break;
        }
    }

    void Start()
    {
        Width = HexGrid.Instance.Width;
        Height = HexGrid.Instance.Depth;
        HeightMap = new float[Width, Height];
        HeightMapFiltered = new int[Width, Height];

        _planeManager = FindObjectOfType<ARPlaneManager>();
        _planeManager.planePrefab.SetActive(_raycastPlanes);

        FindObjectOfType<ARMeshManager>().gameObject.SetActive(_raycastMesh);
    }

    void Update()
    {
        // Debug
        if (UiController.Instance.DebugMode)
        {
            var rayStartPos = HexGrid.Instance.ApproxCenterOfGrid();

            rayStartPos.y += _scanHeight;

            using (Draw.ingame.WithColor(Color.magenta))
            {
                Draw.ingame.Cross(rayStartPos);
            }
        }
    }

    void StopScanning()
    {
        StopCoroutine(_heightScanCoroutine);
    }

    void ClearHeightMap()
    {
        HeightMap = new float[Width, Height];
        HeightMapFiltered = new int[Width, Height];
    }

    // void FindWalls()
    // {
    //     _wallBounds.Clear();

    //     var scanHeight = HexGrid.Instance.ApproxCenterOfGrid();
    //     scanHeight.y += _scanHeight;

    //     // Get all vertical planes taller than the _scanHeight
    //     foreach (var plane in _planeManager.trackables)
    //     {
    //         if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
    //         {
    //             if (plane.center.y > scanHeight.y)
    //             {
    //                 var bounds = plane.GetComponent<MeshFilter>().mesh.bounds;

    //                 _wallBounds.Add(bounds);
    //             }
    //         }
    //     }

    //     // Use noise to sudo generate the "Mountain peaks" roughly around the _scanHeight
    // }

    // bool IsTileWall(Vector3 hexPos)
    // {
    //     // If hex is in range of one of these wall planes, mark it as such
    //     foreach (var bounds in _wallBounds)
    //     {
    //         // Only need to check 2d
    //         hexPos.y = bounds.center.y;

    //         if (bounds.SqrDistance(hexPos) < HexGrid.Instance.HexSize * HexGrid.Instance.HexSize)
    //         {
    //             return true;
    //         }
    //     }

    //     return false;
    // }

    IEnumerator GenerateHeightMapCoroutine()
    {
        var meshLayer = LayerMask.NameToLayer("Mesh");
        int layerMask = 1 << meshLayer;

        var raycastRadius = HexGrid.Instance.HexSize * 0.25f;

        while (true)
        {
            Debug.Log("Scanning...");

            // FindWalls();

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Height; z++)
                {
                    var coords = new Vector3Int(x, 0, z);

                    var hexPos = HexGrid.Instance.GetHexPosition(coords);

                    // if (IsTileWall(hexPos))
                    // {
                    //     HeightMapFiltered[x, z] = (int)WALL;
                    //     HeightMap[x, z] = WALL;
                    // }
                    // else
                    // {
                        var rayPos = hexPos;
                        rayPos.y += _scanHeight;

                        // if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, _scanHeight, layerMask))
                        // if (Physics.CapsuleCast(rayPos, Vector3.down, out RaycastHit hit, _scanHeight, layerMask))

                        if (Physics.SphereCast(rayPos, raycastRadius, Vector3.down, out RaycastHit hit, _scanHeight * 10f, layerMask))
                        // if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, _scanHeight * 10f, layerMask))
                        {
                            Debug.DrawRay(rayPos, Vector3.down * hit.distance, Color.red, _scanTime - 2f);

                            var worldYPos = hit.point.y;
                            var gridYPos = HexGrid.Instance.transform.position.y;
                            var yStepSize = HexGrid.Instance.YStepSize;
                            worldYPos = worldYPos > gridYPos ? worldYPos : gridYPos;

                            var relativeToGrid = worldYPos - gridYPos;
                            int yInGridCoords = (int)Math.Round(MathUtils.RoundToMultipleOf(relativeToGrid, yStepSize) / yStepSize);

                            HeightMapFiltered[x, z] = yInGridCoords;
                            HeightMap[x, z] = hit.point.y;
                        }
                        else
                        {
                            HeightMapFiltered[x, z] = (int)RAYCAST_MISS;
                            HeightMap[x, z] = RAYCAST_MISS;
                        }
                    // }
                }
            }

            SmoothHeightMap();

            OnNewHeightMapData?.Invoke(HeightMapFiltered);

            yield return new WaitForSeconds(_scanTime);
        }
    }

    public void FinalizeHeightMap()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                var y = HeightMapFiltered[x, z];

                HeightMapFiltered[x, z] = y == (int)RAYCAST_MISS ? 0 : y;
            }
        }

        SmoothHeightMap();
    }

    void SmoothHeightMap()
    {
        // Check all neighbors, smooth outliers
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                var coord = new Vector2Int(x, z);
                var neighbors = HexUtils.GetNeighborOffsetCoordinatesList(coord, HexGrid.Instance.Orientation);

                HeightMapFiltered[x, z] = SmoothedValue(coord, neighbors);
            }
        }
    }

    int SmoothedValue(Vector2Int coord, List<Vector2Int> neighbors)
    {
        var coordY = HeightMapFiltered[coord.x, coord.y];

        List<int> neighborYValues = new();
        foreach (var n in neighbors)
        {
            if (!HexGrid.Instance.InRange(n.x, n.y)) continue;

            var neighborY = HeightMapFiltered[n.x, n.y];

            neighborYValues.Add(neighborY);
        }

        var dict = neighborYValues.ToLookup(x => x);
        var maxCount = dict.Max(x => x.Count());

        // if smoothingThreshold num of neighbors have the same value, smooth 
        var smoothingThreshold = 4;
        if (maxCount >= smoothingThreshold)
        {
            var mostSeenValue = dict.Where(x => x.Count() == maxCount).First().Key;
            return mostSeenValue;
        }

        return coordY;
    }
}