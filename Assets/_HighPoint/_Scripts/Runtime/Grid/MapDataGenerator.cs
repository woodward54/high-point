using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MapDataGenerator : Singleton<MapDataGenerator>
{
    public int Width = 256;
    public int Depth = 256;
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
    [Tooltip("Whether or not to automatically update the noise map when a value is changed.")]
    public bool AutoUpdate = true;
    [Tooltip("Whether or not to use the hex grid width and height information to generate the noise map.")]
    public bool UseHexGrid = true;
    [Tooltip("Whether or not to generate the noise map on start.")]
    public bool GenerateMapOnStart = true;
    public bool UseThreadedGeneration = true;

    public List<TerrainHeight> Biomes = new List<TerrainHeight>();
    public Transform SubTerrain;

    // Latest generated maps
    public float[,] NoiseMap { get; private set; }
    public TerrainType[,] TerrainMap { get; private set; }
    public Color[] ColorMap { get; private set; }

    // Events
    public event Action OnNewMapGenerationStated;
    public event Action<float[,]> OnNoiseMapGenerated;
    public event Action<TerrainType[,]> OnTerrainMapGenerated;
    public event Action<Color[], int, int> OnColorMapGenerated;

    HexGrid _hexGrid;
    int _seed;

    protected override void OnAwake()
    {
        _hexGrid = GetComponent<HexGrid>();
    }

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        OnNewMapGenerationStated?.Invoke();

        _seed = Blackboard.Instance.Seed;

        if (UseHexGrid && (_hexGrid != null))
        {
            Width = _hexGrid.Width;
            Depth = _hexGrid.Depth;
        }

        ValidateSettings();

        StartCoroutine(GenerateMapCoroutine());
    }

    IEnumerator GenerateMapCoroutine()
    {
        // Clear the current maps
        NoiseMap = null;
        TerrainMap = null;
        ColorMap = null;

        // If we are in play mode, we generate the noise map on a separate thread
        if (Application.isPlaying && UseThreadedGeneration)
        {
            Task task = Task.Run(() =>
            {
                NoiseMap = Noise.GenerateNoiseMap(Width, Depth, NoiseScale, _seed, Octaves, Persistance, Lacunarity, Offset);
                TerrainMap = AssignTerrainTypes(NoiseMap);
                ColorMap = GenerateColorsFromTerrain(TerrainMap);

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
        // If we are not in play mode, we generate the noise map on the main thread
        // In testing I found that threading is much slower in the editor than in a build or play mode
        else
        {
            NoiseMap = Noise.GenerateNoiseMap(Width, Depth, NoiseScale, _seed, Octaves, Persistance, Lacunarity, Offset);
            TerrainMap = AssignTerrainTypes(NoiseMap);
            ColorMap = GenerateColorsFromTerrain(TerrainMap);
        }
        //We invoke separate events for each map generated so that the parts of code that interested only in one map can subscribe to that event
        OnNoiseMapGenerated?.Invoke(NoiseMap);
        OnColorMapGenerated?.Invoke(ColorMap, Width, Depth);
        OnTerrainMapGenerated?.Invoke(TerrainMap);

        yield return null;
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
        // Make sure the width and height are not less than 1

        Width = Mathf.Max(Width, 1);
        Depth = Mathf.Max(Depth, 1);
    }

    // Assigns a terrain type to each point on the noise map based on the height of the point as compared to the height of the biomes
    private TerrainType[,] AssignTerrainTypes(float[,] noiseMap)
    {
        TerrainType[,] terrainMap = new TerrainType[Width, Depth];

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                float currentHeight = noiseMap[x, z];
                for (int i = 0; i < Biomes.Count; i++)
                {
                    if (currentHeight <= Biomes[i].Height)
                    {
                        terrainMap[x, z] = Biomes[i].TerrainType;
                        break;
                    }
                }
            }
        }

        return terrainMap;
    }

    // Generates a color map from the terrain map by getting the color of each terrain type
    private Color[] GenerateColorsFromTerrain(TerrainType[,] terrainMap)
    {
        Color[] colorMap = new Color[Width * Depth];

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                colorMap[z * Width + x] = terrainMap[x, z].Color;
            }
        }
        return colorMap;
    }

    private void OnValidate()
    {
        ValidateSettings();
    }

}

[Serializable]
public struct TerrainHeight
{
    public float Height;
    public TerrainType TerrainType;
}