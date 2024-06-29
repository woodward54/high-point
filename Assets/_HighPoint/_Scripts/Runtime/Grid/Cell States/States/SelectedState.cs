using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SelectedState : BaseCellState
{
    public override CellState State => CellState.Selected;

    public override void Enter(HexCell cell)
    {
        // Debug.Log($"Cell {cell.AxialCoordinates} is entering Selected State");

        // CameraController.Instance.onDeselectAction += cell.OnDeselect;
        // CameraController.Instance.onFocusAction += cell.OnFocus;
        // CameraController.Instance.IsLocked = true;
        // CameraController.Instance.CameraTarget.transform.position = cell.Terrain.transform.position;

        var hexSize = HexGrid.Instance.HexSize;
        var coords = HexGrid.Instance.GetHexPosition(cell.OffsetCoordinates);

        cell.Terrain.DOMoveY(coords.y + 0.2f * hexSize, 0.2f).SetEase(Ease.OutBack);
    }

    public override void Exit(HexCell cell)
    {
        // Debug.Log($"Cell {cell.AxialCoordinates} is exiting Selected State");

        // CameraController.Instance.onDeselectAction -= cell.OnDeselect;
        // CameraController.Instance.onFocusAction -= cell.OnFocus;
        // CameraController.Instance.IsLocked = false;

        var coords = HexGrid.Instance.GetHexPosition(cell.OffsetCoordinates);
        cell.Terrain.DOMoveY(coords.y, 0.2f).SetEase(Ease.OutBack);
    }

    public override ICellState OnSelect()
    {
        return new VisibleState();
    }

    public override ICellState OnDeselect()
    {
        return new VisibleState();
    }
}