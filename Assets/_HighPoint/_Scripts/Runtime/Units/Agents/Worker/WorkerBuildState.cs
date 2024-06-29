using System;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class WorkerBuildState<TStateType> : AttackState<TStateType>
{
    readonly Worker _workerUnit;

    public WorkerBuildState(Unit unit) : base(unit, unit.transform, SlewMode.YAxis)
    {
        _workerUnit = (Worker)unit;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // Worker needs to wait before building the first ladder/ramp
        _lastAttackTime = Time.time;

        OwnUnit.Animator.CrossFade(AnimatorStates.WORKER_BUILD_STANDING, 0.2f);
    }

    protected override void ExecuteAttack()
    {
        OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.WORKER_BUILD_STANDING, 0.2f);

        var startCell = _workerUnit.LadderStartCell;
        var targetCell = _workerUnit.LadderEndCell;

        var touchingSide = HexUtils.GetTouchingSideIndex(
                startCell.AxialCoordinates,
                targetCell.AxialCoordinates);

        var heightDiff = targetCell.OffsetCoordinates.y - startCell.OffsetCoordinates.y;

        // if (heightDiff == 1)
        // {
        //     SpawnRamp(startCell, touchingSide);

        //     // 0 is default tag
        //     FinalizeConnection(0);
        // }
        // else
        // {
        var widthToHeightRatio = (int)Math.Round(HexGrid.Instance.YStepSize / HexGrid.Instance.HexSize);

        // Each full ladder is 2 units tall
        var totalLaddersNeeded = widthToHeightRatio * heightDiff / 2f;
        var existingLadders = startCell.CellMods.Where(m => m is Ladder).Count();
        var remainingNeeded = totalLaddersNeeded - existingLadders;
        var curLadderIndex = totalLaddersNeeded - remainingNeeded;

        // Avoid race condition where another worker already finished this connection
        if (remainingNeeded <= 0f) return;

        var ladderYPos = startCell.Terrain.transform.position.y
                            + curLadderIndex * 2f * HexGrid.Instance.YStepSize / widthToHeightRatio;

        if (remainingNeeded <= 0.5f)
        {
            // Spawn half ladder (0.5)
            SpawnLadder(startCell, touchingSide, ladderYPos, true);
        }
        else
        {
            // Full ladder (1)
            SpawnLadder(startCell, touchingSide, ladderYPos, false);
        }

        if (remainingNeeded - 1 <= 0)
            FinalizeConnection(PathfindingTag.FromName("Ladder"));
        // }
    }

    void SpawnRamp(HexCell cell, int sideIndex)
    {
        if (cell.CellMods.Count > 0)
        {
            Debug.LogWarning("Trying to add a ramp to a cell that already has mods.");
            return;
        }

        var rot = Vector3.zero;
        rot.y = 270f + HexUtils.SideAngle(sideIndex);

        var rampGO = UnityEngine.Object.Instantiate(
            _workerUnit.RampPrefab,
            cell.Terrain.transform.position + new Vector3(0f, HexGrid.Instance.YStepSize / 2f, 0f),
            Quaternion.Euler(rot),
            HexGrid.Instance.TileMods);

        rampGO.transform.localScale *= HexGrid.Instance.HexSize;

        var rampMod = rampGO.GetComponent<Ramp>();

        cell.CellMods.Add(rampMod);
    }

    // 0 is bottom right side
    void SpawnLadder(HexCell cell, int sideIndex, float yWorldPos, bool halfLadder = false)
    {
        if (cell.CellMods.Any(m => m.GetComponent<Ramp>() != null))
        {
            Debug.LogWarning("Trying to add a ladder to a cell that already has a ramp.");
            return;
        }

        var center = HexGrid.Instance.GetHexPosition(cell.OffsetCoordinates);
        var corners = HexUtils.Corners(HexGrid.Instance.HexSize, HexGrid.Instance.Orientation);

        var a = center + corners[sideIndex % 6];
        var b = center + corners[(sideIndex + 1) % 6];

        var pos = (a + b) / 2f;
        pos.y = yWorldPos;

        var rot = Vector3.zero;
        rot.y = 180f + HexUtils.SideAngle(sideIndex);

        var ladderGO = UnityEngine.Object.Instantiate(
            halfLadder ? _workerUnit.HalfLadderPrefab : _workerUnit.LadderPrefab,
            pos,
            Quaternion.Euler(rot),
            HexGrid.Instance.TileMods);

        ladderGO.transform.localScale *= HexGrid.Instance.HexSize;

        var ladderMod = ladderGO.GetComponent<Ladder>();

        cell.CellMods.Add(ladderMod);
    }

    void FinalizeConnection(PathfindingTag tag)
    {
        var startCell = _workerUnit.LadderStartCell;
        var targetCell = _workerUnit.LadderEndCell;

        // Connect two nodes
        UpdatePathfinding.Instance.ConnectCells(startCell, targetCell, tag);
    }
}