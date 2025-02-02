using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap
    }

    public Renderer textureRenderer;
    public DrawMode drawMode;

    [SerializeField] private MapDataGenerator mapGenerator;
    private void Awake()
    {
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapDataGenerator>();
        }
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void GenerateTextureAndDraw(Color[] colorMap, int width, int height)
    {
        if (drawMode != DrawMode.ColorMap)
        {
            return;
        }
        // Debug.Log("Generating texture from colour map");
        Texture2D texture = TextureGenerator.TextureFromColorMap(colorMap, width, height);
        DrawTexture(texture);
    }

    private void GenerateTextureAndDraw(float[,] noiseMap)
    {
        if (drawMode != DrawMode.NoiseMap)
        {
            return;
        }

        // Debug.Log("Generating texture from height map");
        Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap);
        DrawTexture(texture);
    }

    public void DrawTexture(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    public void SubscribeToEvents()
    {
        // We unsubscribe from events in case we are already subscribed
        UnsubscribeFromEvents();

        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapDataGenerator>();
            if (mapGenerator == null)
            {
                throw new System.Exception("Map Generator not found");
            }
        }

        mapGenerator.OnColorMapGenerated += GenerateTextureAndDraw;
        mapGenerator.OnNoiseMapGenerated += GenerateTextureAndDraw;
    }

    private void UnsubscribeFromEvents()
    {
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapDataGenerator>();
            if (mapGenerator == null)
            {
                throw new System.Exception("Map Generator not found");
            }
        }

        mapGenerator.OnColorMapGenerated -= GenerateTextureAndDraw;
        mapGenerator.OnNoiseMapGenerated -= GenerateTextureAndDraw;
    }
}