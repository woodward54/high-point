using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class HexCell
{
    [field: Header("Cell Properties")]
    [field: SerializeField] public TerrainType TerrainType { get; private set; }
    // [field: SerializeField] public BuildingConfig BuildingType { get; private set; }

    // TODO: convert this to Vector3Int, need to update all the Hex HexUtils logic to work with 3d
    [field: SerializeField] public Vector3Int OffsetCoordinates { get; private set; }
    [field: SerializeField] public Vector3Int CubeCoordinates { get; private set; }
    [field: SerializeField] public Vector2Int AxialCoordinates { get; private set; }
    [field: NonSerialized] public List<HexCell> Neighbors { get; private set; }

    [SerializeField]
    private ICellState state = new HiddenState();
    public ICellState State
    {
        get { return state; }
        private set
        {
            state = value;
        }
    }

    public BuildingUnit Building;
    public Transform Terrain { get; private set; }
    Transform _subTerrain;

    // Ladders, ramps, ect
    readonly public List<BaseCellMod> CellMods = new();

    // public Transform Building { get; private set; }

    public void SetCoordinates(Vector3Int coords)
    {
        OffsetCoordinates = coords;
        CubeCoordinates = HexUtils.OffsetToCube(coords, HexGrid.Instance.Orientation);
        AxialCoordinates = HexUtils.CubeToAxial(CubeCoordinates);
    }

    public void SetTerrainType(TerrainType type)
    {
        TerrainType = type;

        if (Terrain != null)
        {
            Terrain.GetComponent<MeshRenderer>().material.color = GetColorPattern();

            // TODO: might need to update more stuff on the cell like the terrain type prefab
        }
    }

    public void CreateTerrain()
    {
        var hexSize = HexGrid.Instance.HexSize;

        if (TerrainType == null)
        {
            Debug.LogError("TerrainType is null");
            return;
        }
        if (TerrainType.Prefab == null)
        {
            Debug.LogError("TerrainType Prefab is null");
            return;
        }

        Vector3 terrainPos = HexGrid.Instance.GetHexPosition(OffsetCoordinates);

        Terrain = UnityEngine.Object.Instantiate(
            TerrainType.Prefab,
            terrainPos,
            Quaternion.identity,
            HexGrid.Instance.transform);

        // Sub Terrain (dirt)
        CreateSubTerrain();

        Terrain.name = OffsetCoordinates.ToString();
        Terrain.gameObject.layer = LayerMask.NameToLayer("Grid");

        Terrain.GetComponent<MeshRenderer>().material.color = GetColorPattern();

        var scale = Vector3.one;
        scale.x = HexGrid.Instance.HexTerrainSize;
        scale.y = HexGrid.Instance.HexSize;
        scale.z = HexGrid.Instance.HexTerrainSize;
        Terrain.transform.localScale = Vector3.Scale(scale, Terrain.transform.localScale);

        if (HexGrid.Instance.Orientation == HexOrientation.FlatTop)
        {
            Terrain.Rotate(new Vector3(0, 30, 0));
        }

        int randomRotation = UnityEngine.Random.Range(0, 6);
        Terrain.Rotate(new Vector3(0, randomRotation * 60, 0));

        HexTerrain hexTerrain = Terrain.GetComponent<HexTerrain>();
        hexTerrain.OnSelect += OnSelect;
        hexTerrain.OnDeselect += OnDeselect;
    }

    public void UpdateTerrain()
    {
        Vector3 terrainPos = HexGrid.Instance.GetHexPosition(OffsetCoordinates);

        Terrain.name = OffsetCoordinates.ToString();
        Terrain.DOMove(terrainPos, 0.5f).SetEase(Ease.OutExpo);

        UpdateSubTerrain();
    }

    void UpdateSubTerrain()
    {
        var ratio = HexGrid.Instance.YStepSize / HexGrid.Instance.HexSize;
        var yScale = (OffsetCoordinates.y * ratio * 2f) - 1f;
        var s = new Vector3(0.8f, yScale, 0.8f);

        bool isVisible = OffsetCoordinates.y > 0;
        if (isVisible)
        {
            _subTerrain.DOScale(s, 0.5f).SetEase(Ease.OutExpo);
        }

        _subTerrain.gameObject.SetActive(isVisible);
    }

    void CreateSubTerrain()
    {
        _subTerrain = UnityEngine.Object.Instantiate(
                        MapDataGenerator.Instance.SubTerrain,
                        Vector3.zero,
                        Quaternion.identity,
                        Terrain.transform);

        _subTerrain.name = "SubTerrain";
        _subTerrain.tag = "SubTerrain";
        _subTerrain.gameObject.layer = LayerMask.NameToLayer("Grid");

        _subTerrain.position = Terrain.position + new Vector3(0f, -0.5f, 0f);

        UpdateSubTerrain();
    }

    public void CreateBuilding(BuildingConfig type)
    {
        // ClearBuilding();

        // BuildingType = type;

        // Building = UnityEngine.Object.Instantiate(
        //     BuildingType.Prefab,
        //     Terrain.transform.position,
        //     Terrain.transform.rotation,
        //     Terrain.transform);

        // Building.transform.localScale = new Vector3(1f, 2f, 1f);

        // var spawnUnitSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpawnUnitSystem>();

        // spawnUnitSystem.SpawnUnit(type,
        //                 Terrain.transform.position,
        //                 Terrain.transform.rotation,
        //                 0.6f);
    }

    public void SetNeighbors(List<HexCell> neighbors)
    {
        Neighbors = neighbors;
    }

    public void ClearTerrain()
    {
        if (Terrain != null)
        {
            HexTerrain hexTerrain = Terrain.GetComponent<HexTerrain>();
            hexTerrain.OnSelect -= OnSelect;
            hexTerrain.OnDeselect -= OnDeselect;
            // hexTerrain.OnMouseEnterAction -= OnMouseEnter;
            // hexTerrain.OnMouseExitAction -= OnMouseExit;

            UnityEngine.Object.Destroy(Terrain.gameObject);
        }

        foreach (var mod in CellMods)
        {
            UnityEngine.Object.Destroy(mod.gameObject);

            // TODO: remove connection from pathfinding graph
        }

        CellMods.Clear();
    }

    public void ChangeState(ICellState newState)
    {
        if (newState == null)
        {
            Debug.LogError("Trying to set null state.");
            return;
        }

        if (State != newState)
        {
            State.Exit(this);

            State = newState;
            State.Enter(this);
        }
    }

    Color GetColorPattern()
    {
        if (TerrainType == null) return Color.magenta;

        var dampFactor = 0.025f;

        var offset = OffsetCoordinates.x % 2 == 0 ? 1 : 0;
        var zReminder = (OffsetCoordinates.z + offset) % 3;

        var dampening = 1f;
        dampening -= zReminder * dampFactor;

        return TerrainType.Color * dampening;
    }

    public void OnSelect()
    {
        ChangeState(State.OnSelect());
    }

    public void OnDeselect()
    {
        ChangeState(State.OnDeselect());
    }

    // public void OnFocus()
    // {
    //     ChangeState(State.OnFocus());
    // }
}