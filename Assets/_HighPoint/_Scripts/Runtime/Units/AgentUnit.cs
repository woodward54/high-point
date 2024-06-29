using UnityEngine;
using Pathfinding;
using UnityHFSM;
using System;
using Pathfinding.ECS;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(FollowerEntity), typeof(AIDestinationSetter))]
public class AgentUnit : Unit, IOffMeshLinkHandler, IOffMeshLinkStateMachine
{
    protected AgentConfig AgentConfig => (AgentConfig)UnitConfig;

    protected FollowerEntity FollowerEntity { get; private set; }
    protected AIDestinationSetter DestSetter { get; private set; }

    protected static readonly int SPEED_PARAMETER = Animator.StringToHash("Speed");
    protected float SmoothSpeed = 0;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TryGetComponent<FollowerEntity>(out var ai)) ai.onTraverseOffMeshLink = this;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TryGetComponent<FollowerEntity>(out var ai)) ai.onTraverseOffMeshLink = null;
    }

    protected override void AddStates()
    {
        FSM.AddState(UnitState.Search, new AgentSearchState<UnitState>(this, true));
        FSM.AddState(UnitState.Confused, new AgentConfusedState<UnitState>(this, FollowerEntity, DestSetter));
        FSM.AddState(UnitState.Move, new MoveState<UnitState>(this, FollowerEntity, DestSetter));
        FSM.AddState(UnitState.Attack, new AttackState<UnitState>(this, transform));
        FSM.AddState(UnitState.Dead, new DeadState<UnitState>(this));

        FSM.SetStartState(UnitState.Search);
    }

    protected override void AddTransitions()
    {
        FSM.AddTransition(new Transition<UnitState>(UnitState.Search, UnitState.Move, t => Target != null));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Search, UnitState.Confused, t => Target == null));

        FSM.AddTransition(new Transition<UnitState>(UnitState.Move, UnitState.Search, NeedsNewTarget));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Move, UnitState.Attack, IsTargetAttackable));

        FSM.AddTransition(new Transition<UnitState>(UnitState.Attack, UnitState.Search, NeedsNewTarget));

        FSM.AddTransition(new Transition<UnitState>(UnitState.Confused, UnitState.Search, IsTargetInRange));

        FSM.AddTransitionFromAny(UnitState.Dead, t => IsDead);
    }

    protected override void SetupFromSO()
    {
        base.SetupFromSO();
    }

    protected override void Awake()
    {
        FollowerEntity = GetComponent<FollowerEntity>();
        DestSetter = GetComponent<AIDestinationSetter>();

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        FollowerEntity.stopDistance *= HexGrid.Instance.HexSize;
    }

    protected override void Update()
    {
        base.Update();

        // SmoothSpeed = Mathf.Lerp(SmoothSpeed, (Agent.enabled && Agent.hasPath) ? Agent.velocity.magnitude : 0,
        //     Time.deltaTime);
        // Animator.SetFloat(SPEED_PARAMETER, SmoothSpeed);
    }

    protected override void Die()
    {
        // Give some time for the death animation to play out
        StartCoroutine(DieAfter(2f));
    }

    protected bool IsTargetAttackable(Transition<UnitState> _) => IsTargetDamageable(_) && IsTargetInRange(_);

    protected bool IsTargetDamageable(Transition<UnitState> _) =>
            Target != null &&
            Target.gameObject.GetComponent<IDamageable>() != null;

    public bool IsTargetInRange(Transition<UnitState> _) =>
            Target != null &&
            (Target.transform.position - transform.position).sqrMagnitude <= AttackRangeSquared;

    protected bool NeedsNewTarget(Transition<UnitState> _)
    {
        if (Target == null) return true;

        var unit = Target.GetComponent<Unit>();
        if (unit != null && unit.IsDead) return true;

        return false;
    }

    // Off mesh Nav movement (climbing ladders & ramps)
    IOffMeshLinkStateMachine IOffMeshLinkHandler.GetOffMeshLinkStateMachine(AgentOffMeshLinkTraversalContext context) => this;

    void IOffMeshLinkStateMachine.OnFinishTraversingOffMeshLink(AgentOffMeshLinkTraversalContext context)
    {
        Animator.CrossFadeInFixedTime(AnimatorStates.MOVE, 0.2f);
        // Debug.Log("An agent finished traversing an off-mesh link");
    }

    void IOffMeshLinkStateMachine.OnAbortTraversingOffMeshLink()
    {
        Animator.CrossFadeInFixedTime(AnimatorStates.MOVE, 0.2f);
        Debug.Log("An agent aborted traversing an off-mesh link");
    }

    IEnumerable IOffMeshLinkStateMachine.OnTraverseOffMeshLink(AgentOffMeshLinkTraversalContext ctx)
    {
        var startCell = HexGrid.Instance.GetNearest(ctx.link.relativeStart);

        var start = ctx.link.relativeStart;
        var end = ctx.link.relativeEnd;

        if (startCell.CellMods.Any(m => m.GetComponent<Ramp>() != null))
        {
            // Ramp

        }
        else
        {
            // Ladder
            Animator.CrossFadeInFixedTime(AnimatorStates.CLIMB, 0.2f);
            end = (ctx.link.relativeEnd + start) / 2f;
            end.y = ctx.link.relativeEnd.y;
        }

        var dir = end - start;
        var magnitude = dir.magnitude / HexGrid.Instance.HexSize;

        // Disable local avoidance while traversing the off-mesh link.
        // If it was enabled, it will be automatically re-enabled when the agent finishes traversing the link.
        ctx.DisableLocalAvoidance();

        // Move and rotate the agent to face the other side of the link.
        // When reaching the off-mesh link, the agent may be facing the wrong direction.

        // TODO: The agents are moving very fast to the start position here. Is there a way to lower the speed?
        while (!ctx.MoveTowards(
            position: start,
            rotation: Quaternion.LookRotation(dir, ctx.movementPlane.up),
            gravity: true,
            slowdown: true).reached)
        {
            yield return null;
        }

        var climbDuration = magnitude / FollowerEntity.maxSpeed * 1.2f;

        for (float t = 0; t < climbDuration; t += ctx.deltaTime)
        {
            ctx.transform.Position = Vector3.Lerp(start, end, t / climbDuration);
            yield return null;
        }
    }
}