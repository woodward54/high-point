// using DG.Tweening;
// using UnityEngine;
// using System.Threading.Tasks;

// public class HighlightedState : BaseCellState
// {
//     public override CellState State => CellState.Highlighted;

//     public override void Enter(HexCell cell)
//     {
//         // Debug.Log($"Cell {cell.AxialCoordinates} is entering Highlighted State");

//         //Tween local Scale of the cell.Terrain gameobject
//         // LeanTween.scale(cell.Terrain.gameObject, Vector3.one * 1.2f, 0.2f).setEase(LeanTweenType.easeOutBack);
//         // LeanTween.moveY(cell.Terrain.gameObject, 5f, 0.2f).setEase(LeanTweenType.easeOutBack);

//         var coords = HexGrid.Instance.GetHexPosition( cell.OffsetCoordinates, HexGrid.Instance.Orientation);
//         cell.Terrain.DOMoveY(coords.y + 0.2f, 0.2f).SetEase(Ease.OutBack);
//     }

//     public override void Exit(HexCell cell)
//     {
//         // Debug.Log($"Cell {cell.AxialCoordinates} is exiting Highlighted State");

//         //Tween local Scale of the cell.Terrain gameobject
//         // LeanTween.scale(cell.Terrain.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
//         // LeanTween.moveY(cell.Terrain.gameObject, 0f, 0.2f).setEase(LeanTweenType.easeOutBack);

//         // cell.Terrain.DOMoveY(-0.2f, 0.2f).SetEase(Ease.OutBack).SetRelative();

//         var coords = HexGrid.Instance.GetHexPosition( cell.OffsetCoordinates, HexGrid.Instance.Orientation);
//         cell.Terrain.DOMoveY(coords.y, 0.2f).SetEase(Ease.OutBack);
//     }

//     // public override ICellState OnMouseExit()
//     // {
//     //     return new VisibleState();
//     // }

//     public override ICellState OnSelect()
//     {
//         return new SelectedState();
//     }

//     // public override bool IsExitTransitioning()
//     // {
//     //     return _isExitTransitioning;
//     // }
// }