using UnityEngine;
using Pathfinding;
using UnityHFSM;
using System;
using Drawing;

public class Worker : AgentUnit
{
    [field: Header("Worker Properties")]
    [field: SerializeField] public Transform LadderPrefab { get; private set; }
    [field: SerializeField] public Transform HalfLadderPrefab { get; private set; }
    [field: SerializeField] public Transform RampPrefab { get; private set; }

    // State Context vars
    public HexCell LadderStartCell;
    public HexCell LadderEndCell;

    EventBinding<GridUpdatedEvent> GridUpdatedBinding;

    protected override void OnEnable()
    {
        base.OnEnable();

        GridUpdatedBinding = new EventBinding<GridUpdatedEvent>(HandleGridUpdated);
        Bus<GridUpdatedEvent>.Register(GridUpdatedBinding);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Bus<GridUpdatedEvent>.Unregister(GridUpdatedBinding);
    }

    private void HandleGridUpdated(GridUpdatedEvent @event)
    {
        if (@event.UpdatedCell == LadderStartCell)
        {
            // Set null to recalculate target
            SetTarget(null);
        }
    }

    protected override void AddStates()
    {
        FSM.AddState(UnitState.Search, new WorkerSearchState<UnitState>(this, AttackRange));
        FSM.AddState(UnitState.Move, new MoveState<UnitState>(this, FollowerEntity, DestSetter));
        FSM.AddState(UnitState.Attack, new WorkerBuildState<UnitState>(this));
        FSM.AddState(UnitState.Dead, new DeadState<UnitState>(this));

        FSM.SetStartState(UnitState.Search);
    }

    protected override void AddTransitions()
    {
        FSM.AddTransition(new Transition<UnitState>(UnitState.Search, UnitState.Move, t => Target != null));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Move, UnitState.Search, NeedsNewTarget));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Move, UnitState.Attack, IsTargetInRange));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Attack, UnitState.Search, NeedsNewTarget));

        FSM.AddTransitionFromAny(UnitState.Dead, t => IsDead);
    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();

        if (GizmoContext.InSelection(this))
        {
            using (Draw.WithDuration(30f))
            {
                using (Draw.WithLineWidth(2.5f))
                {
                    Draw.Cross(LadderStartCell.Terrain.position, 0.1f, Color.red);
                    Draw.Cross(LadderEndCell.Terrain.position, 0.1f, Color.blue);
                }
            }
        }
    }
}