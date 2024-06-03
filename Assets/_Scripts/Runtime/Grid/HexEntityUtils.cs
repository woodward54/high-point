using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

public static class HexEntityUtils
{
    public static float OuterRadius(float hexSize)
    {
        return hexSize;
    }

    public static float InnerRadius(float hexSize)
    {
        return hexSize * 0.866025404f;
    }

    public static float3 Corner(float hexSize, HexOrientation orientation, int index)
    {
        float angle = 60f * -index;

        if (orientation == HexOrientation.PointyTop)
        {
            angle -= 30f;
        }

        float3 corner = new float3(
            hexSize * math.cos(angle * math.TORADIANS),
            0f,
            hexSize * math.sin(angle * math.TORADIANS)
            );

        return corner;
    }

    public static float3[] Corners(float hexSize, HexOrientation orientation)
    {
        float3[] corners = new float3[6];
        for (int i = 0; i < 6; i++)
        {
            corners[i] = Corner(hexSize, orientation, i);
        }
        return corners;
    }

    public static float3 Center(float hexSize, int3 coords, HexOrientation orientation)
    {
        // Null check to suppress editor errors
        if (HexGrid.Instance == null) return float3.zero;

        float3 center;
        if (orientation == HexOrientation.PointyTop)
        {
            center.x = (coords.x + coords.z * 0.5f - coords.z / 2) * (InnerRadius(hexSize) * 2f);
            center.y = coords.y * hexSize;
            center.z = coords.z * (OuterRadius(hexSize) * 1.5f);
        }
        else
        {
            center.x = coords.x * (OuterRadius(hexSize) * 1.5f);
            center.y = coords.y * hexSize;
            center.z = (coords.z + coords.x * 0.5f - coords.x / 2) * (InnerRadius(hexSize) * 2f);
        }

        return center + (float3)HexGrid.Instance.transform.position;
    }

    public static int3 OffsetToCube(int3 offsetCoord, HexOrientation orientation)
    {
        return OffsetToCube(offsetCoord.x, offsetCoord.z, orientation);
    }

    public static int3 OffsetToCube(int x, int z, HexOrientation orientation)
    {
        if (orientation == HexOrientation.PointyTop)
        {
            return AxialToCube(OffsetToAxialPointy(x, z));
        }
        else
        {
            return AxialToCube(OffsetToAxialFlat(x, z));
        }
    }

    public static float3 AxialToCube(float q, float r)
    {
        return new float3(q, r, -q - r);
    }

    public static int3 AxialToCube(int q, int r)
    {
        return new int3(q, r, -q - r);
    }

    public static int3 AxialToCube(int2 axialCoord)
    {
        return AxialToCube(axialCoord.x, axialCoord.y);
    }

    public static int2 AxialToOffset(int2 axialCoord, HexOrientation orientation)
    {
        return CubeToOffset(AxialToCube(axialCoord), orientation);
    }

    public static int2 CubeToAxial(int q, int r, int s)
    {
        return new int2(q, r);
    }

    public static int2 CubeToAxial(int3 cube)
    {
        return new int2(cube.x, cube.y);
    }

    public static int2 CubeToAxial(int2 cube)
    {
        return new int2(cube.x, cube.y);
    }

    public static int2 OffsetToAxial(int x, int z, HexOrientation orientation)
    {
        if (orientation == HexOrientation.PointyTop)
        {
            return OffsetToAxialPointy(x, z);
        }
        else
        {
            return OffsetToAxialFlat(x, z);
        }
    }

    private static int2 OffsetToAxialFlat(int col, int row)
    {
        var q = col;
        var r = row - (col - (col & 1)) / 2;
        return new int2(q, r);
    }

    private static int2 OffsetToAxialPointy(int col, int row)
    {
        var q = col - (row - (row & 1)) / 2;
        var r = row;
        return new int2(q, r);
    }

    public static int2 CubeToOffset(int x, int y, int z, HexOrientation orientation)
    {
        if (orientation == HexOrientation.PointyTop)
        {
            return CubeToOffsetPointy(x, y, z);
        }
        else
        {
            return CubeToOffsetFlat(x, y, z);
        }
    }

    public static int2 CubeToOffset(int3 offsetCoord, HexOrientation orientation)
    {
        return CubeToOffset(offsetCoord.x, offsetCoord.y, offsetCoord.z, orientation);
    }

    private static int2 CubeToOffsetPointy(int x, int y, int z)
    {
        var offsetCoordinates = new int2(x + (y - (y & 1)) / 2, y);
        return offsetCoordinates;
    }

