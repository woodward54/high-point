using System;
using UnityEngine;

public enum SlewMode
{
    None,
    YAxis,
    ThreeAxis
}

public class AttackState<TStateType> : UnitBaseState<TStateType>
{
    protected float _attackRange => OwnUnit.AttackRange;
    protected float _damagePerSecond => OwnUnit.DamagePerSecond;
    protected float _secondsPerAttack => OwnUnit.SecondsPerAttack;
    protected float _damageDelay => OwnUnit.DamageDelay;

    protected float _lastAttackTime;

    protected Transform _slewTransform;
    protected SlewMode _slewMode;
    protected float _slewSpeed;
    protected float _slewVerticalOffset;
    protected bool _needsLineOfSight;

    Action _onAttack;

    public AttackState(Unit unit, Transform slewTransform, SlewMode slewMode = SlewMode.None, float slewSpeed = 1f, float slewVerticalOffset = 0f, bool needsLineOfSight = false, Action onAttack = null) : base(unit)
    {
        _slewMode = slewMode;
        _slewSpeed = slewSpeed;
        _slewVerticalOffset = slewVerticalOffset;
        _needsLineOfSight = needsLineOfSight;

        _slewTransform = slewTransform;
        _onAttack = onAttack;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        OwnUnit.Animator.CrossFade(AnimatorStates.IDLE, 0.2f);

        _lastAttackTime = Time.time - _secondsPerAttack;
    }

    public override void OnLogic()
    {
        base.OnLogic();

        SlewToTarget();

        if (!IsTargetInView()) return;

        if (Time.time > _lastAttackTime + _secondsPerAttack)
        {
            _lastAttackTime = Time.time;

            ExecuteAttack();
        }
    }

    protected virtual void ExecuteAttack()
    {
        OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.ATTACK, 0.2f);

        var dmg = _damagePerSecond * _secondsPerAttack;

        if (OwnUnit.Target.TryGetComponent(out Unit unit))
        {
            unit.TakeDamage(dmg, OwnUnit, _damageDelay);
        }
        else
        {
            // Noop - we should never be attacking a Transform without a Unit
        }

        _onAttack?.Invoke();
    }

    protected void SlewToTarget()
    {
        if (_slewMode == SlewMode.None) return;

        var targetPos = OwnUnit.Target.position;

        switch (_slewMode)
        {
            case SlewMode.YAxis:
                targetPos.y = _slewTransform.position.y;
                break;

            case SlewMode.ThreeAxis:
                targetPos += Vector3.up * _slewVerticalOffset;
                break;
        }

        var targetDir = targetPos - _slewTransform.position;
        var targetRotation = Quaternion.LookRotation(targetDir);

        _slewTransform.rotation = Quaternion.Slerp(_slewTransform.rotation, targetRotation, _slewSpeed * Time.deltaTime);
    }

    bool IsTargetInView()
    {
        if (!_needsLineOfSight) return true;
        if (_slewMode == SlewMode.None) return true;

        var targetPos = OwnUnit.Target.position;

        switch (_slewMode)
        {
            case SlewMode.YAxis:
                targetPos.y = _slewTransform.position.y;
                break;

            case SlewMode.ThreeAxis:
                targetPos += Vector3.up * _slewVerticalOffset;
                break;
        }

        var targetDir = targetPos - _slewTransform.position;
        var angleDif = Vector3.Angle(targetDir, _slewTransform.forward);

        return angleDif <= 10f;
    }
}