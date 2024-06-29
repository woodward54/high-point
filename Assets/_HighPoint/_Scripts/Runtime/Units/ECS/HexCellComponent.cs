using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct HexCellComponent : IComponentData
{
    public int3 OffsetCoordinates { get; private set; }
    public int3 CubeCoordinates { get; private set; }
    public int2 AxialCoordinates { get; private set; }
    public NativeList<HexCellComponent> Neighbors { get; private set; }

    public void SetCoordinates(int3 coords)
    {
        OffsetCoordinates = coords;
        CubeCoordinates = HexEntityUtils.OffsetToCube(coords, HexGrid.Instance.Orientation);
        AxialCoordinates = HexEntityUtils.CubeToAxial(CubeCoordinates);
    }
}