using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using DG.Tweening;

[RequireComponent(typeof(SeedProvider)),
 RequireComponent(typeof(MapDataGenerator))]
public class BaseGenerator : Singleton<BaseGenerator>
{
    [Tooltip("The scale of the noise map. The higher the scale, the more zoomed in the noise map will be. " +
               "The lower the scale, the more zoomed out the noise map will be. " +
               "The scale should be greater than 0.")]
    public float NoiseScale = 0.5f;
    [Tooltip("The number of layers of noise to generate. More layers means more detail in the noise map.")]
    public int Octaves = 6;
    [Range(0, 1)]
    [Tooltip("The change in amplitude between octaves. The amplitude of each octave is multiplied by this value.")]
    public float Persistance = 0.5f;
    [Tooltip("The change in frequency between octaves. The frequency of each octave is multiplied by this value.")]
    public float Lacunarity = 2f;
    [Tooltip("The offset of the noise map.")]
    public Vector2 Offset = Vector2.zero;

    [Tooltip("Percentage of the map that will be Base Area")]
    [Range(0, 1)]
    [SerializeField] float _baseAreaThreshold = 0.3f;

    [Header("Base Engine Components")]
    public BuildingConfig Wall;
    public BuildingConfig Cannon;
    BuildingConfig EmptyBaseArea;
    public TerrainType BaseTerrain;

    [field: Header("Other Prefabs")]
    [field: SerializeField] public BuildingConfig WallConfig { get; private set; }
    // [field: SerializeField] public Transform Ladder { get; private set; }
    // [field: SerializeField] public Transform Ramp { get; private set; }

    public BuildingConfig[,] BuildingMap { get; private set; }

    float[,] _noiseMap;

    int _seed;

    HexGrid _hexGrid;
    MapDataGenerator _map;

    public event Action<BuildingConfig[,]> OnBaseMapGenerated;
    public event Action OnBuildingInstancesGenerated;

    protected override void OnAwake()
    {
        EmptyBaseArea = ScriptableObject.CreateInstance<BuildingConfig>();

        _hexGrid = GetComponent<HexGrid>();
        _map = GetComponent<MapDataGenerator>();
    }

    void OnEnable()
    {
        HexGrid.OnTerrainFinalized += GenerateBase;
    }

    void OnDisable()
    {
        HexGrid.OnTerrainFinalized -= GenerateBase;
    }

    void GenerateBase()
    {
        _seed = SeedProvider.Instance.Seed;

        ValidateSettings();

        StartCoroutine(GenerateBaseMapCoroutine());
    }

