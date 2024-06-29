using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using UnityHFSM;

public class Cannon : BuildingUnit
{
    [field: Header("Cannon Properties")]
    [SerializeField] Transform _cannonPivot;
    [SerializeField] Transform _cannonBlowback;
    [SerializeField] VisualEffect _shootVfx;

    readonly int _shootId = Shader.PropertyToID("OnShoot");

    protected override void AddStates()
    {
        FSM.AddState(UnitState.Search, new SearchState<UnitState>(this));
        FSM.AddState(UnitState.Attack, new AttackState<UnitState>(this, _cannonPivot, SlewMode.ThreeAxis, 2f, HexGrid.Instance.HexSize * 1.75f, true, OnAttack));
        FSM.AddState(UnitState.Dead, new DeadState<UnitState>(this));

        FSM.SetStartState(UnitState.Search);
    }

    protected override void AddTransitions()
    {
        FSM.AddTransition(new Transition<UnitState>(UnitState.Attack, UnitState.Search, NeedsNewTarget));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Search, UnitState.Attack, IsTargetInRange));

        FSM.AddTransitionFromAny(UnitState.Dead, t => IsDead);
    }

    void OnAttack()
    {
        _shootVfx.SendEvent(_shootId);

        _cannonBlowback.localPosition = new Vector3(0f, 0f, -0.44f);
        _cannonBlowback.DOLocalMoveZ(0f, SecondsPerAttack / 2f).SetEase(Ease.OutCirc);
    }
}