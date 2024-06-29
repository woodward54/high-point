using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapDataGenerator))]
public class MapDataGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapDataGenerator mapGenerator = (MapDataGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGenerator.AutoUpdate)
            {
                mapGenerator.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGenerator.GenerateMap();
        }

        if (GUILayout.Button("Clear"))
        {
            HexGrid.Instance.ClearHexCells();
        }
    }
}