    IEnumerator GenerateBaseMapCoroutine()
    {
        _noiseMap = null;
        BuildingMap = null;

        if (Application.isPlaying && _map.UseThreadedGeneration)
        {
            Task task = Task.Run(() =>
            {
                _noiseMap = Noise.GenerateNoiseMap(_hexGrid.Width, _hexGrid.Depth, NoiseScale, _seed, Octaves, Persistance, Lacunarity, Offset);
                BuildingMap = AssignBuildingTypesByTileHeight(_noiseMap);

            }).ContinueWith(task =>
            {
                // Handle exceptions if any
                if (task.Exception != null)
                {
                    Debug.LogError(task.Exception);
                }
            });

            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
        else
        {
            _noiseMap = Noise.GenerateNoiseMap(_hexGrid.Width, _hexGrid.Depth, NoiseScale, _seed, Octaves, Persistance, Lacunarity, Offset);
            BuildingMap = AssignBuildingTypesByTileHeight(_noiseMap);
        }

        MainThreadDispatcher.Instance.Enqueue(() => StartCoroutine(InstantiateBuildings()));
        OnBaseMapGenerated?.Invoke(BuildingMap);
        yield return null;
    }

    // No Unity API allowed - including looking up transform data, Instantiation, etc.
    BuildingConfig[,] AssignBuildingTypesByTileHeight(float[,] noiseMap)
    {
        System.Random rand = new();

        BuildingConfig[,] buildingMap = new BuildingConfig[_hexGrid.Width, _hexGrid.Depth];

        var cellsByHeight = _hexGrid.GetCellRegionsByHeight();
        cellsByHeight.RemoveAll((g) => g.Key == 0);

        if (cellsByHeight.Count() == 0)
        {
            // No regions
            Debug.LogWarning("AssignBuildingTypesByTileHeight: No height regions found above 0!");
            return buildingMap;
        }

        var yLevelWithMostTiles = cellsByHeight.OrderByDescending(g => g.Count()).First().ToList();

        var coords = yLevelWithMostTiles.Select(n => (n.OffsetCoordinates.x, n.OffsetCoordinates.z));

        // Find biggest region at yLevel
        var biggestRegionAtYLevel = SplitNodesIntoRegions(coords.ToList()).OrderByDescending(r => r.Count()).First();

        var edgeOfPlateau = BuildRegionPerimeter(biggestRegionAtYLevel);

        // Inset the walls by 1 tile
        var insetRegion = biggestRegionAtYLevel.ToList();
        insetRegion.RemoveAll(n => edgeOfPlateau.Contains(n));

        var insetPermitter = BuildRegionPerimeter(insetRegion);

        foreach (var node in insetRegion)
        {
            // Rand number between 0-1
            float buildingType = (float)rand.NextDouble();

            if (buildingType >= 0.95f)
            {
                buildingMap[node.Item1, node.Item2] = Cannon;
            }
            else
            {
                buildingMap[node.Item1, node.Item2] = EmptyBaseArea;
            }

        }

        foreach (var wall in insetPermitter)
        {
            buildingMap[wall.Item1, wall.Item2] = Wall;
        }

        return buildingMap;
    }

    // No Unity API allowed - including looking up transform data, Instantiation, etc.
    BuildingConfig[,] AssignBuildingTypesByNoise(float[,] noiseMap)
    {
        BuildingConfig[,] buildingMap = new BuildingConfig[_hexGrid.Width, _hexGrid.Depth];

        List<List<(int, int)>> islands = new();
        for (int x = 0; x < _hexGrid.Width; x++)
        {
            for (int z = 0; z < _hexGrid.Depth; z++)
            {
                if (islands.SelectMany(i => i)
                            .Any(c => c.Item1 == x && c.Item2 == z)) continue;

                if (noiseMap[x, z] > _baseAreaThreshold) continue;

                islands.Add(BuildRegion(x, z, noiseMap));
            }
        }

        if (islands.Count == 0)
        {
            Debug.LogError("No Islands found!");
            // TODO: handle this edge case, maybe have a default castle? try a new noise map?
            return buildingMap;
        }

        // Filters
        // islands.RemoveAll(i => i.Count < 5);
        var baseArea = islands.OrderByDescending(t => t.Count).First();

        // Perimeter Walls - scan line algorithm

        // Note here- I tried the scanline approach, it was way too complicated on a hex grid...

        List<(int, int)> walls = BuildRegionPerimeter(baseArea);

        foreach (var wall in walls)
        {
            buildingMap[wall.Item1, wall.Item2] = Wall;
        }

        // Remove walls that only border other walls or empty space
        List<(int, int)> wallsToRemove = new();
        foreach (var node in baseArea)
        {
            var neighbors = HexUtils.GetNeighborOffsetCoordinatesList(node.Item1, node.Item2, _hexGrid.Orientation);
            neighbors.RemoveAll(n => !_hexGrid.InRange(n.x, n.y));

            bool validWall = false;
            foreach (var n in neighbors)
            {
                if (baseArea.Contains((n.x, n.y)) &&
                    buildingMap[n.x, n.y] != Wall)
                {
                    validWall = true;
                    break;
                }
            }

            if (!validWall)
                wallsToRemove.Add((node.Item1, node.Item2));
        }

        foreach (var invalidWall in wallsToRemove)
        {
            buildingMap[invalidWall.Item1, invalidWall.Item2] = null;
        }

        return buildingMap;
    }

    // BFS
    List<List<(int, int)>> SplitNodesIntoRegions(List<(int, int)> nodes)
    {
        List<List<(int, int)>> islands = new();

        List<(int, int)> seenNodes = new();

        int islandIndex = 0;
        while (seenNodes.Count != nodes.Count)
        {
            Queue<(int, int)> unvisitedNodes = new();
            var newIslandNode = nodes.Where(n => !seenNodes.Contains(n)).First();
            unvisitedNodes.Enqueue(newIslandNode);

            islands.Add(new List<(int, int)>());

            while (unvisitedNodes.Count > 0)
            {
                var (x, z) = unvisitedNodes.Dequeue();

                if (!_hexGrid.InRange(x, z)) continue;

                if (seenNodes.Contains((x, z))) continue;

                if (!nodes.Contains((x, z))) continue;

                seenNodes.Add((x, z));
                islands[islandIndex].Add((x, z));

                var neighbors = HexUtils.GetNeighborOffsetCoordinatesList(x, z, _hexGrid.Orientation);

                foreach (var n in neighbors)
                {
                    unvisitedNodes.Enqueue((n.x, n.y));
                }
            }

            islandIndex++;
        }

        return islands;
    }

    List<(int, int)> BuildRegionPerimeter(List<(int, int)> region)
    {
        List<(int, int)> perimeterNodes = new();

        List<(int, int)> seenNodes = new();
        Queue<(int, int)> unvisitedNodes = new();

        var startCoord = region[0];
        unvisitedNodes.Enqueue(startCoord);

        while (unvisitedNodes.Count > 0)
        {
            var (x, z) = unvisitedNodes.Dequeue();

            if (!_hexGrid.InRange(x, z)) continue;

            if (seenNodes.Contains((x, z))) continue;

            if (!region.Contains((x, z))) continue;

            seenNodes.Add((x, z));

            var neighbors = HexUtils.GetNeighborOffsetCoordinatesList(x, z, _hexGrid.Orientation);

            bool touchingOutsideRegion = false;
            foreach (var n in neighbors)
            {
                unvisitedNodes.Enqueue((n.x, n.y));

                if (!region.Contains((n.x, n.y)))
                    touchingOutsideRegion = true;
            }

            if (touchingOutsideRegion)
            {
                perimeterNodes.Add((x, z));
            }
        }

        return perimeterNodes;
    }

    // BFS
    List<(int, int)> BuildRegion(int startX, int startZ, float[,] noiseMap)
    {
        List<(int, int)> seenNodes = new();
        Queue<(int, int)> unvisitedNodes = new();
        unvisitedNodes.Enqueue((startX, startZ));

        while (unvisitedNodes.Count > 0)
        {
            var (x, z) = unvisitedNodes.Dequeue();

            if (seenNodes.Contains((x, z))) continue;

            seenNodes.Add((x, z));

            // North, South, East, West
            if (_hexGrid.InRange(x, z + 1) && noiseMap[x, z + 1] <= _baseAreaThreshold)
                unvisitedNodes.Enqueue((x, z + 1));

            if (_hexGrid.InRange(x, z - 1) && noiseMap[x, z - 1] <= _baseAreaThreshold)
                unvisitedNodes.Enqueue((x, z - 1));

            if (_hexGrid.InRange(x + 1, z) && noiseMap[x + 1, z] <= _baseAreaThreshold)
                unvisitedNodes.Enqueue((x + 1, z));

            if (_hexGrid.InRange(x - 1, z) && noiseMap[x - 1, z] <= _baseAreaThreshold)
                unvisitedNodes.Enqueue((x - 1, z));
        }

        return seenNodes;
    }

    // Handled by coroutine and currently the most expensive operation
    IEnumerator InstantiateBuildings()
    {
        var batchSize = HexGrid.Instance.BatchSize;

        List<(Vector2Int, BuildingConfig)> _buildingsToSpawn = new();

        for (int x = 0; x < _hexGrid.Width; x++)
        {
            for (int z = 0; z < _hexGrid.Depth; z++)
            {
                // int flippedX = _hexGrid.Width - x - 1;
                // int flippedZ = _hexGrid.Depth - z - 1;

                if (BuildingMap[x, z] != null)
                {
                    _buildingsToSpawn.Add((new Vector2Int(x, z), BuildingMap[x, z]));
                }
            }
        }

        Debug.Log("Spawning " + _buildingsToSpawn.Where(b => b.Item2 != EmptyBaseArea).Count() + " buildings.");

        int batchCount = 0;
        int totalBatches = Mathf.CeilToInt(_buildingsToSpawn.Count / batchSize);
        for (int i = 0; i < _buildingsToSpawn.Count; i++)
        {
            var buildingToSpawn = _buildingsToSpawn[i];
            var cell = HexGrid.Instance.GetCell(buildingToSpawn.Item1.x, buildingToSpawn.Item1.y);

            cell.SetTerrainType(BaseTerrain);

            // default is empty base area building (needed to set terrain, but no building is needed)
            if (buildingToSpawn.Item2 != EmptyBaseArea)
            {
                var terrain = cell.Terrain;

                var bldg = UnitManager.Instance.SpawnUnit(buildingToSpawn.Item2,
                                                terrain.position,
                                                Quaternion.identity);

                // Animate in
                var baseSize = bldg.transform.localScale;
                bldg.transform.localScale = new Vector3(0f, 0f, 0f);
                bldg.transform.DOScale(baseSize, 1.0f).SetEase(Ease.OutBack);
            }

            // Yield every batchSize hex cells
            if (i % batchSize == 0 && i != 0)
            {
                batchCount++;
                // OnCellBatchGenerated?.Invoke((float)batchCount / totalBatches);
                yield return null;
            }
        }

        OnBuildingInstancesGenerated?.Invoke();
    }

    private void ValidateSettings()
    {
        // We make sure the octaves is not less than 0
        Octaves = Mathf.Max(Octaves, 0);
        // We make sure the lacunarity is not less than 1
        Lacunarity = Mathf.Max(Lacunarity, 1);
        // We make sure the persistance is between 0 and 1
        Persistance = Mathf.Clamp01(Persistance);
        // We make sure the scale is not 0 because we will be dividing by it
        NoiseScale = Mathf.Max(NoiseScale, 0.0001f);
    }
}