    private static int2 CubeToOffsetFlat(int x, int y, int z)
    {
        var offsetCoordinates = new int2(x, y + (x - (x & 1)) / 2);
        return offsetCoordinates;
    }

    /// <summary>
    /// Rounds the cube coordinates to the nearest hexagon center.
    /// Used for getting the nearest hexagon to a point in space.
    /// </summary>
    /// <param name="frac"> 
    /// Frac is the fractional cube coordinates.
    /// </param>
    /// <returns></returns>
    private static int3 CubeRound(float3 frac)
    {
        int3 roundedCoordinates = new();
        int rx = (int)math.round(frac.x);
        int ry = (int)math.round(frac.y);
        int rz = (int)math.round(frac.z);
        float xDiff = math.abs(rx - frac.x);
        float yDiff = math.abs(ry - frac.y);
        float zDiff = math.abs(rz - frac.z);
        if (xDiff > yDiff && xDiff > zDiff)
        {
            rx = -ry - rz;
        }
        else if (yDiff > zDiff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }
        roundedCoordinates.x = rx;
        roundedCoordinates.y = ry;
        roundedCoordinates.z = rz;
        return roundedCoordinates;
    }

    /// <summary>
    /// Rounds the axial coordinates to the nearest hexagon center.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public static int2 AxialRound(float2 coords)
    {
        return CubeToAxial(CubeRound(AxialToCube(coords.x, coords.y)));
    }

    /// <summary>
    /// Converts a point in space to the nearest hexagon center
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="hexSize"></param>
    /// <param name="orientation"></param>
    /// <returns>Axial Coordinate</returns>
    public static int2 CoordinateToAxial(float x, float z, float hexSize, HexOrientation orientation)
    {
        if (orientation == HexOrientation.PointyTop)
        {
            return CoordinateToPointyAxial(x, z, hexSize);
        }
        else
        {
            return CoordinateToFlatAxial(x, z, hexSize);
        }
    }

    /// <summary>
    /// Helper function for CoordinateToAxial.
    /// It gets a fractional axial coordinate from a point in space for a pointy top orientation.
    /// </summary>
    private static int2 CoordinateToPointyAxial(float x, float z, float hexSize)
    {
        float2 pointyHexCoordinates = new()
        {
            x = (math.sqrt(3) / 3 * x - 1f / 3 * z) / hexSize,
            y = 2f / 3 * z / hexSize
        };

        return AxialRound(pointyHexCoordinates);
    }

    /// <summary>
    /// Helper function for CoordinateToAxial.
    /// It gets a fractional axial coordinate from a point in space for a flat top orientation.
    /// </summary>
    private static int2 CoordinateToFlatAxial(float x, float z, float hexSize)
    {
        float2 flatHexCoordinates = new()
        {
            x = 2f / 3 * x / hexSize,
            y = (-1f / 3 * x + math.sqrt(3) / 3 * z) / hexSize
        };
        return AxialRound(flatHexCoordinates);
    }

    /// <summary>
    /// Converts a point in space to the nearest hexagon center
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="hexSize"></param>
    /// <param name="orientation"></param>
    /// <returns>Offset coordinate</returns>
    public static int2 CoordinateToOffset(float x, float z, float hexSize, HexOrientation orientation)
    {
        return CubeToOffset(AxialToCube(CoordinateToAxial(x, z, hexSize, orientation)), orientation);
    }

    public static List<int2> GetNeighborCoordinatesList(int2 axialCoordinates)
    {
        List<int2> neighbors = new List<int2>
        {
            new(axialCoordinates.x + 1, axialCoordinates.y),
            new(axialCoordinates.x - 1, axialCoordinates.y),
            new(axialCoordinates.x, axialCoordinates.y + 1),
            new(axialCoordinates.x, axialCoordinates.y - 1),
            new(axialCoordinates.x + 1, axialCoordinates.y - 1),
            new(axialCoordinates.x - 1, axialCoordinates.y + 1)
        };
        return neighbors;
    }

    public static List<int2> GetNeighborOffsetCoordinatesList(int2 offsetCoordinates, HexOrientation orientation)
    {
        return GetNeighborOffsetCoordinatesList(offsetCoordinates.x, offsetCoordinates.y, orientation);
    }

    public static List<int2> GetNeighborOffsetCoordinatesList(int x, int z, HexOrientation orientation)
    {
        var neighborsAxial = GetNeighborCoordinatesList(OffsetToAxial(x, z, orientation));

        return neighborsAxial.Select(n => AxialToOffset(n, orientation)).ToList();
    }

}