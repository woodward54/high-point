using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    void OnSceneGUI()
    {
        HexGrid hexGrid = (HexGrid)target;

        for (int x = 0; x < hexGrid.Width; x++)
        {
            for (int z = 0; z < hexGrid.Depth; z++)
            {
                Vector3Int coords = new(x, 0, z);
                var center = hexGrid.GetHexPosition(coords);

                var cubeCoord = HexUtils.OffsetToCube(x, z, hexGrid.Orientation);
                var axialCoord = HexUtils.OffsetToAxial(x, z, hexGrid.Orientation);

                Handles.Label(center + Vector3.forward * 0.5f, $"[{x}, {z}]");
                // Handles.Label(center, $"({cubeCoord.x}, {cubeCoord.y}, {cubeCoord.z})");
                // Handles.Label(center - Vector3.forward * 0.5f, $"{{{axialCoord.x}, {axialCoord.y}}}");
            }
        }
    }
}

