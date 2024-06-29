using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class HiddenState : BaseCellState
{
    public override CellState State => CellState.Hidden;

    public override void Enter(HexCell cell)
    {
        if (cell.Terrain == null)
        {
            Debug.LogWarning("Terrain is null");
            return;
        }

        cell.Terrain.gameObject.SetActive(false);
    }

    public override void Exit(HexCell cell)
    {
        if (cell.Terrain == null)
        {
            Debug.LogWarning("Terrain is null");
            return;
        }

        cell.Terrain.gameObject.SetActive(true);
        var baseSize = HexGrid.Instance.HexTerrainSize; 
        var startScale = new Vector3(baseSize, cell.Terrain.transform.localScale.y, baseSize);

        cell.Terrain.transform.localScale = new Vector3(0f, startScale.y, 0f);

        var delay = Random.Range(0f, 1f);
        cell.Terrain.DOScale(startScale, 2.0f).SetEase(Ease.OutExpo).SetDelay(delay);
    }
}